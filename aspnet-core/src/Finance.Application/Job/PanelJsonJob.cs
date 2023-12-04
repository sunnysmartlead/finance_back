using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Json;
using Finance.DemandApplyAudit;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Job
{
    public class PanelJsonJob : AsyncBackgroundJob<long>, ITransientDependency
    {
        private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;
        private readonly IRepository<ModelCountYear, long> _modelCountYearRepository;
        private readonly IRepository<Gradient, long> _gradientRepository;
        private readonly IRepository<Solution, long> _solutionRepository;
        private readonly IRepository<PanelJson, long> _panelJsonRepository;

        public PanelJsonJob(PriceEvaluationGetAppService priceEvaluationGetAppService, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<Gradient, long> gradientRepository, IRepository<Solution, long> solutionRepository, IRepository<PanelJson, long> panelJsonRepository)
        {
            _priceEvaluationGetAppService = priceEvaluationGetAppService;
            _modelCountYearRepository = modelCountYearRepository;
            _gradientRepository = gradientRepository;
            _solutionRepository = solutionRepository;
            _panelJsonRepository = panelJsonRepository;
        }

        /// <summary>
        /// 添加核价看板、核价表缓存
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async override Task ExecuteAsync(long auditFlowId)
        {
            #region 缓存核价表
            var data = from g in _gradientRepository.GetAll()
                       from s in _solutionRepository.GetAll()
                       from y in _modelCountYearRepository.GetAll()
                       where g.AuditFlowId == auditFlowId
                       && s.AuditFlowId == auditFlowId
                       && y.AuditFlowId == auditFlowId
                       select new //GetPriceEvaluationTableInput
                       {
                           AuditFlowId = auditFlowId,
                           GradientId = g.Id,
                           InputCount = 0,
                           SolutionId = s.Id,
                           Year = y.Year,
                           UpDown = y.UpDown,
                       };
            var result = await data.ToListAsync();
            var all = result.GroupBy(p => new { p.AuditFlowId, p.GradientId, p.InputCount, p.SolutionId, })
                .Select(p => new //GetPriceEvaluationTableInput
                {
                    AuditFlowId = p.Key.AuditFlowId,
                    GradientId = p.Key.GradientId,
                    InputCount = p.Key.InputCount,
                    SolutionId = p.Key.SolutionId,
                    Year = PriceEvalConsts.AllYear,
                    UpDown = YearType.Year
                }).Distinct();
            result.AddRange(all);


            foreach (var item in result)
            {
                //核价表
                var priceEvaluationTable = await _priceEvaluationGetAppService.GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    InputCount = item.InputCount,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                //其他成本项目2
                var otherCostItem2List = await _priceEvaluationGetAppService.GetOtherCostItem2List(new GetOtherCostItem2ListInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                //bom成本
                var bomCost = await _priceEvaluationGetAppService.GetBomCost(new GetBomCostInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                    InputCount = item.InputCount,
                });

                //制造成本汇总表（未修改）
                var manufacturingCostNoChange = await _priceEvaluationGetAppService.GetManufacturingCostNoChange(new GetManufacturingCostInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                //制造成本汇总表（已修改）
                var manufacturingCost = await _priceEvaluationGetAppService.GetManufacturingCost(new GetManufacturingCostInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                //物流成本（未修改）
                var logisticsCostNoChange = await _priceEvaluationGetAppService.GetLogisticsCostNoChange(new GetLogisticsCostInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                //物流成本（已修改）
                var logisticsCost = await _priceEvaluationGetAppService.GetLogisticsCost(new GetLogisticsCostInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                //质量成本（未修改）
                var qualityCostNoChange = await _priceEvaluationGetAppService.GetQualityCostNoChange(new GetOtherCostItemInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                //质量成本（已修改）
                var qualityCost = await _priceEvaluationGetAppService.GetQualityCost(new GetOtherCostItemInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                //损耗成本（未修改）
                var lossCostNoChange = await _priceEvaluationGetAppService.GetLossCostNoChange(new GetCostItemInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                //损耗成本（已修改）
                var lossCost = await _priceEvaluationGetAppService.GetLossCost(new GetCostItemInput
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                });

                await _panelJsonRepository.InsertAsync(new PanelJson
                {
                    AuditFlowId = item.AuditFlowId,
                    GradientId = item.GradientId,
                    InputCount = item.InputCount,
                    SolutionId = item.SolutionId,
                    Year = item.Year,
                    UpDown = item.UpDown,
                    DataJson = priceEvaluationTable.ToJsonString(),
                    OtherCostItem2List = otherCostItem2List.ToJsonString(),
                    BomCost = bomCost.ToJsonString(),
                    ManufacturingCostNoChange = manufacturingCostNoChange.ToJsonString(),
                    ManufacturingCost = manufacturingCost.ToJsonString(),
                    LogisticsCostNoChange = logisticsCostNoChange.ToJsonString(),
                    LogisticsCost = logisticsCost.ToJsonString(),
                    QualityCostNoChange = qualityCostNoChange.ToJsonString(),
                    QualityCost = qualityCost.ToJsonString(),
                    LossCostNoChange = lossCostNoChange.ToJsonString(),
                    LossCost = lossCost.ToJsonString(),
                });
            }

            #endregion
        }
    }
}
