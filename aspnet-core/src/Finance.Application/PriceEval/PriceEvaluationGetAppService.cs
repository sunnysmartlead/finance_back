using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Json;
using Abp.Linq.Extensions;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.BaseLibrary;
using Finance.DemandApplyAudit;
using Finance.EngineeringDepartment;
using Finance.Entering;
using Finance.Ext;
using Finance.FinanceMaintain;
using Finance.FinanceParameter;
using Finance.Hr;
using Finance.Infrastructure;
using Finance.NerPricing;
using Finance.NrePricing;
using Finance.PriceEval.Dto;
using Finance.PriceEval.Dto.AllManufacturingCost;
using Finance.PriceEval.Dto.MoqFormulas;
using Finance.PriceEval.Dto.ProjectSelf;
using Finance.Processes;
using Finance.ProductDevelopment;
using Finance.ProductionControl;
using Finance.ProjectManagement;
using Finance.PropertyDepartment.Entering.Method;
using Finance.PropertyDepartment.Entering.Model;
using LinqKit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using Newtonsoft.Json;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Tsp;
using Rougamo;
using Spire.Pdf.Exporting.XPS.Schema;
using Spire.Xls.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using test;

namespace Finance.PriceEval
{
    /// <summary>
    /// 获取核价
    /// </summary>
    //[ParameterValidator]
    public class PriceEvaluationGetAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 结构料Id前缀
        /// </summary>
        public const string StructureBomName = "s";

        /// <summary>
        /// 电子料Id前缀
        /// </summary>
        public const string ElectronicBomName = "e";
        #region 类初始化

        protected readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

        protected readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;

        protected readonly IRepository<Pcs, long> _pcsRepository;
        protected readonly IRepository<PcsYear, long> _pcsYearRepository;

        protected readonly IRepository<ModelCount, long> _modelCountRepository;
        protected readonly IRepository<ModelCountYear, long> _modelCountYearRepository;

        protected readonly IRepository<Requirement, long> _requirementRepository;




        protected readonly IRepository<ElectronicBomInfo, long> _electronicBomInfoRepository;
        protected readonly IRepository<StructureBomInfo, long> _structureBomInfoRepository;

        protected readonly IRepository<EnteringElectronicCopy, long> _enteringElectronicRepository;
        protected readonly IRepository<StructureElectronicCopy, long> _structureElectronicRepository;

        protected readonly IRepository<LossRateInfo, long> _lossRateInfoRepository;
        protected readonly IRepository<LossRateYearInfo, long> _lossRateYearInfoRepository;

        protected readonly IRepository<ExchangeRate, long> _exchangeRateRepository;

        protected readonly IRepository<ManufacturingCostInfo, long> _manufacturingCostInfoRepository;
        protected readonly IRepository<ProcessHoursEnteritem, long> _yearInfoRepository;
        protected readonly IRepository<WorkingHoursInfo, long> _workingHoursInfoRepository;
        protected readonly IRepository<RateEntryInfo, long> _rateEntryInfoRepository;
        protected readonly IRepository<ProductionControlInfo, long> _productionControlInfoRepository;
        protected readonly IRepository<QualityRatioEntryInfo, long> _qualityCostProportionEntryInfoRepository;
        protected readonly IRepository<UserInputInfo, long> _userInputInfoRepository;
        protected readonly IRepository<QualityRatioYearInfo, long> _qualityCostProportionYearInfoRepository;
        protected readonly IRepository<UPHInfo, long> _uphInfoRepository;
        protected readonly IRepository<AllManufacturingCost, long> _allManufacturingCostRepository;

        protected readonly IRepository<Gradient, long> _gradientRepository;
        protected readonly IRepository<GradientModel, long> _gradientModelRepository;
        protected readonly IRepository<GradientModelYear, long> _gradientModelYearRepository;

        protected readonly IRepository<Solution, long> _solutionRepository;
        private readonly IRepository<UpdateItem, long> _updateItemRepository;


        private readonly IRepository<BomEnter, long> _bomEnterRepository;
        private readonly IRepository<BomEnterTotal, long> _bomEnterTotalRepository;

        private readonly IRepository<PriceEvalJson, long> _priceEvalJsonRepository;
        private readonly NrePricingAppService _nrePricingAppService;
        private readonly IRepository<ShareCount, long> _shareCountRepository;

        private readonly IRepository<Logisticscost, long> _logisticscostRepository;

        private readonly IRepository<QualityCostRatio, long> _qualityCostRatioRepository;
        private readonly IRepository<QualityCostRatioYear, long> _qualityCostRatioYearRepository;

        private readonly IRepository<CustomerTargetPrice, long> _customerTargetPriceRepository;
        private readonly IRepository<FollowLineTangent, long> _followLineTangentRepository;
        private readonly IRepository<ProcessHoursEnterUph, long> _processHoursEnterUphRepository;
        private readonly IRepository<ProcessHoursEnterDevice, long> _processHoursEnterDeviceRepository;
        private readonly IRepository<ProcessHoursEnter, long> _processHoursEnterRepository;

        private readonly IRepository<PanelJson, long> _panelJsonRepository;


        private string errMessage = string.Empty;


        /// <summary>
        /// 构造函数
        /// </summary>
        public PriceEvaluationGetAppService(
            IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository,
            IRepository<PriceEvaluation, long> priceEvaluationRepository,
            IRepository<Pcs, long> pcsRepository,
            IRepository<PcsYear, long> pcsYearRepository,
            IRepository<ModelCount, long> modelCountRepository,
            IRepository<ModelCountYear, long> modelCountYearRepository,
            IRepository<Requirement, long> requirementRepository,
            IRepository<ElectronicBomInfo, long> electronicBomInfoRepository,
            IRepository<StructureBomInfo, long> structureBomInfoRepository,
            IRepository<EnteringElectronicCopy, long> enteringElectronicRepository,
            IRepository<StructureElectronicCopy, long> structureElectronicRepository,
            IRepository<LossRateInfo, long> lossRateInfoRepository,
            IRepository<LossRateYearInfo, long> lossRateYearInfoRepository,
            IRepository<ExchangeRate, long> exchangeRateRepository,
            IRepository<ManufacturingCostInfo, long> manufacturingCostInfoRepository,
            IRepository<ProcessHoursEnteritem, long> yearInfoRepository,
            IRepository<WorkingHoursInfo, long> workingHoursInfoRepository,
            IRepository<RateEntryInfo, long> rateEntryInfoRepository,
            IRepository<ProductionControlInfo, long> productionControlInfoRepository,
            IRepository<QualityRatioEntryInfo, long> qualityCostProportionEntryInfoRepository,
            IRepository<UserInputInfo, long> userInputInfoRepository,
            IRepository<QualityRatioYearInfo, long> qualityCostProportionYearInfoRepository,
            IRepository<UPHInfo, long> uphInfoRepository,
            IRepository<AllManufacturingCost, long> allManufacturingCostRepository,
            IRepository<Gradient, long> gradientRepository,
            IRepository<GradientModel, long> gradientModelRepository,
            IRepository<GradientModelYear, long> gradientModelYearRepository,
            IRepository<UpdateItem, long> updateItemRepository,
            IRepository<Solution, long> solutionRepository,
            IRepository<BomEnter, long> bomEnterRepository,
            IRepository<BomEnterTotal, long> bomEnterTotalRepository,
            NrePricingAppService nrePricingAppService,
            IRepository<ShareCount, long> shareCountRepository,
            IRepository<Logisticscost, long> logisticscostRepository,
            IRepository<QualityCostRatio, long> qualityCostRatioRepository,
            IRepository<QualityCostRatioYear, long> qualityCostRatioYearRepository,
            IRepository<CustomerTargetPrice, long> customerTargetPriceRepository,
            IRepository<FollowLineTangent, long> followLineTangentRepository,
            IRepository<ProcessHoursEnterUph, long> processHoursEnterUphRepository,
            IRepository<ProcessHoursEnterDevice, long> processHoursEnterDeviceRepository,
            IRepository<ProcessHoursEnter, long> processHoursEnterRepository,
            IRepository<PanelJson, long> panelJsonRepository)
        {
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _pcsRepository = pcsRepository;
            _pcsYearRepository = pcsYearRepository;
            _modelCountRepository = modelCountRepository;
            _modelCountYearRepository = modelCountYearRepository;
            _requirementRepository = requirementRepository;
            _electronicBomInfoRepository = electronicBomInfoRepository;
            _structureBomInfoRepository = structureBomInfoRepository;
            _enteringElectronicRepository = enteringElectronicRepository;
            _structureElectronicRepository = structureElectronicRepository;
            _lossRateInfoRepository = lossRateInfoRepository;
            _lossRateYearInfoRepository = lossRateYearInfoRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _manufacturingCostInfoRepository = manufacturingCostInfoRepository;
            _yearInfoRepository = yearInfoRepository;
            _workingHoursInfoRepository = workingHoursInfoRepository;
            _rateEntryInfoRepository = rateEntryInfoRepository;
            _productionControlInfoRepository = productionControlInfoRepository;
            _qualityCostProportionEntryInfoRepository = qualityCostProportionEntryInfoRepository;
            _userInputInfoRepository = userInputInfoRepository;
            _qualityCostProportionYearInfoRepository = qualityCostProportionYearInfoRepository;
            _uphInfoRepository = uphInfoRepository;
            _allManufacturingCostRepository = allManufacturingCostRepository;

            _gradientRepository = gradientRepository;
            _gradientModelRepository = gradientModelRepository;
            _gradientModelYearRepository = gradientModelYearRepository;

            _updateItemRepository = updateItemRepository;
            _solutionRepository = solutionRepository;
            _bomEnterRepository = bomEnterRepository;
            _nrePricingAppService = nrePricingAppService;

            _shareCountRepository = shareCountRepository;
            _bomEnterTotalRepository = bomEnterTotalRepository;

            _logisticscostRepository = logisticscostRepository;
            _qualityCostRatioRepository = qualityCostRatioRepository;
            _qualityCostRatioYearRepository = qualityCostRatioYearRepository;

            _customerTargetPriceRepository = customerTargetPriceRepository;
            _followLineTangentRepository = followLineTangentRepository;
            _processHoursEnterUphRepository = processHoursEnterUphRepository;
            _processHoursEnterDeviceRepository = processHoursEnterDeviceRepository;
            _processHoursEnterRepository = processHoursEnterRepository;

            _panelJsonRepository = panelJsonRepository;
        }



        #endregion

        #region 核价表


        /// <summary>
        /// 获取当前新增的项目的版本号
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public async virtual Task<int> GetQuoteVersion(string projectCode)
        {
            var count = await _priceEvaluationRepository.CountAsync(p => p.ProjectCode == projectCode);
            return count + 1;
        }

        /// <summary>
        /// 生成核价表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<CreatePriceEvaluationTableResult> CreatePriceEvaluationTable(CreatePriceEvaluationTableInput input)
        {
            var data = await (from g in _gradientModelYearRepository.GetAll()
                              join s in _solutionRepository.GetAll() on g.ProductId equals s.Productld
                              join m in _gradientModelRepository.GetAll() on g.GradientModelId equals m.Id
                              where g.AuditFlowId == input.AuditFlowId
                              select new
                              {
                                  g.ProductId,
                                  g.AuditFlowId,
                                  m.GradientId,
                                  SolutionId = s.Id,
                                  g.Year,
                                  g.UpDown
                              }).ToListAsync();

            var dtos = (await data.SelectAsync(async p => new PriceEvalJson
            {
                AuditFlowId = p.AuditFlowId,
                GradientId = p.GradientId,
                SolutionId = p.SolutionId,
                UpDown = p.UpDown,
                Year = p.Year,
                Json = JsonConvert.SerializeObject(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
                {
                    AuditFlowId = p.AuditFlowId,
                    GradientId = p.GradientId,
                    InputCount = 0,
                    SolutionId = p.SolutionId,
                    UpDown = p.UpDown,
                    Year = p.Year,
                })),
            })).ToList();

            await _priceEvalJsonRepository.BulkInsertAsync(dtos);
            return new CreatePriceEvaluationTableResult { IsSuccess = true, Message = "生成成功！" };

            //var noneInputCountOrYearmodelAny = await _modelCountRepository.GetAll()
            //    .Where(p => p.AuditFlowId == input.AuditFlowId && !p.Year.HasValue && !p.InputCount.HasValue).AnyAsync();
            //if (noneInputCountOrYearmodelAny)
            //{
            //    throw new FriendlyException($"模组的投入量和年份全部正确设置才可生成核价表！");
            //    //return new CreatePriceEvaluationTableResult { IsSuccess = false, Message = "模组的投入量和年份全部正确设置才可生成核价表！" };
            //}
            ////var dto = await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId)
            ////    .SelectAsync(async p => (JsonConvert.SerializeObject(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            ////    {
            ////        AuditFlowId = p.AuditFlowId,
            ////        InputCount = p.InputCount.Value,
            ////        SolutionId = 0,
            ////        GradientId = 0,
            ////        UpDown = YearType.Year,
            ////        Year = p.Year.Value
            ////    })), p.Id, (JsonConvert.SerializeObject(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            ////    {
            ////        AuditFlowId = p.AuditFlowId,
            ////        InputCount = p.InputCount.Value,
            ////        SolutionId = 0,
            ////        GradientId = 0,
            ////        UpDown = YearType.Year,
            ////        Year = 0
            ////    })))));

            //var data = await (from g in _gradientModelYearRepository.GetAll()
            //                  join s in _solutionRepository.GetAll() on g.ProductId equals s.Productld
            //                  join m in _gradientModelRepository.GetAll() on g.GradientModelId equals m.Id
            //                  where g.AuditFlowId == input.AuditFlowId
            //                  select new
            //                  {
            //                      g.ProductId,
            //                      g.AuditFlowId,
            //                      m.GradientId,
            //                      SolutionId = s.Id,
            //                      g.Year,
            //                      g.UpDown
            //                  }).ToListAsync();
            //var dto = await data.SelectAsync(async p => (JsonConvert.SerializeObject(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            //{
            //    AuditFlowId = p.AuditFlowId,
            //    InputCount = 0,
            //    SolutionId = p.SolutionId,
            //    GradientId = p.GradientId,
            //    UpDown = p.UpDown,
            //    Year = p.Year,
            //})), p.ProductId, (JsonConvert.SerializeObject(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            //{
            //    AuditFlowId = p.AuditFlowId,
            //    InputCount = 0,
            //    SolutionId = p.SolutionId,
            //    GradientId = p.GradientId,
            //    UpDown = p.UpDown,
            //    Year = 0
            //})))));


            //foreach (var item in dto)
            //{
            //    var entity = await _modelCountRepository.GetAsync(item.ProductId);
            //    entity.TableJson = item.Item1;
            //    entity.TableAllJson = item.Item3;
            //}


            ////获取所有年份核价表

            //var dataYear = await (from m in _modelCountRepository.GetAll()
            //                      join y in _modelCountYearRepository.GetAll() on m.Id equals y.ProductId
            //                      where m.AuditFlowId == input.AuditFlowId
            //                      select new { m.Id, m.InputCount, y.Year, m.AuditFlowId, YearId = y.Id }).SelectAsync(async p => (JsonConvert.SerializeObject(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            //                      {
            //                          AuditFlowId = p.AuditFlowId,
            //                          InputCount = p.InputCount.Value,
            //                          //ProductId = p.Id,
            //                          Year = p.Year
            //                      })), p.YearId));

            //foreach (var item in dataYear)
            //{
            //    var entity = await _modelCountYearRepository.GetAsync(item.YearId);
            //    entity.TableJson = item.Item1;
            //}

            //return new CreatePriceEvaluationTableResult { IsSuccess = true, Message = "生成成功！" };

        }

        /// <summary>
        /// 查询已经生成的核价表（和GetPriceEvaluationTable接口的区别是，GetPriceEvaluationTable接口是实时数据，此接口是保存的数据，不会随着数据变化而变化）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ExcelPriceEvaluationTableDto> GetPriceEvaluationTableResult(GetPriceEvaluationTableResultInput input)
        {
            var json = await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.Id == input.ProductId)
                .Select(p => input.IsAll ? p.TableAllJson : p.TableJson).FirstOrDefaultAsync();
            if (json.IsNullOrWhiteSpace())
            {
                throw new FriendlyException("核价表尚未生成！");
            }
            var dto = JsonConvert.DeserializeObject<ExcelPriceEvaluationTableDto>(json);
            return dto;
        }

        /// <summary>
        /// 设置投入量和年份（用来控制生成的核价表的投入量和年份）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task SetPriceEvaluationTableInputCount(SetPriceEvaluationTableInputCount input)
        {
            foreach (var item in input.ModelCountInputCount)
            {
                await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.Id == item.ProductId)
                    .UpdateFromQueryAsync(p => new ModelCount
                    {
                        InputCount = item.InputCount,
                        Year = item.Year,
                    });
            }
        }

        /// <summary>
        /// 获取核价表模组的InputCount（投入量）和年份
        /// </summary>
        /// <param name="auditFlowId">流程Id</param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ModelCountInputCountEditDto>> GetPriceEvaluationTableInputCount(long auditFlowId)
        {
            var data = from m in _modelCountRepository.GetAll()
                       where m.AuditFlowId == auditFlowId
                       select new ModelCountInputCountEditDto
                       {
                           Id = m.Id,
                           ProductName = m.Product,
                           InputCount = m.InputCount,
                           Year = m.Year
                       };
            var result = await data.ToListAsync();

            return new ListResultDto<ModelCountInputCountEditDto>(result);
        }

        /// <summary>
        /// 获取项目核价表
        /// </summary>
        /// <param name="input">获取项目核价表的接口参数输入</param>
        /// <returns>项目核价表</returns>
        public async virtual Task<ExcelPriceEvaluationTableDto> GetPriceEvaluationTable(GetPriceEvaluationTableInput input)
        {
            var json = await _panelJsonRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
            && p.GradientId == input.GradientId && p.SolutionId == input.SolutionId && p.InputCount == input.InputCount
            && p.Year == input.Year && p.UpDown == input.UpDown);
            if (json is not null)
            {
                return json.DataJson.FromJsonString<ExcelPriceEvaluationTableDto>();
            }
            var solution = await _solutionRepository.GetAsync(input.SolutionId);
            var productId = solution.Productld;

            //获取标题
            var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);

            //获取零件名
            var modelName = await (from mc in _modelCountRepository.GetAll()
                                   where mc.AuditFlowId == input.AuditFlowId && mc.Id == productId
                                   select mc.Product).FirstOrDefaultAsync();

            //物料成本
            var electronicAndStructureList = await this.GetBomCost(new GetBomCostInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, InputCount = input.InputCount, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown });

            //电子料汇总信息
            var electronicSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.ElectronicName).GroupBy(p => p.CategoryName)
                .Select(p => new ElectronicSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //电子料合计
            var electronicSumValue = electronicSum.Sum(o => o.Value);

            //结构料汇总信息
            var structuralSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.StructuralName).GroupBy(p => p.CategoryName)
                .Select(p => new StructuralSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //胶水等辅材汇总信息
            var glueMaterialSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.GlueMaterialName).GroupBy(p => p.CategoryName)
                .Select(p => new GlueMaterialSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //SMT外协汇总信息
            var smtOutSourceSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.SMTOutSourceName).GroupBy(p => p.CategoryName)
                .Select(p => new SMTOutSourceSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //包材汇总信息
            var packingMaterialSum = electronicAndStructureList.Where(p => p.SuperType == FinanceConsts.PackingMaterialName).GroupBy(p => p.CategoryName)
                .Select(p => new PackingMaterialSum { Name = p.Key, Value = p.Sum(o => o.TotalMoneyCyn) }).ToList();

            //制造成本
            var manufacturingCostAll = await this.GetManufacturingCost(new GetManufacturingCostInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown });

            //制造成本合计
            var manufacturingAllCost = manufacturingCostAll.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal;

            //其他成本项目2
            var getOtherCostItem2 = await GetOtherCostItem2(new GetOtherCostItem2Input { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown });

            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {
                //获取总年数
                var yearCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == productId).Select(p => p.Year).ToListAsync();

                //计算需求量（模组总量）
                var moudelCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).Where(p => yearCount.Contains(p.Year)).SumAsync(p => p.Quantity);

                #region 项目成本

                var costItem = await GetLossCost(new GetCostItemInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown, });

                #endregion

                #region 其他成本项目

                var otherCostItem = await GetOtherCostItem(new GetOtherCostItemInput
                {
                    AuditFlowId = input.AuditFlowId,
                    GradientId = input.GradientId,
                    SolutionId = input.SolutionId,
                    Year = input.Year,
                    UpDown = input.UpDown
                });

                #endregion

                var result = new PriceEvaluationTableDto
                {
                    Year = 0,
                    UpDown = YearType.Year,
                    Title = $"{priceEvaluation?.Title}项目{modelName}核价表（量产/样品）（全生命周期）",
                    Date = DateTime.Now,
                    InputCount = input.InputCount,//项目经理填写
                    RequiredCount = moudelCount,
                    Material = electronicAndStructureList,
                    ElectronicSumValue = electronicSumValue,
                    ElectronicSum = electronicSum,
                    StructuralSum = structuralSum,
                    GlueMaterialSum = glueMaterialSum,
                    SMTOutSourceSum = smtOutSourceSum,
                    PackingMaterialSum = packingMaterialSum,
                    ManufacturingCost = manufacturingCostAll,
                    LossCost = costItem,
                    OtherCostItem = otherCostItem,
                    OtherCostItem2 = getOtherCostItem2,
                    TotalCost = electronicAndStructureList.Sum(p => p.TotalMoneyCyn) + manufacturingAllCost + costItem.Sum(p => p.WastageCost) + costItem.Sum(p => p.MoqShareCount) + otherCostItem.Total + getOtherCostItem2.FirstOrDefault(p => p.ItemName == "单颗成本").Total.GetValueOrDefault(),

                    PreparedDate = DateTime.Now,//编制日期
                    AuditDate = DateTime.Now, //工作流取    审核日期
                    ApprovalDate = DateTime.Now,//工作流取  批准日期
                };
                var dto = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(result);
                DtoExcel(dto);

                //核价表总成本保留2位小数
                Math.Round(dto.TotalCost, 2);
                return dto;
            }
            else
            {
                var result = await GetData(input.Year, input.UpDown);
                var dto = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(result);
                DtoExcel(dto);

                //核价表总成本保留2位小数
                Math.Round(dto.TotalCost, 2);
                return dto;
            }

            async Task<PriceEvaluationTableDto> GetData(int year, YearType upDown)
            {
                //质量成本比例
                //var qualityCostProportionEntryInfo = await _qualityCostProportionYearInfoRepository.GetAll().FirstOrDefaultAsync(p => p.Year == year);
                //if (qualityCostProportionEntryInfo is null)
                //{
                //    qualityCostProportionEntryInfo = await _qualityCostProportionYearInfoRepository.GetAll().OrderByDescending(p => p.Year).FirstOrDefaultAsync();
                //}

                //获取终端走量数量
                var pcsCount = await _pcsYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).Where(p => p.Year == year).SumAsync(p => p.Quantity);

                //计算需求量（模组总量）
                var moudelCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).Where(p => p.Year == year).SumAsync(p => p.Quantity);

                //计算-成本项目
                var costItem = await this.GetLossCostByMaterial(year, electronicAndStructureList, new GetCostItemInput
                {
                    AuditFlowId = input.AuditFlowId,
                    GradientId = input.GradientId,
                    SolutionId = input.SolutionId,
                    UpDown = upDown,
                    Year = year,
                });

                #region 其他成本项目

                var otherCostItem = await GetOtherCostItem(new GetOtherCostItemInput
                {
                    AuditFlowId = input.AuditFlowId,
                    GradientId = input.GradientId,
                    SolutionId = input.SolutionId,
                    Year = year,
                    UpDown = upDown,
                });

                #endregion

                return new PriceEvaluationTableDto
                {
                    Year = year,
                    UpDown = upDown,
                    Title = $"{priceEvaluation?.Title}项目{modelName}核价表（量产/样品）({year}年)",
                    Date = DateTime.Now,
                    InputCount = input.InputCount,//项目经理填写
                    RequiredCount = moudelCount,
                    Material = electronicAndStructureList,
                    ElectronicSumValue = electronicSumValue,
                    ElectronicSum = electronicSum,
                    StructuralSum = structuralSum,
                    GlueMaterialSum = glueMaterialSum,
                    SMTOutSourceSum = smtOutSourceSum,
                    PackingMaterialSum = packingMaterialSum,
                    ManufacturingCost = manufacturingCostAll,
                    LossCost = costItem,
                    OtherCostItem = otherCostItem,
                    OtherCostItem2 = getOtherCostItem2,
                    TotalCost = electronicAndStructureList.Sum(p => p.TotalMoneyCyn) + manufacturingAllCost + costItem.Sum(p => p.WastageCost) + costItem.Sum(p => p.MoqShareCount) + otherCostItem.Total + getOtherCostItem2.FirstOrDefault(p => p.ItemName == "单颗成本").Total.GetValueOrDefault(),
                    PreparedDate = DateTime.Now,//编制日期
                    AuditDate = DateTime.Now,//工作流取    审核日期
                    ApprovalDate = DateTime.Now,//工作流取  批准日期
                };
            }
        }

        #endregion

        #region 其他成本项目

        /// <summary>
        /// 获取物流费
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<decimal> GetLogisticsFee(GetOtherCostItemInput input)
        {
            decimal logisticsFee;
            var solution = await _solutionRepository.GetAsync(input.SolutionId);

            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {
                //Sum（每年的物流成本*每年的月度需求量）/Sum(月度需求量)
                //var data = await (from m in _modelCountYearRepository.GetAll()
                //                  join p in _productionControlInfoRepository.GetAll() on m.Year.ToString() equals p.Year
                //                  where m.AuditFlowId == input.AuditFlowId && m.ProductId == input.ProductId
                //                  && p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId
                //                  select p.PerTotalLogisticsCost * m.Quantity).SumAsync();
                //var monthlyDemand = await _modelCountYearRepository
                //    .GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId).SumAsync(p => p.Quantity);


                //var data = await (from m in _gradientModelYearRepository.GetAll()
                //                  join p in _productionControlInfoRepository.GetAll() on m.Year.ToString() equals p.Year
                //                  where m.AuditFlowId == input.AuditFlowId && m.ProductId == solution.Productld
                //                  && p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld
                //                  select p.PerTotalLogisticsCost * m.Count).SumAsync();
                //var monthlyDemand = await _gradientModelYearRepository
                //    .GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld).SumAsync(p => p.Count);
                //logisticsFee = data / monthlyDemand;

                var data = await (from gm in _gradientModelRepository.GetAll()
                                  join gmy in _gradientModelYearRepository.GetAll() on gm.Id equals gmy.GradientModelId
                                  where gm.GradientId == input.GradientId && gm.AuditFlowId == input.AuditFlowId
                                  select gmy).ToListAsync();
                var logisticsCosts = await data.SelectAsync(async p =>
                {
                    var d = await GetLogisticsCost(new GetLogisticsCostInput { AuditFlowId = input.AuditFlowId, Year = p.Year, UpDown = p.UpDown, GradientId = input.GradientId, SolutionId = input.SolutionId });
                    return d.FirstOrDefault().PerTotalLogisticsCost * p.Count;
                });
                logisticsFee = logisticsCosts.Sum() / data.Count;
                //await _gradientModelYearRepository.GetAll().Where(p=>p.AuditFlowId == input.AuditFlowId && p.)

            }
            else
            {
                //物流成本
                //var productionControlInfo = await _productionControlInfoRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld && p.Year == input.Year.ToString());

                //物流费
                var data = await GetLogisticsCost(new GetLogisticsCostInput { AuditFlowId = input.AuditFlowId, Year = input.Year, UpDown = input.UpDown, GradientId = input.GradientId, SolutionId = input.SolutionId });
                logisticsFee = data.FirstOrDefault().PerTotalLogisticsCost;
            }
            return logisticsFee;
        }

        /// <summary>
        /// 获取其他成本项目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<OtherCostItem> GetOtherCostItem(GetOtherCostItemInput input)
        {
            //物流费
            decimal logisticsFee = await GetLogisticsFee(input);

            //质量成本
            var qualityCost = await this.GetQualityCost(new GetOtherCostItemInput
            {
                AuditFlowId = input.AuditFlowId,
                GradientId = input.GradientId,
                SolutionId = input.SolutionId,
                Year = input.Year,
                UpDown = input.UpDown
            });

            //计算-其他成本项目
            var otherCostItem = new OtherCostItem
            {
                Id = input.Year,
                Fixture = null,//留白
                LogisticsFee = logisticsFee,
                ProductCategory = qualityCost.ProductCategory,
                CostProportion = qualityCost.CostProportion,
                QualityCost = qualityCost.QualityCost,
                AccountingPeriod = 60, //账期写死60天
                CapitalCostRate = null,//留白
                TaxCost = null,//留白
            };
            otherCostItem.Total = otherCostItem.LogisticsFee + otherCostItem.QualityCost;//物流费+质量成本（MAX)+财务成本+税务成本（财务成本、税务成本一般都为0）
            return otherCostItem;
        }

        #endregion

        #region 其他成本项目2

        /// <summary>
        /// 获取修改项（其他成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<OtherCostItem2List>> GetUpdateItemOtherCost(GetUpdateItemInput input)
        {

            var entity = await _updateItemRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
&& p.UpdateItemType == UpdateItemType.OtherCostItem2List
&& p.GradientId == input.GradientId
&& p.SolutionId == input.SolutionId
&& p.Year == input.Year
&& p.UpDown == input.UpDown);
            if (entity is null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<List<OtherCostItem2List>>(entity.MaterialJson);

        }

        /// <summary>
        /// 获取其他成本项目2（核价表用）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal async virtual Task<List<OtherCostItem2>> GetOtherCostItem2(GetOtherCostItem2Input input)
        {
            var solution = await _solutionRepository.GetAsync(input.SolutionId);
            var gradientModel = await _gradientModelRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.GradientId == input.GradientId && p.ProductId == solution.Productld);


            //获取总年数
            var yearCount = await _gradientModelYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.GradientModelId == gradientModel.Id)
                    .OrderBy(p => p.Year).Select(p => new { p.Year, Quantity = p.Count, p.UpDown }).ToListAsync();


            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {

                var data = await yearCount.SelectAsync(async p => await GetData(p.Year, p.UpDown, p.Quantity));
                var dto = data.SelectMany(p => p).GroupBy(p => p.ItemName).Select(p => new OtherCostItem2
                {
                    Year = 0,
                    UpDown = YearType.Year,
                    Quantity = 0,
                    ItemName = p.Key,
                    Total = p.Key == "单颗成本" ? 0 : null,
                    MoldCosts = p.Key == "单颗成本" ? p.Sum(o => o.MoldCosts * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    FixtureCost = p.Key == "单颗成本" ? p.Sum(o => o.FixtureCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    ToolCost = p.Key == "单颗成本" ? p.Sum(o => o.ToolCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    InspectionCost = p.Key == "单颗成本" ? p.Sum(o => o.InspectionCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    ExperimentCost = p.Key == "单颗成本" ? p.Sum(o => o.ExperimentCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    SpecializedEquipmentCost = p.Key == "单颗成本" ? p.Sum(o => o.SpecializedEquipmentCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    TestSoftwareCost = p.Key == "单颗成本" ? p.Sum(o => o.TestSoftwareCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    OtherExpensesCost = p.Key == "单颗成本" ? p.Sum(o => o.OtherExpensesCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    TravelCost = p.Key == "单颗成本" ? p.Sum(o => o.TravelCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    SluggishCost = p.Key == "单颗成本" ? p.Sum(o => o.SluggishCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    RetentionCost = p.Key == "单颗成本" ? p.Sum(o => o.RetentionCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    LineCost = p.Key == "单颗成本" ? p.Sum(o => o.LineCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                    OtherCost = p.Key == "单颗成本" ? p.Sum(o => o.OtherCost * o.Quantity) / p.Sum(o => o.Quantity) : null,
                });
                var dk = dto.FirstOrDefault(p => p.ItemName == "单颗成本");
                dk.Total = dk.MoldCosts + dk.FixtureCost + dk.ToolCost + dk.InspectionCost + dk.ExperimentCost + dk.SpecializedEquipmentCost + dk.TestSoftwareCost + dk.OtherExpensesCost + dk.TravelCost + dk.SluggishCost
                    + dk.RetentionCost + dk.LineCost + dk.OtherCost;
                return dto.ToList();
            }
            else
            {
                var quantity = yearCount.FirstOrDefault(p => p.Year == input.Year && p.UpDown == input.UpDown);
                return await GetData(input.Year, input.UpDown, quantity == null ? 0 : quantity.Quantity);
            }

            async Task<List<OtherCostItem2>> GetData(int year, YearType upDown, decimal quantity)
            {
                //修改项
                var getUpdateItemOtherCosts = await GetUpdateItemOtherCost(new GetUpdateItemInput
                {
                    AuditFlowId = input.AuditFlowId,
                    GradientId = input.GradientId,
                    SolutionId = input.SolutionId,
                    Year = year,
                    UpDown = upDown
                });

                //原版
                var getOtherCostItem2List = await GetOtherCostItem2List(new GetOtherCostItem2ListInput
                {
                    AuditFlowId = input.AuditFlowId,
                    GradientId = input.GradientId,
                    SolutionId = input.SolutionId,
                });

                foreach (var item in getOtherCostItem2List)
                {
                    var getUpdateItemOtherCost = getUpdateItemOtherCosts?.FirstOrDefault(p => p.ItemName == item.ItemName);
                    if (getUpdateItemOtherCost is not null)
                    {
                        item.Total = getUpdateItemOtherCost.Total;
                        item.Count = getUpdateItemOtherCost.Count;
                        item.Cost = getUpdateItemOtherCost.Cost;
                        item.Note = getUpdateItemOtherCost.Note;
                        item.IsShare = getUpdateItemOtherCost.IsShare;
                    }
                }

                //计算单颗成本
                getOtherCostItem2List.ForEach(item =>
                {
                    item.Count = item.IsShare ? item.Count : decimal.Zero;
                    item.Cost = item.Count == decimal.Zero ? decimal.Zero : item.Total / item.Count;
                });

                //var data = await _nrePricingAppService.GetPricingFormDownload(input.AuditFlowId, input.SolutionId);

                //var dto = ObjectMapper.Map<ExcelPricingFormDto>(data);
                var otherCostItem2 = new List<OtherCostItem2> {
                new OtherCostItem2
                {
                    ItemName = "总费用",
                    Total = getOtherCostItem2List.Sum(p=>p.Total),
                    MoldCosts = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==MoldCosts).Total,
                    FixtureCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==FixtureCost).Total,
                    ToolCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==ToolCost).Total,
                    InspectionCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==InspectionCost).Total,
                    ExperimentCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==ExperimentCost).Total,
                    SpecializedEquipmentCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==SpecializedEquipmentCost).Total,
                    TestSoftwareCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==TestSoftwareCost).Total,
                    OtherExpensesCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==OtherExpensesCost).Total,
                    TravelCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==TravelCost).Total,
                    SluggishCost =getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==SluggishCost).Total,
                    RetentionCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==RetentionCost).Total,
                    LineCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==LineCost).Total,
                    OtherCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==OtherCost).Total,
                } ,
                new OtherCostItem2
                {
                    ItemName = "分摊数量",
                    //Total = 0,
                    MoldCosts = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==MoldCosts).Count,
                    FixtureCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==FixtureCost).Count,
                    ToolCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==ToolCost).Count,
                    InspectionCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==InspectionCost).Count,
                    ExperimentCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==ExperimentCost).Count,
                    SpecializedEquipmentCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==SpecializedEquipmentCost).Count,
                    TestSoftwareCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==TestSoftwareCost).Count,
                    OtherExpensesCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==OtherExpensesCost).Count,
                    TravelCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==TravelCost).Count,
                    SluggishCost =getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==SluggishCost).Count,
                    RetentionCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==RetentionCost).Count,
                    LineCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==LineCost).Count,
                    OtherCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==OtherCost).Count,
                } ,
                new OtherCostItem2
                {
                    ItemName = "单颗成本",
                    Total = getOtherCostItem2List.Sum(p=>p.Cost),
                    MoldCosts = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==MoldCosts).Cost,
                    FixtureCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==FixtureCost).Cost,
                    ToolCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==ToolCost).Cost,
                    InspectionCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==InspectionCost).Cost,
                    ExperimentCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==ExperimentCost).Cost,
                    SpecializedEquipmentCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==SpecializedEquipmentCost).Cost,
                    TestSoftwareCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==TestSoftwareCost).Cost,
                    OtherExpensesCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==OtherExpensesCost).Cost,
                    TravelCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==TravelCost).Cost,
                    SluggishCost =getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==SluggishCost).Cost,
                    RetentionCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==RetentionCost).Cost,
                    LineCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==LineCost).Cost,
                    OtherCost = getOtherCostItem2List.FirstOrDefault(p=>p.ItemName==OtherCost).Cost,
                } ,
            };
                var ft = otherCostItem2.FirstOrDefault(p => p.ItemName == "分摊数量");

                var otherCostSum = getOtherCostItem2List.Sum(p => p.Cost);
                ft.Total = otherCostSum == 0 ? 0 : getOtherCostItem2List.Sum(p => p.Total) / otherCostSum;

                otherCostItem2.ForEach(p =>
                {
                    p.Year = year;
                    p.UpDown = upDown;
                    p.Quantity = quantity;
                });

                return otherCostItem2;
            }

        }

        public const string MoldCosts = "模具费分摊";
        public const string FixtureCost = "治具费分摊";
        public const string ToolCost = "工装费分摊";
        public const string InspectionCost = "检具费用分摊";
        public const string ExperimentCost = "实验费分摊";
        public const string SpecializedEquipmentCost = "专用设备分摊";
        public const string TestSoftwareCost = "测试软件费分摊";
        public const string OtherExpensesCost = "其他费用分摊";
        public const string TravelCost = "差旅费分摊";
        public const string SluggishCost = "呆滞物料分摊";
        public const string RetentionCost = "质保金分摊";
        public const string LineCost = "线体成本分摊";

        public const string NreCost = "NRE费用分摊";
        public const string AfterSalesPartsCost = "售后件费用分摊";


        public const string OtherCost = "其他成本";

        public const string NreFy = "NRE费用";


        public const string Shjfy = "售后件费用";

        /// <summary>
        /// 当年年份是否用其他成本
        /// </summary>
        /// <param name="year"></param>
        /// <param name="upDown"></param>
        /// <param name="sop"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual bool IsHasCostFunc(long year, YearType upDown, int sop, decimal count)
        {
            //截止年份，年份大于等于此立刻截至
            var jrnf = sop + count;

            if (upDown == YearType.Year)
            {
                return year < jrnf;
            }
            else
            {
                //距离截至剩余的年数
                var syqx = jrnf - year;
                if (syqx >= 1)
                {
                    return true;
                }
                else if (syqx < 1 && syqx > 0)
                {
                    return upDown == YearType.FirstHalf;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// 其他成本项目2（核价看板用）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<OtherCostItem2List>> GetOtherCostItem2List(GetOtherCostItem2ListInput input)
        {
            var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);
            var solution = await _solutionRepository.GetAsync(input.SolutionId);
            var shareCount = await _shareCountRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld);
            var data = await _nrePricingAppService.GetPricingFormDownload(input.AuditFlowId, input.SolutionId);

            var modelCountYears = await _modelCountYearRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId);
            var sopYear = modelCountYears.MinBy(p => p.Year);

            //分摊年数
            var yearCount = shareCount is null ? 0 : shareCount.Year;

            //(sopYear + yearCount)表示分摊截止年份，input.Year减去它，如果不为负数，表示当前年份可分摊
            var isHasCost = IsHasCostFunc(input.Year, input.UpDown, sopYear.Year, yearCount);//input.Year - (sopYear.Year + yearCount) < 0;



            var otherCostItem2List = new List<OtherCostItem2List>
            {
                new OtherCostItem2List
                {
                    CostType = NreFy,
                    ItemName = MoldCosts,
                    Total = data.MouldInventoryTotal,
                    Note = string.Empty,
                    IsShare = priceEvaluation.AllocationOfMouldCost
                },
                new OtherCostItem2List
                {
                    CostType = NreFy,
                    ItemName = FixtureCost,
                    Total = data.FixtureCostTotal,
                    Note = string.Empty,
                    IsShare = priceEvaluation.AllocationOfFixtureCost
                },
                new OtherCostItem2List
                {
                    CostType = NreFy,
                    ItemName = ToolCost,
                    Total = data.ToolingCostTotal,
                    Note = string.Empty,
                    IsShare = priceEvaluation.FrockCost
                },
                new OtherCostItem2List
                {
                    CostType = NreFy,
                    ItemName = InspectionCost,
                    Total = data.QAQCDepartmentsTotal,
                    Note = string.Empty,
                    IsShare = priceEvaluation.FixtureCost
                },
                new OtherCostItem2List
                {
                    CostType = NreFy,
                    ItemName = SpecializedEquipmentCost,
                    Total = data.ProductionEquipmentCost.Where(p=>p.DeviceStatusName=="专用").Sum(p=>p.Cost),//data.ProductionEquipmentCostTotal,
                    Note = string.Empty,
                    IsShare = priceEvaluation.AllocationOfEquipmentCost
                },
                new OtherCostItem2List
                {
                    CostType = NreFy,
                    ItemName = ExperimentCost,
                    Total = data.LaboratoryFeeModelsTotal,
                    Note = string.Empty,
                    IsShare = priceEvaluation.ExperimentCost
                },
                new OtherCostItem2List
                {
                    CostType = NreFy,
                    ItemName = TestSoftwareCost,
                    Total = data.SoftwareTestingCostTotal,
                    Note = string.Empty,
                    IsShare = priceEvaluation.TestCost
                },
                 new OtherCostItem2List
                {
                    CostType = NreFy,
                    ItemName =OtherExpensesCost,
                    Total = data.RestsCostTotal,
                    Note = string.Empty,
                    IsShare = priceEvaluation.OtherCost
                },
                  new OtherCostItem2List
                {
                    CostType = NreFy,
                    ItemName = TravelCost,
                    Total = data.TravelExpenseTotal,
                    Note = string.Empty,
                    IsShare = priceEvaluation.TravelCost
                },

                new OtherCostItem2List
                {
                    CostType = Shjfy,
                    ItemName =SluggishCost,
                    Total = decimal.Zero,
                    Note = string.Empty,
                    IsShare = false,
                },
                 new OtherCostItem2List
                {
                    CostType = Shjfy,
                    ItemName = RetentionCost,
                    Total = decimal.Zero,
                    Note = string.Empty,
                    IsShare = false,
                },
                 new OtherCostItem2List
                {
                    CostType = Shjfy,
                    ItemName = LineCost,
                    Total =decimal.Zero,// data.ProductionEquipmentCostTotal,
                    Note = string.Empty,
                    IsShare = false,
                },

            };

            if (input.Year == PriceEvalConsts.AllYear)
            {
                var dto = await modelCountYears.SelectAsync(async p => await GetOtherCostItem2List(new GetOtherCostItem2ListInput
                {
                    AuditFlowId = input.AuditFlowId,
                    GradientId = input.GradientId,
                    SolutionId = input.SolutionId,
                    Year = p.Year,
                    UpDown = p.UpDown
                }));

                var ff = dto.SelectMany(p => p).GroupBy(p => p.ItemName).Select(item => new OtherCostItem2List
                {
                    ItemName = item.Key,
                    Cost = item.Sum(p => p.Cost * modelCountYears.First(o => o.Year == p.Year).Quantity),
                });

                otherCostItem2List.ForEach(item =>
                {
                    item.Count = shareCount is null ? 0 : shareCount.Count * 1000;
                    item.Cost = ff.First(p => p.ItemName == item.ItemName).Cost; //item.Count == decimal.Zero ? decimal.Zero : item.Total / item.Count;
                    item.YearCount = yearCount;
                });
            }
            else
            {
                otherCostItem2List.ForEach(item =>
                {
                    item.Count = !isHasCost ? 0 : shareCount is null ? 0 : shareCount.Count * 1000;
                    item.Cost = !isHasCost ? 0 : item.Count == decimal.Zero ? decimal.Zero : item.Total / item.Count;
                    item.YearCount = yearCount;
                    item.Year = input.Year;
                    item.UpDown = input.UpDown;
                });
            }


            otherCostItem2List.Add(new OtherCostItem2List
            {
                CostType = "其他费用",
                ItemName = OtherCost,
                Total = decimal.Zero,
                Count = 0,
                Cost = 0,
                Note = string.Empty,
                IsShare = false,
                Year = input.Year,
                UpDown = input.UpDown
            });
            return otherCostItem2List;
        }

        #endregion

        #region 损耗成本

        /// <summary>
        /// 获取损耗成本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<LossCost>> GetLossCost(GetCostItemInput input)
        {
            //物料成本
            var electronicAndStructureList = await this.GetBomCost(new GetBomCostInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown });
            return await this.GetLossCostByMaterial(input.Year, electronicAndStructureList, input);
        }

        /// <summary>
        /// 根据物料 获取 损耗成本
        /// </summary>
        /// <param name="year"></param>
        /// <param name="electronicAndStructureList"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<List<LossCost>> GetLossCostByMaterial(int year, List<Material> electronicAndStructureList, GetCostItemInput input)
        {
            //计算-成本项目
            var costItem = electronicAndStructureList.GroupBy(x => x.SuperType)
                .Select(p => new LossCost { EditId = p.Key, Id = year, Name = p.Key, WastageCost = p.Sum(o => o.Loss), MoqShareCount = p.Sum(o => o.MoqShareCount) }).ToList();//只有损耗，还要增加分摊


            //取得修改项
            var updateItem = await _updateItemRepository
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                && p.UpdateItemType == UpdateItemType.LossCost && p.GradientId == input.GradientId
                && p.SolutionId == input.SolutionId
                && p.Year == input.Year && p.UpDown == input.UpDown);

            var lossCost = ObjectMapper.Map<SetUpdateItemInput<List<LossCost>>>(updateItem);
            if (lossCost is not null)
            {
                var dataIds = lossCost.UpdateItem.Select(p => p.EditId);

                foreach (var item in costItem.Where(p => dataIds.Contains(p.EditId)))
                {
                    ObjectMapper.Map(lossCost.UpdateItem.FirstOrDefault(p => p.EditId == item.EditId), item);
                }
            }

            return costItem;
        }

        /// <summary>
        /// 获取全生命周期 损耗成本 根据多个年份的损耗成本计算
        /// </summary>
        /// <param name="costItemList"></param>
        /// <returns></returns>
        private List<LossCost> GetCostItemAllPrivate(List<LossCost> costItemList)
        {
            var costItem = costItemList.GroupBy(p => p.Id).Select(p => new LossCost
            {
                Id = p.Key,
                Name = p.FirstOrDefault().Name,
                WastageCost = p.Sum(o => o.WastageCost),
                MoqShareCount = p.Sum(o => o.MoqShareCount),
            }).ToList();

            return costItem;
        }

        #endregion

        #region BOM成本

        /// <summary>
        /// 获取 bom成本（含损耗）汇总表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<Material>> GetBomCost(GetBomCostInput input)
        {
            var solution = await _solutionRepository.GetAsync(input.SolutionId);
            var productId = solution.Productld;

            var customerTargetPrice = await _customerTargetPriceRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == productId);


            var sopYear = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == productId).MinAsync(p => p.Year);
            var data = await GetAllData(input);

            //取得修改项
            var updateItem = await _updateItemRepository
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                && p.UpdateItemType == UpdateItemType.Material && p.GradientId == input.GradientId
                && p.SolutionId == input.SolutionId
                && p.Year == input.Year && p.UpDown == input.UpDown);
            var material = ObjectMapper.Map<SetUpdateItemInput<List<Material>>>(updateItem);
            if (material is not null)
            {
                var dataIds = material.UpdateItem.Select(p => p.Id);

                foreach (var item in data.Where(p => dataIds.Contains(p.Id)))
                {
                    //item = material.Material.FirstOrDefault(p => p.Id == item.Id);
                    ObjectMapper.Map(material.UpdateItem.FirstOrDefault(p => p.Id == item.Id), item);
                }
            }

            data.ForEach(p => p.IsCustomerSupplyStr = p.IsCustomerSupply ? "是" : "否");

            return data.OrderBy(p => p.SuperType.BomSort()).ThenByDescending(p => p.TotalMoneyCyn).ToList();
            async Task<List<Material>> GetAllData(GetBomCostInput input)
            {
                var gradient = await _gradientRepository.GetAsync(input.GradientId);
                var gradientModel = await _gradientModelRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.GradientId == input.GradientId && p.ProductId == productId);



                //全生命周期处理
                if (input.Year == PriceEvalConsts.AllYear)
                {
                    //获取总年数
                    //var yearCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == productId)
                    //    .OrderBy(p => p.Year).Select(p => new { p.Year, p.UpDown, p.Quantity }).ToListAsync();
                    var yearCount = await _gradientModelYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.GradientModelId == gradientModel.Id)
                        .OrderBy(p => p.Year).Select(p => new { p.Year, Quantity = p.Count, p.UpDown }).ToListAsync();

                    //获取数据
                    var material = await yearCount.SelectAsync(async p => await GetData(p.Year, p.UpDown));


                    //加总求平均以获取Dto
                    var dto = material.SelectMany(p => p).Join(yearCount, p => p.Year, p => p.Year, (a, b) => { a.Quantity = b.Quantity; return a; })
                        .GroupBy(p => new
                        {
                            p.Id,
                            p.SuperType,
                            p.CategoryName,
                            p.TypeName,
                            p.Sap,
                            p.MaterialName,
                            p.AssemblyCount,
                            p.CurrencyText,
                            //p.MoqShareCount,
                            p.Moq,
                            p.AvailableInventory,
                            p.Remarks,
                        }).Select(p => new Material
                        {
                            Id = p.Key.Id,
                            SuperType = p.Key.SuperType,
                            CategoryName = p.Key.CategoryName,
                            TypeName = p.Key.TypeName,
                            Sap = p.Key.Sap,
                            MaterialName = p.Key.MaterialName,
                            AssemblyCount = p.Key.AssemblyCount,
                            CurrencyText = p.Key.CurrencyText,
                            ExchangeRate = p.MinBy(o => o.Year).ExchangeRate,
                            SopExchangeRate = p.MinBy(o => o.Year).ExchangeRate,
                            Loss = p.Key.AssemblyCount == 0 ? 0 : p.Sum(o => o.Loss * o.Quantity) / p.Sum(o => p.Key.AssemblyCount.To<decimal>() * o.Quantity) * p.Key.AssemblyCount.To<decimal>(),
                            InputCount = p.Sum(o => o.InputCount),
                            PurchaseCount = p.Sum(o => o.PurchaseCount),
                            //MoqShareCount = p.Key.MoqShareCount,
                            MoqShareCount = p.MinBy(p => p.Year).MoqShareCount,
                            Moq = p.Key.Moq,
                            AvailableInventory = p.Key.AvailableInventory,
                            Remarks = p.Key.Remarks,
                            MaterialPriceCyn = p.Key.AssemblyCount == 0 ? 0 : p.Sum(o => o.MaterialPrice * o.ExchangeRate * o.AssemblyCount.To<decimal>() * o.Quantity) / p.Sum(o => p.Key.AssemblyCount.To<decimal>() * o.Quantity), //* p.Key.AssemblyCount.To<decimal>(),
                            ModificationComments = p.FirstOrDefault()?.ModificationComments
                        }).ToList();
                    foreach (var item in dto)
                    {
                        item.TotalMoneyCyn = item.MaterialPriceCyn * item.AssemblyCount.To<decimal>();
                        item.MaterialPrice = item.MaterialPriceCyn / item.SopExchangeRate;
                        item.LossRate = item.TotalMoneyCyn == 0 ? 0 : item.Loss / item.TotalMoneyCyn;
                        item.MaterialCost = item.TotalMoneyCyn + item.Loss;
                    }
                    return dto.GroupBy(p => p.SuperType).Select(p => p.Select(o => o).OrderByDescending(o => o.TotalMoneyCyn).ToList())
                        .SelectMany(p => p).ToList();
                }
                else
                {
                    return await GetData(input.Year, input.UpDown);
                }

                async Task<List<Material>> GetData(int year, YearType upDown)
                {
                    var gradientModelYear = await (from gm in _gradientModelRepository.GetAll()
                                                   join gmy in _gradientModelYearRepository.GetAll() on gm.Id equals gmy.GradientModelId
                                                   where gm.AuditFlowId == input.AuditFlowId && gm.GradientId == input.GradientId && gm.ProductId == productId && gmy.Year == year && gmy.UpDown == upDown
                                                   select gmy).FirstOrDefaultAsync();

                    var lossYear = year - sopYear;

                    //产品大类名
                    var productTypeName = await (from m in _modelCountRepository.GetAll()
                                                 join fd in _financeDictionaryDetailRepository.GetAll() on m.ProductType equals fd.Id
                                                 where m.Id == productId
                                                 select fd.DisplayName).FirstOrDefaultAsync();
                    //产品大类是否存在于损耗率中
                    var productTypeNameIsHasFromLossRate = await _lossRateInfoRepository.GetAll().AnyAsync(p => p.SuperType == productTypeName);

                    //筛选用的产品大类
                    var filterProductTypeName = productTypeNameIsHasFromLossRate ? productTypeName : FinanceConsts.ProductTypeNameOther;

                    //获取【电子料】表
                    var electronic = from eb in _electronicBomInfoRepository.GetAll()
                                     join ec in _enteringElectronicRepository.GetAll() on eb.Id equals ec.ElectronicId

                                     join lri in _lossRateInfoRepository.GetAll()
                                     //on new { eb.TypeName, eb.CategoryName } equals new { TypeName = lri.CategoryName, CategoryName = lri.MaterialCategory }
                                     on eb.TypeName equals lri.CategoryName


                                     join lriy in _lossRateYearInfoRepository.GetAll() on lri.Id equals lriy.LossRateInfoId

                                     join er in _exchangeRateRepository.GetAll() on ec.Currency equals er.ExchangeRateKind

                                     //join s in _solutionRepository.GetAll() on eb.SolutionId equals s.Id
                                     //join m in _modelCountRepository.GetAll() on s.Productld equals m.Id
                                     //join fd in _financeDictionaryDetailRepository.GetAll() on m.ProductType equals fd.Id
                                     where
                                     lri.SuperType == filterProductTypeName &&
                                     eb.AuditFlowId == input.AuditFlowId && lriy.Year == lossYear
                                     && eb.SolutionId == input.SolutionId && ec.SolutionId == input.SolutionId

                                     select new Material
                                     {
                                         Id = $"{PriceEvaluationGetAppService.ElectronicBomName}{eb.Id}",
                                         SuperType = FinanceConsts.ElectronicName,//lri.SuperType,
                                         CategoryName = eb.CategoryName,
                                         TypeName = eb.TypeName,
                                         Sap = eb.SapItemNum,
                                         MaterialName = eb.SapItemName,
                                         AssemblyCount = eb.AssemblyQuantity,//装配数量
                                         SystemiginalCurrency = ec.SystemiginalCurrency,//ec.SystemiginalCurrency,
                                         CurrencyText = ec.Currency,
                                         ExchangeRateValue = er.ExchangeRateValue,
                                         LossRate = lriy.Rate,//损耗率
                                         Moq = ec.MOQ,
                                         //AvailableInventory = ec.AvailableStock,
                                         StandardMoney = ec.StandardMoney,
                                         Remarks = ec.Remark,
                                         ModificationComments = ec.ModificationComments,
                                     };
                    var electronicList = await electronic.ToListAsync();



                    //获取【结构料】表（其他大类都在这）

                    var structure = from sb in _structureBomInfoRepository.GetAll()
                                    join se in _structureElectronicRepository.GetAll() on sb.Id equals se.StructureId

                                    //join lri in _lossRateInfoRepository.GetAll() on sb.CategoryName equals lri.MaterialCategory
                                    join lri in _lossRateInfoRepository.GetAll()
                                    on new { sb.TypeName, sb.CategoryName } equals new { TypeName = lri.CategoryName, CategoryName = lri.MaterialCategory }
                                    //on sb.TypeName equals lri.CategoryName

                                    join lriy in _lossRateYearInfoRepository.GetAll() on lri.Id equals lriy.LossRateInfoId

                                    join er in _exchangeRateRepository.GetAll() on se.Currency equals er.ExchangeRateKind

                                    //join s in _solutionRepository.GetAll() on sb.SolutionId equals s.Id
                                    //join m in _modelCountRepository.GetAll() on s.Productld equals m.Id
                                    //join fd in _financeDictionaryDetailRepository.GetAll() on m.ProductType equals fd.Id

                                    where
                                    lri.SuperType == filterProductTypeName &&
                                    sb.TypeName == lri.CategoryName &&
                                    sb.AuditFlowId == input.AuditFlowId && lriy.Year == lossYear
                                     && sb.SolutionId == input.SolutionId && se.SolutionId == input.SolutionId

                                    select new Material
                                    {
                                        Id = $"{PriceEvaluationGetAppService.StructureBomName}{sb.Id}",
                                        SuperType = sb.SuperTypeName,// lri.SuperType,
                                        CategoryName = sb.CategoryName,
                                        TypeName = sb.TypeName,
                                        Sap = sb.SapItemNum,
                                        MaterialName = sb.DrawingNumName,//改成图号名称  //sb.MaterialName,
                                        AssemblyCount = sb.AssemblyQuantity,//装配数量
                                        SystemiginalCurrency = se.SystemiginalCurrency,//se.Sop,
                                        CurrencyText = se.Currency,
                                        ExchangeRateValue = er.ExchangeRateValue,
                                        ExchangeRateId = er.Id,
                                        LossRate = lriy.Rate,//损耗率
                                        Moq = se.MOQ,
                                        //AvailableInventory = se.AvailableStock,
                                        StandardMoney = se.StandardMoney,
                                        Remarks = se.Remark,
                                        ModificationComments = se.ModificationComments,
                                    };
                    var structureList = await structure.ToListAsync();

                    var electronicAndStructureList = electronicList.Union(structureList).ToList();


                    //是否客供
                    var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);
                    var bomIsCustomerSupply = priceEvaluation.BomIsCustomerSupplyJson.IsNullOrWhiteSpace() ? null : priceEvaluation.BomIsCustomerSupplyJson.FromJsonString<List<BomIsCustomerSupply>>();


                    var moqFormulas = MiniExcel.Query<MoqFormulaDto>("wwwroot/Excel/MoqFormula.xlsx").ToList();


                    electronicAndStructureList.ForEach(item =>
                    {
                        item.Year = year;
                        item.MaterialPrice = GetMaterialPrice(item.SystemiginalCurrency, year, upDown, gradient.GradientValue);
                        item.ExchangeRate = customerTargetPrice is not null && customerTargetPrice.Currency is not 0 && customerTargetPrice.Id == item.ExchangeRateId ? customerTargetPrice.ExchangeRate : GetExchangeRate(item.ExchangeRateValue, year);//二开：如果营销部录入有汇率，就取录入
                        item.MaterialPriceCyn = GetYearValue(item.StandardMoney, year, upDown, gradient.GradientValue);//二开：材料单价原币*汇率
                        item.TotalMoneyCyn = (decimal)item.AssemblyCount * item.MaterialPriceCyn;//人民币合计金额=装配数量*人民币单价（诸年之和）二开：也可以直接取本位币
                        item.Loss = item.LossRate / 100 * item.TotalMoneyCyn;//等于合计金额*损耗率
                        item.MaterialCost = item.TotalMoneyCyn + item.Loss;//材料成本（含损耗）
                        //item.InputCount = Math.Round((decimal)item.AssemblyCount * (1 + item.LossRate) * input.InputCount, 0).To<int>();//（装配数量*（1+损耗率）*投入量） ，四舍五入，取整
                        item.InputCount = (electronicAndStructureList.Count(p => p.Sap == item.Sap) * gradientModelYear.Count * 1000) / (1 - (item.LossRate / 100));

                        item.PurchaseCount = item.AvailableInventory > item.InputCount ? 0 : ((item.InputCount - item.AvailableInventory) > item.Moq ? (item.Moq == 0 ? 0 : Formula(item)) : item.Moq);
                        item.MoqShareCount = (item.Moq == 0 || item.InputCount == 0) ? 0 : ((item.PurchaseCount - item.InputCount) < 0 ? 0 : (item.PurchaseCount - item.InputCount) * item.MaterialPriceCyn / item.InputCount);

                        item.IsCustomerSupply = bomIsCustomerSupply == null ? false : bomIsCustomerSupply.FirstOrDefault(p => p.Id == item.Id).IsCustomerSupply;
                        item.TotalMoneyCynNoCustomerSupply = item.IsCustomerSupply ? 0 : item.TotalMoneyCyn;


                    });

                    decimal Formula(Material item)
                    {
                        var row = moqFormulas.FirstOrDefault(p => (p.TypeName == MoqFormulaDto.Pcb && p.CategoryName == item.CategoryName) || (p.TypeName == item.TypeName && p.CategoryName == item.CategoryName));
                        if (row is null || row.Formula == MoqFormulaDto.Formula1)
                        {
                            return (item.Moq * Math.Ceiling((item.InputCount - item.AvailableInventory) / item.Moq));
                        }
                        else
                        {
                            return item.InputCount;
                        }
                    }

                    return electronicAndStructureList.GroupBy(p => p.SuperType).Select(p => p.Select(o => o).OrderByDescending(o => o.TotalMoneyCyn).ToList())
                        .SelectMany(p => p).ToList();
                }
            }

        }

        /// <summary>
        /// 获取 bom成本（含损耗）汇总表 Dto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<BomCost> GetBomCostDto(GetBomCostInput input)
        {
            var data = await GetBomCost(input);
            var dto = new BomCost();
            dto.Material = data;
            dto.TotalMoneyCynCount = dto.Material.Sum(p => p.TotalMoneyCynNoCustomerSupply);
            dto.ElectronicCount = dto.Material.Where(p => p.SuperType == FinanceConsts.ElectronicName).Sum(p => p.TotalMoneyCyn);
            return dto;
        }

        #endregion

        #region 制造成本汇总表


        /// <summary>
        /// 获取 制造成本汇总表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<ManufacturingCost>> GetManufacturingCost(GetManufacturingCostInput input)
        {
            var solution = await _solutionRepository.GetAsync(input.SolutionId);
            var gradient = await _gradientRepository.GetAsync(input.GradientId);

            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {
                #region 组测

                //获取总年数
                var yearCount = await _modelCountYearRepository
                    .GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld);

                //获取数据
                //var dtoList = await yearCount.SelectAsync(async p => await GetGroupTest(p,input,gradient, solution.Productld));
                var dtoList = new List<ManufacturingCost>();
                yearCount.ForEach(p =>
                {
                    var dto = GetGroupTest(p.Year, p.UpDown, input, gradient, solution.Productld).Result;
                    if (dto != null)
                        dtoList.Add(dto);
                });
                //var dtoList = yearCount.Select(p => mfcs.ToList());
                //所有年份的直接制造成本
                var manufacturingCostDirectList = dtoList.Select(p => p.ManufacturingCostDirect);

                //所有年份的间接制造成本
                var manufacturingCostIndirectList = dtoList.Select(p => p.ManufacturingCostIndirect);

                //月需求量的和
                var manufacturingCostDirectListMonthlyDemand = dtoList.Sum(p => p.MonthlyDemand);

                var manufacturingCostDirect = new ManufacturingCostDirect
                {
                    Id = 0,
                    DirectLabor = manufacturingCostDirectList.Sum(p => p.DirectLaborNo) / manufacturingCostDirectListMonthlyDemand,
                    EquipmentDepreciation = manufacturingCostDirectList.Sum(p => p.EquipmentDepreciationNo) / manufacturingCostDirectListMonthlyDemand,
                    LineChangeCost = manufacturingCostDirectList.Sum(p => p.LineChangeCost * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                    ManufacturingExpenses = manufacturingCostDirectList.Sum(p => p.ManufacturingExpenses * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                };
                manufacturingCostDirect.Subtotal = manufacturingCostDirect.DirectLabor + manufacturingCostDirect.EquipmentDepreciation + manufacturingCostDirect.LineChangeCost + manufacturingCostDirect.ManufacturingExpenses;

                var manufacturingCostIndirect = new ManufacturingCostIndirect
                {
                    Id = 0,
                    DirectLabor = manufacturingCostIndirectList.Sum(p => p.DirectLabor * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                    ManufacturingExpenses = manufacturingCostIndirectList.Sum(p => p.ManufacturingExpenses * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                    EquipmentDepreciation = manufacturingCostIndirectList.Sum(p => p.EquipmentDepreciation * p.MonthlyDemand) / manufacturingCostDirectListMonthlyDemand,
                };
                manufacturingCostIndirect.Subtotal = manufacturingCostIndirect.DirectLabor + manufacturingCostIndirect.EquipmentDepreciation + manufacturingCostIndirect.ManufacturingExpenses;

                //manufacturingCostDirect.Round2();//保留2位小数
                //manufacturingCostIndirect.Round2();//保留2位小数
                var dto = new ManufacturingCost
                {
                    Id = input.Year,
                    CostType = CostType.GroupTest,
                    CostItem = PriceEvalConsts.GroupTest,
                    GradientKy = dtoList.Sum(p => p.GradientKy),
                    ManufacturingCostDirect = manufacturingCostDirect,
                    ManufacturingCostIndirect = manufacturingCostIndirect,
                    Subtotal = manufacturingCostDirect.Subtotal + manufacturingCostIndirect.Subtotal
                };
                //dto.Round2();//保留2位小数

                #endregion

                #region SMT和COB 其他

                //COB、其他
                List<ManufacturingCost> entity = await GetDbCost(input);
                entity.Add(dto);

                //SMT 暂时为0，待需求确定后补充
                ManufacturingCost allYearSmt = new();
                //取得修改项
                var updateItem = await _updateItemRepository
                    .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                    && p.UpdateItemType == UpdateItemType.ManufacturingCost && p.GradientId == input.GradientId
                    && p.SolutionId == input.SolutionId
                    && p.Year == input.Year);

                var smtManufacturingCostEdit = ObjectMapper.Map<SetUpdateItemInput<List<ManufacturingCost>>>(updateItem);
                if (smtManufacturingCostEdit is not null)
                {
                    var smtEdit = smtManufacturingCostEdit.UpdateItem.FirstOrDefault(p => p.EditId == CostType.SMT.ToString());
                    if (smtEdit is not null)
                    {
                        entity.Add(smtEdit);
                    }
                    else
                    {
                        allYearSmt = new ManufacturingCost
                        {
                            Id = input.Year,
                            CostType = CostType.SMT,
                            GradientKy = 0,
                            MonthlyDemand = 0,
                            ManufacturingCostDirect = new ManufacturingCostDirect
                            {
                                Id = input.Year,
                                DirectLabor = 0,
                                EquipmentDepreciation = 0,
                                LineChangeCost = 0,
                                ManufacturingExpenses = 0,
                                Subtotal = 0,
                            },
                            ManufacturingCostIndirect = new ManufacturingCostIndirect
                            {
                                Id = input.Year,
                                DirectLabor = 0,
                                EquipmentDepreciation = 0,
                                ManufacturingExpenses = 0,
                                Subtotal = 0
                            },
                            Subtotal = 0
                        };
                        entity.Add(allYearSmt);
                    }
                }


                //entity.Insert(2, dto);
                #endregion

                #region 合计
                var total = GetTotal(input.Year, entity);

                entity.Add(total);
                #endregion

                entity.ForEach(p =>
                {
                    p.EditId = p.CostType.ToString();
                    p.CostItem = p.CostType switch
                    {
                        CostType.GroupTest => PriceEvalConsts.GroupTest,
                        CostType.SMT => PriceEvalConsts.SMT,
                        CostType.COB => PriceEvalConsts.COB,
                        CostType.Total => PriceEvalConsts.Total,
                        CostType.Other => PriceEvalConsts.Other,
                        _ => throw new FriendlyException($"CostType输入参数不正确。参数为：{p.CostType}"),
                    };
                });

                return entity.OrderBy(p => p.CostTypeSort()).ToList();
            }
            else
            {
                var entity = await GetData(input, gradient, solution.Productld);


                entity.ForEach(p =>
                {
                    p.EditId = p.CostType.ToString();
                    p.CostItem = p.CostType switch
                    {
                        CostType.GroupTest => PriceEvalConsts.GroupTest,
                        CostType.SMT => PriceEvalConsts.SMT,
                        CostType.COB => PriceEvalConsts.COB,
                        CostType.Total => PriceEvalConsts.Total,
                        CostType.Other => PriceEvalConsts.Other,
                        _ => throw new FriendlyException($"CostType输入参数不正确。参数为：{p.CostType}"),
                    };
                });

                return entity.OrderBy(p => p.CostTypeSort()).ToList();
            }



        }
        async Task<List<ManufacturingCost>> GetData(GetManufacturingCostInput input, Gradient gradient, long sProductld)
        {
            #region 获取组测

            var dto = await GetGroupTest(input.Year, input.UpDown, input, gradient, sProductld);

            #endregion

            #region SMT和COB 其他
            List<ManufacturingCost> entity = await GetDbCost(input);
            //if (dto != null)
            //{

            //COB、其他
            entity.Add(dto);
            //}
            //entity.Insert(2, dto);

            //SMT 暂时为0，待需求确定后补充
            entity.Add(new ManufacturingCost
            {
                EditId = CostType.SMT.ToString(),
                Id = input.Year,
                CostType = CostType.SMT,
                GradientKy = 0,
                MonthlyDemand = 0,
                ManufacturingCostDirect = new ManufacturingCostDirect
                {
                    Id = input.Year,
                    DirectLabor = 0,
                    EquipmentDepreciation = 0,
                    LineChangeCost = 0,
                    ManufacturingExpenses = 0,
                    Subtotal = 0,
                },
                ManufacturingCostIndirect = new ManufacturingCostIndirect
                {
                    Id = input.Year,
                    DirectLabor = 0,
                    EquipmentDepreciation = 0,
                    ManufacturingExpenses = 0,
                    Subtotal = 0
                },
                Subtotal = 0
            });

            #endregion

            #region 合计
            var total = GetTotal(input.Year, entity);
            entity.Add(total);
            #endregion

            return entity;
        }

        #region 获取数据库中存储的制造成本

        async Task<List<ManufacturingCost>> GetDbCost(GetManufacturingCostInput inputDto)
        {
            var data = new List<ManufacturingCost>();

            if (inputDto.Year == PriceEvalConsts.AllYear)
            {



                var gradient = await _gradientRepository.FirstOrDefaultAsync(p => p.Id == inputDto.GradientId);
                var gradientValue = gradient.GradientValue.ToString();

                var cob = await (from b in _bomEnterRepository.GetAll()
                                 where b.AuditFlowId == inputDto.AuditFlowId && b.SolutionId == inputDto.SolutionId && b.ModelCountYearId == PriceEvalConsts.AllYear
                                 && b.Classification == gradientValue
                                 select new ManufacturingCost
                                 {
                                     Id = inputDto.Year,
                                     CostType = CostType.COB,
                                     GradientKy = 0,
                                     MonthlyDemand = 0,
                                     ManufacturingCostDirect = new ManufacturingCostDirect
                                     {
                                         Id = inputDto.Year,
                                         DirectLabor = b.DirectLaborPrice.GetValueOrDefault(),
                                         EquipmentDepreciation = b.DirectDepreciation.GetValueOrDefault(),
                                         LineChangeCost = b.DirectLineChangeCost.GetValueOrDefault(),
                                         ManufacturingExpenses = b.DirectManufacturingCosts.GetValueOrDefault(),
                                         Subtotal = b.DirectSummary.GetValueOrDefault(),
                                     },
                                     ManufacturingCostIndirect = new ManufacturingCostIndirect
                                     {
                                         Id = inputDto.Year,
                                         DirectLabor = b.IndirectLaborPrice.GetValueOrDefault(),
                                         EquipmentDepreciation = b.IndirectDepreciation.GetValueOrDefault(),
                                         ManufacturingExpenses = b.IndirectManufacturingCosts.GetValueOrDefault(),
                                         Subtotal = b.IndirectSummary.GetValueOrDefault()
                                     },
                                     Subtotal = b.TotalCost.GetValueOrDefault()
                                 }).FirstOrDefaultAsync();

                var other = await (from b in _bomEnterTotalRepository.GetAll()
                                   where b.AuditFlowId == inputDto.AuditFlowId && b.SolutionId == inputDto.SolutionId && b.ModelCountYearId == PriceEvalConsts.AllYear
                                     && b.Classification == gradientValue
                                   select new ManufacturingCost
                                   {
                                       Id = inputDto.Year,
                                       CostType = CostType.Other,
                                       GradientKy = 0,
                                       MonthlyDemand = 0,
                                       ManufacturingCostDirect = new ManufacturingCostDirect
                                       {
                                           Id = inputDto.Year,
                                           DirectLabor = 0,
                                           EquipmentDepreciation = 0,
                                           LineChangeCost = 0,
                                           ManufacturingExpenses = 0,
                                           Subtotal = 0,
                                       },
                                       ManufacturingCostIndirect = new ManufacturingCostIndirect
                                       {
                                           Id = inputDto.Year,
                                           DirectLabor = 0,
                                           EquipmentDepreciation = 0,
                                           ManufacturingExpenses = 0,
                                           Subtotal = 0
                                       },
                                       Subtotal = b.TotalCost.Value
                                   }).FirstOrDefaultAsync();

                if (cob is not null)
                {
                    data.Add(cob);

                }
                if (other is not null)
                {
                    data.Add(other);

                }
            }
            else
            {
                var gradient = await _gradientRepository.FirstOrDefaultAsync(p => p.Id == inputDto.GradientId);
                var gradientValue = gradient.GradientValue.ToString();

                var cob = await (from b in _bomEnterRepository.GetAll()
                                 join y in _modelCountYearRepository.GetAll() on b.ModelCountYearId equals y.Id
                                 where y.UpDown == inputDto.UpDown
                                 && b.AuditFlowId == inputDto.AuditFlowId && b.SolutionId == inputDto.SolutionId && y.Year == inputDto.Year
                                 && b.Classification == gradientValue
                                 select new ManufacturingCost
                                 {
                                     Id = inputDto.Year,
                                     CostType = CostType.COB,
                                     GradientKy = 0,
                                     MonthlyDemand = 0,
                                     ManufacturingCostDirect = new ManufacturingCostDirect
                                     {
                                         Id = inputDto.Year,
                                         DirectLabor = b.DirectLaborPrice.GetValueOrDefault(),
                                         EquipmentDepreciation = b.DirectDepreciation.GetValueOrDefault(),
                                         LineChangeCost = b.DirectLineChangeCost.GetValueOrDefault(),
                                         ManufacturingExpenses = b.DirectManufacturingCosts.GetValueOrDefault(),
                                         Subtotal = b.DirectSummary.GetValueOrDefault(),
                                     },
                                     ManufacturingCostIndirect = new ManufacturingCostIndirect
                                     {
                                         Id = inputDto.Year,
                                         DirectLabor = b.IndirectLaborPrice.GetValueOrDefault(),
                                         EquipmentDepreciation = b.IndirectDepreciation.GetValueOrDefault(),
                                         ManufacturingExpenses = b.IndirectManufacturingCosts.GetValueOrDefault(),
                                         Subtotal = b.IndirectSummary.GetValueOrDefault()
                                     },
                                     Subtotal = b.TotalCost.GetValueOrDefault()
                                 }).FirstOrDefaultAsync();

                var other = await (from b in _bomEnterTotalRepository.GetAll()
                                   join y in _modelCountYearRepository.GetAll() on b.ModelCountYearId equals y.Id
                                   where y.UpDown == inputDto.UpDown
                                        && b.AuditFlowId == inputDto.AuditFlowId && b.SolutionId == inputDto.SolutionId && y.Year == inputDto.Year
                                        && b.Classification == gradientValue
                                   select new ManufacturingCost
                                   {
                                       Id = inputDto.Year,
                                       CostType = CostType.Other,
                                       GradientKy = 0,
                                       MonthlyDemand = 0,
                                       ManufacturingCostDirect = new ManufacturingCostDirect
                                       {
                                           Id = inputDto.Year,
                                           DirectLabor = 0,
                                           EquipmentDepreciation = 0,
                                           LineChangeCost = 0,
                                           ManufacturingExpenses = 0,
                                           Subtotal = 0,
                                       },
                                       ManufacturingCostIndirect = new ManufacturingCostIndirect
                                       {
                                           Id = inputDto.Year,
                                           DirectLabor = 0,
                                           EquipmentDepreciation = 0,
                                           ManufacturingExpenses = 0,
                                           Subtotal = 0
                                       },
                                       Subtotal = b.TotalCost.Value
                                   }).FirstOrDefaultAsync();


                if (cob is not null)
                {
                    data.Add(cob);

                }
                if (other is not null)
                {
                    data.Add(other);

                }

            }

            data.ForEach(p =>
            {
                p.EditId = p.CostType.ToString();
                p.CostItem = p.CostType switch
                {
                    CostType.GroupTest => PriceEvalConsts.GroupTest,
                    CostType.SMT => PriceEvalConsts.SMT,
                    CostType.COB => PriceEvalConsts.COB,
                    CostType.Total => PriceEvalConsts.Total,
                    CostType.Other => PriceEvalConsts.Other,
                    _ => throw new FriendlyException($"CostType输入参数不正确。参数为：{p.CostType}"),
                };
            });


            //取得修改项
            var updateItem = await _updateItemRepository
                .FirstOrDefaultAsync(p => p.AuditFlowId == inputDto.AuditFlowId
                && p.UpdateItemType == UpdateItemType.ManufacturingCost && p.GradientId == inputDto.GradientId
                && p.SolutionId == inputDto.SolutionId
                && p.Year == inputDto.Year);

            var manufacturingCostEdit = ObjectMapper.Map<SetUpdateItemInput<List<ManufacturingCost>>>(updateItem);
            if (manufacturingCostEdit is not null)
            {
                var dataIds = manufacturingCostEdit.UpdateItem.Select(p => p.EditId);

                foreach (var item in data.Where(p => dataIds.Contains(p.EditId)))
                {
                    ObjectMapper.Map(manufacturingCostEdit.UpdateItem.FirstOrDefault(p => p.EditId == item.EditId), item);
                }
            }


            return data;
        }
        #endregion

        #region 组测

        async Task<ManufacturingCost> GetGroupTest(int year, YearType upDown, GetManufacturingCostInput input, Gradient gradient, long sProductld)
        {

            //取得修改项
            var updateItem = await _updateItemRepository
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                && p.UpdateItemType == UpdateItemType.ManufacturingCost && p.GradientId == input.GradientId
                && p.SolutionId == input.SolutionId
                && p.Year == year && p.UpDown == upDown);

            var manufacturingCostEdit = ObjectMapper.Map<SetUpdateItemInput<List<ManufacturingCost>>>(updateItem);
            if (manufacturingCostEdit is not null)
            {
                var zf = manufacturingCostEdit.UpdateItem.FirstOrDefault(p => p.EditId == PriceEvalConsts.Zc);

                if (zf is not null)
                {
                    return zf;
                }
            }

            //工序工时年份
            var yearInfo = await (from yi in _yearInfoRepository.GetAll()
                                  join e in _processHoursEnterRepository.GetAll() on yi.ProcessHoursEnterId equals e.Id
                                  join y in _modelCountYearRepository.GetAll() on yi.ModelCountYearId equals y.Id
                                  where y.AuditFlowId == input.AuditFlowId
                                  && e.SolutionId == input.SolutionId
                                  && y.Year == year && y.UpDown == upDown
                                  select yi).ToListAsync();



            //获取制造成本参数
            var manufacturingCostInfo = await _manufacturingCostInfoRepository.FirstOrDefaultAsync(p => p.Year == year);
            if (manufacturingCostInfo is null)
            {
                manufacturingCostInfo = await _manufacturingCostInfoRepository.GetAll().OrderByDescending(p => p.Year).FirstOrDefaultAsync();
            }

            var gradientModel = await _gradientModelRepository.FirstOrDefaultAsync(p => p.GradientId == input.GradientId && p.ProductId == sProductld);
            if (gradientModel == null)
                errMessage = "梯度模式数据未找到";
            //模组数量
            //var modelCountYear = await _modelCountYearRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId && p.Year == year);
            var gradientModelYear = await _gradientModelYearRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId
            && p.GradientModelId == gradientModel.Id && p.ProductId == sProductld && p.Year == year && p.UpDown == upDown);

            //计算连续乘积的委托
            Func<List<decimal>, decimal> product = p =>
            {
                var init = 1M;
                p.ForEach(o => init *= o);
                return init;
            };

            //要求
            var requirement = await _requirementRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).ToListAsync();

            //（1-累计降幅）
            var oneCumulativeDecline = product.Invoke(requirement.Select(p => 1 - (p.AnnualDeclineRate * 0.01M)).ToList());

            //年需求量(gradientModelYear的数量)

            //月需求量
            var monthlyDemand = Math.Ceiling(gradientModelYear.Sum(p => p.Count) * 1000 / (upDown == YearType.Year ? 12M : 6M)).To<int>();

            //UPH值（工序工时界面暂无，暂定120）
            //var uph = (await _processHoursEnterUphRepository.FirstOrDefaultAsync(p =>
            //p.AuditFlowId == input.AuditFlowId
            //&& p.SolutionId == input.SolutionId
            //&& p.Year)).Value;
            //(await _uphInfoRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld)).UPH;
            var uph = (await (from p in _processHoursEnterUphRepository.GetAll()
                              join m in _modelCountYearRepository.GetAll() on p.ModelCountYearId equals m.Id
                              where p.AuditFlowId == input.AuditFlowId && p.SolutionId == input.SolutionId
                              && m.Year == year && m.UpDown == upDown && p.Uph == "zcuph"
                              select p).FirstOrDefaultAsync()).Value;



            //每班日产能
            var dailyCapacityPerShift = uph * manufacturingCostInfo.WorkingHours.To<decimal>() * (manufacturingCostInfo.RateOfMobilization / 100);

            //工时工序静态字段表
            //var workingHoursInfo =await (from p in _processHoursEnterDeviceRepository.GetAll()
            //                       join e in _processHoursEnterRepository.GetAll() on p.ProcessHoursEnterId equals e.Id
            //                       where e.AuditFlowId == input.AuditFlowId && e.SolutionId == input.SolutionId
            //                       select p).ToListAsync();

            //await _workingHoursInfoRepository.GetAll()
            //.Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == sProductld).ToListAsync();

            var workingHoursInfo = await _processHoursEnterRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.SolutionId == input.SolutionId);


            //设备金额
            var equipmentMoney = workingHoursInfo.Sum(p => p.DeviceTotalPrice);

            //每班日产能 * 月工作天数 * 每日班次
            var ycn = (dailyCapacityPerShift.Value * (decimal)manufacturingCostInfo.MonthlyWorkingDays * (decimal)manufacturingCostInfo.DailyShift);

            //产线数量
            var lineCount = ycn == 0 ? 0 : Math.Ceiling(monthlyDemand / ycn);

            //产线设备月折旧（删除稼动率）
            //var monthlyDepreciation = (equipmentMoney - 0) * lineCount * manufacturingCostInfo.RateOfMobilization / ((decimal)manufacturingCostInfo.UsefulLifeOfFixedAssets * 12) / oneCumulativeDecline;
            var monthlyDepreciation = (equipmentMoney - 0) * lineCount * 0.95M / ((decimal)manufacturingCostInfo.UsefulLifeOfFixedAssets * 12) / oneCumulativeDecline;


            //月产能
            var monthlyCapacity = dailyCapacityPerShift * 26 * 2;

            //月产能 * 产线数量
            var monthlyCapacityAndlineCount = monthlyCapacity * lineCount;

            //产能利用率
            var capacityUtilization = (monthlyDemand == 0 || monthlyCapacityAndlineCount == 0) ? 0 : monthlyDemand / monthlyCapacityAndlineCount / (manufacturingCostInfo.CapacityUtilizationRate / 100);
            capacityUtilization = capacityUtilization > 1 ? 1 : capacityUtilization;


            //分摊后折旧
            var allocatedDepreciation = monthlyDepreciation * capacityUtilization;

            //制造工时
            //var manufacturingHours = (yearInfo.Sum(p => p.StandardLaborHours) + yearInfo.Sum(p => p.StandardMachineHours)).To<decimal>();
            var manufacturingHours = (yearInfo.Sum(p => p.LaborHour) + yearInfo.Sum(p => p.MachineHour)).To<decimal>();

            //人员单价
            var personPrice = manufacturingCostInfo.AverageWage / oneCumulativeDecline;

            //财务费率
            var rateEntryInfo = await _rateEntryInfoRepository.FirstOrDefaultAsync(p => p.Year == year);
            if (rateEntryInfo is null)
            {
                rateEntryInfo = await _rateEntryInfoRepository.GetAll().OrderByDescending(p => p.Year).FirstOrDefaultAsync();
            }

            //工时工序
            //var workingHoursInputInfo = await _yearInfoRepository
            //.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == sProductld && p.Year == year && p.Part == YearPart.SwitchLine);

            //切线工时
            var workingHoursInputInfo = await _followLineTangentRepository
            .FirstOrDefaultAsync(p => p.Year == year);



            //跟线工价
            //var linePrice = personPrice * manufacturingCostInfo.TraceLineOfPerson.To<decimal>() / (decimal)manufacturingCostInfo.MonthlyWorkingDays / manufacturingCostInfo.WorkingHours.To<decimal>() / 3600;
            var linePrice = personPrice * workingHoursInputInfo.PerFollowUpQuantity.To<decimal>() / (decimal)manufacturingCostInfo.MonthlyWorkingDays / manufacturingCostInfo.WorkingHours.To<decimal>() / 3600;

            //跟线成本
            var lineCost = linePrice * workingHoursInputInfo.LaborHour.To<decimal>();

            //切线成本
            var switchLineCost = lineCount == 0 ? 0 : workingHoursInputInfo.MachineHour.To<decimal>() * (allocatedDepreciation / lineCount / manufacturingCostInfo.MonthlyWorkingDays.To<decimal>() / (manufacturingCostInfo.WorkingHours.To<decimal>() * 2) / 3600);

            //换线成本
            var lineChangeCost = lineCost + switchLineCost;

            //直接人工=人员单价*人员数量*(月需求量/每班日产能)/月工作天数/(1-累积降幅）
            var directLaborNo = dailyCapacityPerShift.Value == 0 ? 0 : personPrice * (yearInfo.Sum(p => p.PersonnelNumber).To<decimal>() + 1)
                * Math.Ceiling(monthlyDemand / dailyCapacityPerShift.Value) / (decimal)manufacturingCostInfo.MonthlyWorkingDays
                / oneCumulativeDecline;

            //直接制造成本
            var manufacturingCost = new ManufacturingCostDirect
            {
                MonthlyDemand = monthlyDemand,
                Id = year,
                //直接人工=人员单价*人员数量*(月需求量/每班日产能)/月工作天数/(1-累积降幅）/月需求量（新增：除以月需求量）
                DirectLabor = monthlyDemand == 0 ? 0 : directLaborNo / monthlyDemand,
                DirectLaborNo = directLaborNo,
                EquipmentDepreciation = monthlyDemand == 0 ? 0 : allocatedDepreciation.Value / monthlyDemand,
                EquipmentDepreciationNo = allocatedDepreciation.Value,
                LineChangeCost = lineChangeCost.Value,
                ManufacturingExpenses = (rateEntryInfo.DirectManufacturingRate * manufacturingHours) / 3600,
            };
            manufacturingCost.Subtotal = manufacturingCost.DirectLabor + manufacturingCost.EquipmentDepreciation + manufacturingCost.LineChangeCost + manufacturingCost.ManufacturingExpenses;

            //间接制造成本
            var manufacturingCostIndirect = new ManufacturingCostIndirect
            {
                MonthlyDemand = monthlyDemand,
                Id = year,
                DirectLabor = (rateEntryInfo.IndirectLaborRate * yearInfo.Sum(p => p.LaborHour).To<decimal>()) / 3600,
                EquipmentDepreciation = (rateEntryInfo.IndirectDepreciationRate * yearInfo.Sum(p => p.MachineHour).To<decimal>()) / 3600,
                ManufacturingExpenses = ((rateEntryInfo.IndirectManufacturingRate * manufacturingHours) / 3600) + manufacturingCostInfo.ProcessCost,
            };
            manufacturingCostIndirect.Subtotal = manufacturingCostIndirect.DirectLabor + manufacturingCostIndirect.EquipmentDepreciation + manufacturingCostIndirect.ManufacturingExpenses;



            //manufacturingCost.Round2();//保留两位小数
            //manufacturingCostIndirect.Round2();//保留两位小数

            var dto = new ManufacturingCost
            {
                EditId = PriceEvalConsts.Zc,
                Id = year,
                CostType = CostType.GroupTest,
                CostItem = PriceEvalConsts.GroupTest,
                //GradientKy = gradientModelYear.Quantity,
                GradientKy = gradient.GradientValue,
                MonthlyDemand = monthlyDemand,
                ManufacturingCostDirect = manufacturingCost,
                ManufacturingCostIndirect = manufacturingCostIndirect,
                Subtotal = manufacturingCost.Subtotal + manufacturingCostIndirect.Subtotal
            };
            return dto;

            //dto.Round2();//保留两位小数
        }


        #endregion

        #region 合计
        static ManufacturingCost GetTotal(int year, List<ManufacturingCost> entity)
        {
            var manufacturingCostDirectTotal = entity.Select(p => p.ManufacturingCostDirect).Where(p => p != null);
            var manufacturingCostIndirectTotal = entity.Select(p => p.ManufacturingCostIndirect).Where(p => p != null);

            var ddfg = manufacturingCostDirectTotal.Count();

            var manufacturingCostDirectTotalDto = new ManufacturingCostDirect
            {
                Id = year,
                DirectLabor = manufacturingCostDirectTotal.Sum(p => p.DirectLabor),
                EquipmentDepreciation = manufacturingCostDirectTotal.Sum(p => p.EquipmentDepreciation),
                LineChangeCost = manufacturingCostDirectTotal.Sum(p => p.LineChangeCost),
                ManufacturingExpenses = manufacturingCostDirectTotal.Sum(p => p.ManufacturingExpenses),
                Subtotal = manufacturingCostDirectTotal.Sum(p => p.Subtotal),
            };
            var manufacturingCostIndirectTotalDto = new ManufacturingCostIndirect
            {
                Id = year,
                EquipmentDepreciation = manufacturingCostIndirectTotal.Sum(p => p.EquipmentDepreciation),
                DirectLabor = manufacturingCostIndirectTotal.Sum(p => p.DirectLabor),
                ManufacturingExpenses = manufacturingCostIndirectTotal.Sum(p => p.ManufacturingExpenses),
                Subtotal = manufacturingCostIndirectTotal.Sum(p => p.Subtotal),
            };
            //manufacturingCostDirectTotalDto.Round2();//保留2位小数
            //manufacturingCostIndirectTotalDto.Round2();//保留2位小数

            var total = new ManufacturingCost
            {
                Id = year,
                CostItem = PriceEvalConsts.Total,
                CostType = CostType.Total,
                GradientKy = entity.Sum(p => p.GradientKy),
                ManufacturingCostDirect = manufacturingCostDirectTotalDto,
                ManufacturingCostIndirect = manufacturingCostIndirectTotalDto,
                Subtotal = entity.Sum(p => p.Subtotal),
            };
            //total.Round2();//保留2位小数
            return total;
        }

        #endregion

        #endregion

        #region 物流成本汇总表

        /// <summary>
        /// 获取 物流成本汇总表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<ProductionControlInfoListDto>> GetLogisticsCost(GetLogisticsCostInput input)
        {
            var solution = await _solutionRepository.GetAsync(input.SolutionId);

            var gradient = await _gradientRepository.FirstOrDefaultAsync(p => p.Id == input.GradientId);
            var gradientValue = gradient.GradientValue.ToString();

            var year = await _modelCountYearRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld);



            if (input.Year == PriceEvalConsts.AllYear)
            {
                var gradientModelYear = await (from gm in _gradientModelRepository.GetAll()
                                               join gmy in _gradientModelYearRepository.GetAll() on gm.Id equals gmy.GradientModelId
                                               where gm.AuditFlowId == input.AuditFlowId && gm.GradientId == input.GradientId && gm.ProductId == solution.Productld
                                               select gmy).ToListAsync();
                //不同的总年数量
                var sumYearCount = gradientModelYear.DistinctBy(p => p.Year).Count();
                var valueSum = gradientModelYear.Sum(p => p.Count);
                var monthEndDemand = valueSum * 1000 / sumYearCount / 12;


                var allYear = await year.SelectAsync(async p => await GetData(new GetLogisticsCostInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = p.Year, UpDown = p.UpDown }, solution, gradientValue));

                var data = allYear.SelectMany(p => p);

                var cs = year.First().UpDown == YearType.Year ? 12 : 6;
                var yearCount = year.DistinctBy(p => p.Year).Count();

                var dto = new ProductionControlInfoListDto
                {
                    EditId = "0",
                    Year = "全生命周期",
                    Freight = data.Sum(p => p.Freight * cs) / (yearCount * 12),
                    MonthEndDemand = monthEndDemand,
                    StorageExpenses = data.Sum(p => p.StorageExpenses * cs) / (yearCount * 12),
                    PerPackagingPrice = data.Sum(p => p.PerPackagingPrice * year.Count) / year.Sum(p => p.Quantity),
                    PerFreight = data.Sum(p => p.PerFreight * year.Count) / year.Sum(p => p.Quantity),
                    PerTotalLogisticsCost = data.Sum(p => p.PerTotalLogisticsCost * year.Count) / year.Sum(p => p.Quantity),
                };
                return new List<ProductionControlInfoListDto> { dto };

                //var queryItem = await _logisticscostRepository.GetAll().Where(t => t.AuditFlowId == input.AuditFlowId && t.SolutionId == input.SolutionId && t.Classification == gradientValue)
                //    .Select(l => new ProductionControlInfoListDto
                //    {
                //        Year = l.Year,//input.Year.ToString(),
                //        Freight = l.FreightPrice.GetValueOrDefault(),
                //        MonthEndDemand = monthEndDemand,//l.MonthlyDemandPrice.GetValueOrDefault(),
                //        PerFreight = l.SinglyDemandPrice.GetValueOrDefault(),
                //        PerPackagingPrice = l.PackagingPrice.GetValueOrDefault(),
                //        PerTotalLogisticsCost = l.TransportPrice.GetValueOrDefault(),
                //        StorageExpenses = l.StoragePrice.GetValueOrDefault(),
                //    })
                //.ToListAsync();
                //return queryItem;
            }
            else
            {
                return await GetData(input, solution, gradientValue);
            }

            async Task<List<ProductionControlInfoListDto>> GetData(GetLogisticsCostInput input, Solution solution, string gradientValue)
            {
                var gradientModelYear = await (from gm in _gradientModelRepository.GetAll()
                                               join gmy in _gradientModelYearRepository.GetAll() on gm.Id equals gmy.GradientModelId
                                               where gm.AuditFlowId == input.AuditFlowId && gm.GradientId == input.GradientId && gm.ProductId == solution.Productld && gmy.Year == input.Year && gmy.UpDown == input.UpDown
                                               select gmy).FirstOrDefaultAsync();
                var monthEndDemand = gradientModelYear.Count * 1000 / (input.UpDown == YearType.Year ? 12 : 6);

                var data = await (from l in _logisticscostRepository.GetAll()
                                  join y in _modelCountYearRepository.GetAll() on l.ModelCountYearId equals y.Id
                                  where l.AuditFlowId == input.AuditFlowId && l.SolutionId == input.SolutionId && l.Classification == gradientValue
                                  && y.UpDown == input.UpDown && y.Year == input.Year
                                  select new ProductionControlInfoListDto
                                  {
                                      EditId = l.Id.ToString(),
                                      Year = input.Year.ToString(),
                                      Freight = l.FreightPrice.GetValueOrDefault(),
                                      MonthEndDemand = monthEndDemand, //l.MonthlyDemandPrice.GetValueOrDefault(),
                                      PerFreight = l.SinglyDemandPrice.GetValueOrDefault(),
                                      PerPackagingPrice = l.PackagingPrice.GetValueOrDefault(),
                                      PerTotalLogisticsCost = l.TransportPrice.GetValueOrDefault(),
                                      StorageExpenses = l.StoragePrice.GetValueOrDefault(),
                                  }).ToListAsync();


                //取得修改项
                var updateItem = await _updateItemRepository
                    .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                    && p.UpdateItemType == UpdateItemType.LogisticsCost && p.GradientId == input.GradientId
                    && p.SolutionId == input.SolutionId
                    && p.Year == input.Year && p.UpDown == input.UpDown);

                var logisticsCost = ObjectMapper.Map<SetUpdateItemInput<List<ProductionControlInfoListDto>>>(updateItem);
                if (logisticsCost is not null)
                {
                    var dataIds = logisticsCost.UpdateItem.Select(p => p.EditId);

                    foreach (var item in data.Where(p => dataIds.Contains(p.EditId)))
                    {
                        ObjectMapper.Map(logisticsCost.UpdateItem.FirstOrDefault(p => p.EditId == item.EditId), item);
                    }
                }

                return data;
            }



            ////物流成本
            //var productionControlInfo = await _productionControlInfoRepository
            //    .GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld)
            //    .WhereIf(input.Year != PriceEvalConsts.AllYear, p => p.Year == input.Year.ToString())
            //    .ToListAsync();
            //return ObjectMapper.Map<List<ProductionControlInfoListDto>>(productionControlInfo);
        }

        #endregion

        #region 质量成本

        /// <summary>
        /// 获取 质量成本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<QualityCostListDto> GetQualityCost(GetOtherCostItemInput input)
        {
            var solution = await _solutionRepository.GetAsync(input.SolutionId);
            var gradientModel = await _gradientModelRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.GradientId == input.GradientId && p.ProductId == solution.Productld);


            //物流费
            decimal logisticsFee = await GetLogisticsFee(input);

            //制造成本
            var manufacturingCost = await GetManufacturingCost(new GetManufacturingCostInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown });

            //全生命周期处理
            if (input.Year == PriceEvalConsts.AllYear)
            {
                //获取总年数
                var yearCount = await _gradientModelYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.GradientModelId == gradientModel.Id)
                        .OrderBy(p => p.Year).Select(p => new { p.Year, Quantity = p.Count, p.UpDown }).ToListAsync();

                //var yearCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld).ToListAsync();

                var data = await yearCount.SelectAsync(async p =>
                {
                    var dto = new GetOtherCostItemInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = p.Year, UpDown = p.UpDown };
                    var result = await GetBomCost(new GetBomCostInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = p.Year, UpDown = p.UpDown, InputCount = 0 });
                    return (dto, result);
                });

                var otherCostItemtList = (await data.SelectAsync(async p => await GetQualityCostPrivate(p.dto, p.result, logisticsFee, manufacturingCost.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal))).ToList();

                var qualityCost = (from o in otherCostItemtList
                                   join y in yearCount on o.Year equals y.Year
                                   select o.QualityCost * y.Quantity).Sum();

                return new QualityCostListDto
                {
                    ProductCategory = otherCostItemtList.FirstOrDefault().ProductCategory,
                    CostProportion = otherCostItemtList.Sum(p => p.CostProportion),
                    QualityCost = qualityCost / yearCount.Sum(p => p.Quantity),
                };
            }
            else
            {


                //物料成本
                var electronicAndStructureList = await this.GetBomCost(new GetBomCostInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown });
                var dto = await this.GetQualityCostPrivate(input, electronicAndStructureList, logisticsFee, manufacturingCost.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal);


                //取得修改项
                var updateItem = await _updateItemRepository
                    .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                    && p.UpdateItemType == UpdateItemType.QualityCost && p.GradientId == input.GradientId
                    && p.SolutionId == input.SolutionId
                    && p.Year == input.Year && p.UpDown == input.UpDown);

                var qualityCostListDto = ObjectMapper.Map<SetUpdateItemInput<List<QualityCostListDto>>>(updateItem);
                if (qualityCostListDto is not null)
                {
                    var q = qualityCostListDto.UpdateItem.FirstOrDefault(p => p.EditId == dto.EditId);
                    if (q is not null)
                    {
                        return q;
                    }
                }
                return dto;
            }
        }

        /// <summary>
        /// 获取目标毛利率
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        private static decimal GetGrossProfitMargin(string productType)
        {
            switch (productType)
            {
                case FinanceConsts.ProductType_ExternalImaging: return 0.18M;
                case FinanceConsts.ProductType_EnvironmentalPerception: return 0.28M;
                case FinanceConsts.ProductType_CabinMonitoring: return 0.24M;
                default:
                    throw new FriendlyException($"营销部录入的产品小类有误，录入的产品小类为：{productType}");
            }
        }

        /// <summary>
        /// 获取 质量成本(内部使用)（单年份）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="electronicAndStructureList"></param>
        /// <param name="logisticsFee"></param>
        /// <param name="manufacturingCostSubtotal"></param>
        /// <returns></returns>
        private async Task<QualityCostListDto> GetQualityCostPrivate(GetOtherCostItemInput input, List<Material> electronicAndStructureList
         , decimal logisticsFee, decimal manufacturingCostSubtotal)
        {
            var solution = await _solutionRepository.GetAsync(input.SolutionId);

            var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);

            var sopYear = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld).MinAsync(p => p.Year);

            var qualituCostYear = input.Year - sopYear;

            //产品大类名
            var productType = await (from m in _modelCountRepository.GetAll()
                                     where m.Id == solution.Productld
                                     select m.ProductType).FirstOrDefaultAsync();
            //产品大类是否存在质量成本比例中
            var productTypeNameIsHas = await _qualityCostRatioRepository.GetAll().AnyAsync(p => p.Category == productType);

            //筛选用的产品大类
            var filterProductTypeName = productTypeNameIsHas ? productType : FinanceConsts.QualityCostType_Qt;


            //项目管理部人员输入
            var userInputInfo = await _userInputInfoRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);

            //产品类别
            var modelCount = await _modelCountRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.Id == solution.Productld);

            ////1-目标毛利率
            //var grossProfitMargin = 1 - GetGrossProfitMargin(modelCount.ProductType);


            //成本比例
            //await (from q in _qualityCostProportionEntryInfoRepository.GetAll()
            //                        join qy in _qualityCostProportionYearInfoRepository.GetAll() on q.Id equals qy.QualityCostId
            //                        join d in _financeDictionaryDetailRepository.GetAll() on q.Category equals d.Id
            //                        join m in _modelCountRepository.GetAll() on d.Id equals m.ProductType
            //                        where m.Id == solution.Productld && q.IsFirst == userInputInfo.IsFirst
            //                        && qy.Year <= input.Year
            //                        select qy).OrderByDescending(p => p.Year).Select(p => p.Rate).FirstOrDefaultAsync();

            var costProportion = await (from a in _qualityCostRatioRepository.GetAll()
                                        join b in _qualityCostRatioYearRepository.GetAll() on a.Id equals b.QualityCostRatioId
                                        where a.Category == filterProductTypeName && a.IsItTheFirstProduct == solution.IsFirst
                                        && b.Year == qualituCostYear
                                        select b.Value).FirstOrDefaultAsync();

            ////材料成本合计（质量成本（MAX)）
            //var totalMaterialCost = (electronicAndStructureList.Sum(p => p.MaterialCost) + electronicAndStructureList.Sum(p => p.MoqShareCount)
            //    + logisticsFee + manufacturingCostSubtotal) / grossProfitMargin * costProportion;

            var totalMaterialCost = electronicAndStructureList.Sum(p => p.TotalMoneyCyn) * costProportion / 100;


            //产品小类名称
            var productTypeName = await (from m in _modelCountRepository.GetAll()
                                         join d in _financeDictionaryDetailRepository.GetAll() on m.ProductType equals d.Id
                                         where m.Id == solution.Productld && m.AuditFlowId == input.AuditFlowId
                                         select d.DisplayName).FirstOrDefaultAsync();

            return new QualityCostListDto
            {
                EditId = UpdateItemType.QualityCost.ToString(),
                Year = input.Year,
                ProductCategory = productTypeName,
                CostProportion = costProportion,
                QualityCost = totalMaterialCost,
            };
        }


        #endregion

        #region 后端对接函数

        /// <summary>
        /// 根据流程表主键获取Pcs数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<PcsListDto>> GetPcsByPriceAuditFlowId(long auditFlowId) =>
            new ListResultDto<PcsListDto>((await _pcsRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
               .Join(_pcsYearRepository.GetAll(), p => p.Id, p => p.PcsId, (pcs, pcsYear) => new { pcs, pcsYear }).ToListAsync()).GroupBy(p => p.pcs).Select(p =>
               {
                   var dto = ObjectMapper.Map<PcsListDto>(p.Key);
                   dto.PcsYear = ObjectMapper.Map<IList<PcsYearListDto>>(p.Select(o => o.pcsYear));
                   return dto;
               }).ToList());

        /// <summary>
        /// 根据流程表主键获取模组数量
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ModelCountListDto>> GetModelCountByAuditFlowId(long auditFlowId)
        {
            var data = (await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
                    .Join(_modelCountYearRepository.GetAll(), p => p.Id, p => p.ProductId, (modelCount, modelCountYear) => new { modelCount, modelCountYear }).ToListAsync()).GroupBy(p => p.modelCount).Select(p =>
                    {
                        var dto = ObjectMapper.Map<ModelCountListDto>(p.Key);
                        dto.ModelCountYearListDto = ObjectMapper.Map<IList<ModelCountYearListDto>>(p.Select(o => o.modelCountYear));
                        return dto;
                    }).ToList();
            return new ListResultDto<ModelCountListDto>(data);
        }



        #endregion

        #region Json读取
        /// <summary>
        /// 根据Json获取材料单价（原币），如果isAll为ture，根据公式，如果为false，根据选择的年份
        /// </summary>
        /// <param name="json"></param>
        /// <param name="isAll"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static decimal GetMaterialPrice(string json, int year, YearType upDown, decimal gradientValue)
        {
            return GetYearValue(json, year, upDown, gradientValue);
        }

        /// <summary>
        /// 根据Json获取汇率，如果isAll为ture，根据平均值，如果为false，根据选择的年份
        /// </summary>
        /// <param name="json"></param>
        /// <param name="isAll"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static decimal GetExchangeRate(string json, int year)
        {
            var list = JsonConvert.DeserializeObject<List<YearOrValueMode>>(json);
            var query = list.Where(p => p.Year == year);
            if (query.Any())
            {
                return query.FirstOrDefault().Value;
            }
            else
            {
                return list.OrderByDescending(p => p.Year).FirstOrDefault().Value;
            }
        }

        /// <summary>
        /// 获取年份值，如果获取为不存在的年份，则取最后一年
        /// </summary>
        /// <param name="json"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static decimal GetYearValue(string json, int year, YearType upDown, decimal gradientValue)
        {
            //var list = EnteringMapper.JsonToList(json);
            var list = JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(json).FirstOrDefault(p => p.Kv == gradientValue).YearOrValueModes;
            var query = list.Where(p => p.Year == year && p.UpDown == upDown);
            if (query.Any())
            {
                return query.FirstOrDefault().Value;
            }
            else
            {
                return list.OrderByDescending(p => p.Year).FirstOrDefault().Value;
            }
        }

        #endregion

        #region 核价看板

        /// <summary>
        /// 核价看板-【产品选择】下拉框下拉数据接口
        /// </summary>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ModelCountSelectListDto>> GetPricingPanelProductSelectList(GetPricingPanelProductSelectListInput input)
        {
            var data = from m in _modelCountRepository.GetAll()
                       where m.AuditFlowId == input.AuditFlowId
                       select new ModelCountSelectListDto
                       {
                           Id = m.Id,
                           ProductName = m.Product
                       };
            var result = await data.ToListAsync();
            return new ListResultDto<ModelCountSelectListDto>(result);
        }

        /// <summary>
        /// 核价看板-时间选择下拉框下拉数据接口
        /// </summary>
        /// <returns></returns>
        public async virtual Task<ListResultDto<YearListDto>> GetPricingPanelTimeSelectList(GetPricingPanelTimeSelectListInput input)
        {
            var data = await _pcsYearRepository.GetAll()
                .Where(p => p.AuditFlowId == input.AuditFlowId)
                .Select(p => new YearListDto { Id = p.Year, Name = $"{p.Year}年", UpDown = p.UpDown })
                .Distinct()
                .OrderBy(p => p.Id)
                .ToListAsync();
            if (data.Count > 0)
            {
                data.Add(new YearListDto { Id = PriceEvalConsts.AllYear, Name = "全生命周期" });
            }

            return new ListResultDto<YearListDto>(data.OrderBy(p => p.Id).ThenBy(p => p.UpDown).ToList());
        }

        /// <summary>
        /// 核价看板-产品成本占比图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ProportionOfProductCostListDto>> GetPricingPanelProportionOfProductCost(GetPricingPanelProportionOfProductCostInput input)
        {
            var data = await this.GetPriceEvaluationTable(new GetPriceEvaluationTableInput { AuditFlowId = input.AuditFlowId, InputCount = 0, GradientId = input.GradientId, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown });

            //bom成本
            var bomCost = data.Material.Sum(p => p.TotalMoneyCyn);

            //损耗成本
            var costItemAll = data.Material.Sum(p => p.Loss);

            //制造成本
            var manufacturingCost = data.ManufacturingCost.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal; //data.ManufacturingCost.Where(p => p.CostType != CostType.Total).Sum(p => p.Subtotal); 

            //物流成本
            var logisticsFee = data.OtherCostItem.LogisticsFee;

            //质量成本
            var qualityCost = data.OtherCostItem.QualityCost;

            //其他成本
            var other = data.OtherCostItem2.FirstOrDefault(p => p.ItemName == "单颗成本").Total.GetValueOrDefault();


            var sum = bomCost + costItemAll + manufacturingCost + logisticsFee + qualityCost + other;

            var list = new List<ProportionOfProductCostListDto>
            {
                new ProportionOfProductCostListDto{ Name="bom成本", Proportion= bomCost/sum},
                new ProportionOfProductCostListDto{ Name="损耗成本", Proportion= costItemAll/sum},
                new ProportionOfProductCostListDto{ Name="制造成本", Proportion= manufacturingCost/sum},
                new ProportionOfProductCostListDto{ Name="物流成本", Proportion= logisticsFee/sum},
                new ProportionOfProductCostListDto{ Name="质量成本", Proportion= qualityCost/sum},
                new ProportionOfProductCostListDto{ Name="其他成本", Proportion= other/sum},

            };
            return new ListResultDto<ProportionOfProductCostListDto>(list);
        }

        /// <summary>
        /// 添加核价表TR方案Id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task AddPricingPanelTrProgrammeId(AddPricingPanelTrProgrammeIdInput input)
        {
            await _priceEvaluationRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId).UpdateFromQueryAsync(p => new PriceEvaluation
            {
                TrProgramme = input.FileManagementId
            });
        }

        /// <summary>
        /// 获取推移图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<GoTable>> GetGoTable(GetGoTableInput input)
        {
            var solution = await _solutionRepository.GetAsync(input.SolutionId);

            //获取总年数
            var yearCount = await _modelCountYearRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld)
                .Select(p => new { p.Year, p.UpDown }).OrderBy(p => p.Year).ToListAsync();
            var dtoList = (await yearCount.SelectAsync(async p => await GetPriceEvaluationTable(new GetPriceEvaluationTableInput { Year = p.Year, GradientId = input.GradientId, AuditFlowId = input.AuditFlowId, InputCount = input.InputCount, SolutionId = input.SolutionId, UpDown = p.UpDown }))).ToList();
            var dto = dtoList.Select(p => new GoTable { Year = p.Year, UpDown = p.UpDown, Value = p.TotalCost }).ToList();
            return new ListResultDto<GoTable>(dto);
        }


        /// <summary>
        /// 初版产品核价表下载-流
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal async virtual Task<MemoryStream> PriceEvaluationTableDownloadStream(PriceEvaluationTableDownloadStreamInput input)
        {
            return null;
            //if (input.IsAll)
            //{
            //    var dtoAll = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(await GetPriceEvaluationTableResult(new GetPriceEvaluationTableResultInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, IsAll = true }));
            //    DtoExcel(dtoAll);
            //    var memoryStream = new MemoryStream();
            //    await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/PriceEvaluationTable.xlsx", dtoAll);
            //    return memoryStream;
            //}
            //else
            //{


            //    var dto = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(await GetPriceEvaluationTableResult(new GetPriceEvaluationTableResultInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, IsAll = false }));

            //    DtoExcel(dto);


            //    var memoryStream2 = new MemoryStream();

            //    await MiniExcel.SaveAsByTemplateAsync(memoryStream2, "wwwroot/Excel/PriceEvaluationTable.xlsx", dto);
            //    return memoryStream2;
            //}
        }

        private string GetYearName(YearType yearType)
        {
            if (yearType == YearType.Year)
            {
                return "年";
            }
            else if (yearType == YearType.FirstHalf)
            {
                return "上半年";

            }
            else if (yearType == YearType.SecondHalf)
            {
                return "下半年";

            }
            else
            {
                return "无法识别的类型";
            }
        }


        /// <summary>
        /// 初版产品核价表下载
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async virtual Task<FileResult> PriceEvaluationTableDownload(PriceEvaluationTableDownloadInput input)
        {
            var solution = await _solutionRepository.GetAsync(input.SolutionId);
            var productId = solution.Productld;

            var year = await _modelCountYearRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == productId);

            var dtoAll = ObjectMapper.Map<ExcelPriceEvaluationTableDto>(await GetPriceEvaluationTable(new GetPriceEvaluationTableInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, InputCount = 0, Year = 0, UpDown = YearType.Year }));

            DtoExcel(dtoAll);
            var dto = await year.SelectAsync(async p => await GetPriceEvaluationTable(new GetPriceEvaluationTableInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, SolutionId = input.SolutionId, InputCount = 0, Year = p.Year, UpDown = p.UpDown }));
            var dtos = dto.Select(p => ObjectMapper.Map<ExcelPriceEvaluationTableDto>(p)).Select(p => DtoExcel(p));

            var streams = (await dtos.Select(p => new { stream = new MemoryStream(), p })
                .SelectAsync(async p =>
                {
                    await MiniExcel.SaveAsByTemplateAsync(p.stream, "wwwroot/Excel/PriceEvaluationTableModel.xlsx", p.p);
                    return new { stream = p.stream, excels = $"{p.p.Year}{GetYearName(p.p.UpDown)}" };
                })).ToList();

            var memoryStream = new MemoryStream();


            await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/PriceEvaluationTableModel.xlsx", dtoAll);


            streams.Add(new { stream = memoryStream, excels = "全生命周期" });

            var ex = streams.Select(p => (p.stream, p.excels)).ToArray();
            var memoryStream2 = NpoiExtensions.ExcelMerge(ex);

            return new FileContentResult(memoryStream2.ToArray(), "application/octet-stream") { FileDownloadName = "产品核价表.xlsx" };

        }

        /// <summary>
        /// Dto Excel导出处理
        /// </summary>
        /// <param name="dtoAll"></param>
        private static ExcelPriceEvaluationTableDto DtoExcel(ExcelPriceEvaluationTableDto dtoAll)
        {
            dtoAll.Material = dtoAll.Material.Select((a, b) => { a.Index = b + 1; return a; }).ToList();

            dtoAll.ManufacturingCostDto = dtoAll.ManufacturingCost.Select(p => new ManufacturingCostDto2
            {
                DirectLabor1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.DirectLabor,
                EquipmentDepreciation1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.EquipmentDepreciation,
                LineChangeCost1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.LineChangeCost,
                ManufacturingExpenses1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.ManufacturingExpenses,
                Subtotal1 = p.CostType == CostType.Other ? null : p.ManufacturingCostDirect.Subtotal,
                DirectLabor2 = p.CostType == CostType.Other ? null : p.ManufacturingCostIndirect.DirectLabor,
                EquipmentDepreciation2 = p.CostType == CostType.Other ? null : p.ManufacturingCostIndirect.EquipmentDepreciation,
                ManufacturingExpenses2 = p.CostType == CostType.Other ? null : p.ManufacturingCostIndirect.ManufacturingExpenses,
                Subtotal2 = p.CostType == CostType.Other ? null : p.ManufacturingCostIndirect.Subtotal,

                Subtotal = p.Subtotal,
                CostItem = p.CostItem,
                CostType = p.CostType
            }).ToList();

            dtoAll.Fixture = dtoAll.OtherCostItem.Fixture;
            dtoAll.LogisticsFee = dtoAll.OtherCostItem.LogisticsFee;
            dtoAll.ProductCategory = dtoAll.OtherCostItem.ProductCategory;
            dtoAll.CostProportion = dtoAll.OtherCostItem.CostProportion;
            dtoAll.CostProportionText = $"{dtoAll.OtherCostItem.CostProportion * 100}%";
            dtoAll.QualityCost = dtoAll.OtherCostItem.QualityCost;
            dtoAll.AccountingPeriod = dtoAll.OtherCostItem.AccountingPeriod;
            dtoAll.CapitalCostRate = dtoAll.OtherCostItem.CapitalCostRate;
            dtoAll.TaxCost = dtoAll.OtherCostItem.TaxCost;
            dtoAll.Total = dtoAll.OtherCostItem.Total;
            dtoAll.WastageCostCount = dtoAll.LossCost.Sum(p => p.WastageCost);
            dtoAll.MoqShareCountCount = dtoAll.LossCost.Sum(p => p.MoqShareCount);

            dtoAll.ShDzl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.ElectronicName).Sum(p => p.WastageCost).ToString();
            dtoAll.ShJgl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.StructuralName).Sum(p => p.WastageCost).ToString();
            dtoAll.ShJs = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.GlueMaterialName).Sum(p => p.WastageCost).ToString();
            dtoAll.ShWxjg = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.SMTOutSourceName).Sum(p => p.WastageCost).ToString();
            dtoAll.ShBzcl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.PackingMaterialName).Sum(p => p.WastageCost).ToString();


            dtoAll.MoqDzl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.ElectronicName).Sum(p => p.MoqShareCount).ToString();
            dtoAll.MoqJgl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.StructuralName).Sum(p => p.MoqShareCount).ToString();
            dtoAll.MoqJs = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.GlueMaterialName).Sum(p => p.MoqShareCount).ToString();
            dtoAll.MoqWxjg = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.SMTOutSourceName).Sum(p => p.MoqShareCount).ToString();
            dtoAll.MoqBzcl = dtoAll.LossCost.Where(p => p.Name == FinanceConsts.PackingMaterialName).Sum(p => p.MoqShareCount).ToString();


            dtoAll.TotalMoneyCynCount = dtoAll.Material.Sum(p => p.TotalMoneyCyn);
            dtoAll.LossCount = dtoAll.Material.Sum(p => p.Loss);
            dtoAll.LossRateCount = dtoAll.Material.Sum(p => p.LossRate);

            return dtoAll;
        }


        /// <summary>
        /// 核价看板页面，设置是否客供。填写BOM的Id和是否客供
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task SetIsCustomerSupply(SetIsCustomerSupplyInput input)
        {
            var pe = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);
            pe.BomIsCustomerSupplyJson = input.BomIsCustomerSupplyList.ToJsonString();
        }

        /// <summary>
        /// 核价看板，梯度下拉框获取梯度接口
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<GradientListDto>> GetGradient(long auditFlowId)
        {
            var entity = await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            return new PagedResultDto<GradientListDto>(entity.Count, ObjectMapper.Map<List<GradientListDto>>(entity));
        }

        ///// <summary>
        ///// 设置核价看板修改项
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public virtual async Task SetEditItem(SetEditItemInput input)
        //{
        //    var entity = await _editItemRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);
        //    if (entity is null)
        //    {
        //        await _editItemRepository.InsertAsync(new EditItem
        //        {
        //            AuditFlowId = input.AuditFlowId,
        //            EditItemJson = input.EditItem.ToJsonString(),
        //        });
        //    }
        //    else
        //    {
        //        entity.EditItemJson = input.EditItem.ToJsonString();
        //    }

        //}
        #endregion


        #region 方案成本对比表

        /// <summary>
        /// 方案成本对比表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async virtual Task<List<SolutionContrast>> GetSolutionContrast(GetSolutionContrastInput input)
        {
            var bom1 = await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = input.AuditFlowId,
                GradientId = input.GradientId,
                InputCount = 0,
                SolutionId = input.SolutionId_1,
                Year = input.Year,
                UpDown = input.UpDown,
            });

            var bom2 = await GetPriceEvaluationTable(new GetPriceEvaluationTableInput
            {
                AuditFlowId = input.AuditFlowId,
                GradientId = input.GradientId,
                InputCount = 0,
                SolutionId = input.SolutionId_2,
                Year = input.Year,
                UpDown = input.UpDown,
            });

            var result = new List<SolutionContrast>
            {
                new SolutionContrast { ItemName="PCBA（除sensor芯片、串行芯片）",
                    Price_1 = null,
                    Count_1 = bom1.Material.Count(p => p.TypeName !="串行芯片" && p.TypeName !="Sensor芯片"),
                    Rate_1 = null,
                    Sum_1 =bom1.Material.Where(p => p.TypeName !="串行芯片" && p.TypeName !="Sensor芯片").Sum(p=>p.TotalMoneyCynNoCustomerSupply),

                    Price_2 = null,
                    Count_2 = bom1.Material.Count(p => p.TypeName !="串行芯片" && p.TypeName !="Sensor芯片"),
                    Rate_2 = null,
                    Sum_2 =bom2.Material.Where(p => p.TypeName !="串行芯片" && p.TypeName !="Sensor芯片").Sum(p=>p.TotalMoneyCynNoCustomerSupply),
                },
                new SolutionContrast { ItemName="结构件（除lens）",
                    Price_1 = null,
                    Count_1 = bom1.Material.Count(p=>p.SuperType == FinanceConsts.ElectronicName && p.TypeName != "镜头"),
                    Rate_1 = null,
                    Sum_1 =bom1.Material.Where(p=>p.SuperType == FinanceConsts.ElectronicName && p.TypeName != "镜头").Sum(p=>p.TotalMoneyCynNoCustomerSupply),

                    Price_2 = null,
                    Count_2 = bom1.Material.Count(p=>p.Id.StartsWith(ElectronicBomName) && p.TypeName != "镜头"),
                    Rate_2 = null,
                    Sum_2 =bom2.Material.Where(p=>p.Id.StartsWith(ElectronicBomName) && p.TypeName != "镜头").Sum(p=>p.TotalMoneyCynNoCustomerSupply),
                },
                new SolutionContrast { ItemName="损耗成本",
                    Price_1 = null,
                    Count_1 = null,
                    Rate_1 = null,
                    Sum_1 =bom1.LossCost.Sum(p=>p.WastageCost),

                     Price_2 = null,
                    Count_2 = null,
                    Rate_2 = null,
                    Sum_2 =bom2.LossCost.Sum(p=>p.WastageCost),
                },
                new SolutionContrast { ItemName="制造成本",
                    Price_1 = null,
                    Count_1 = null,
                    Rate_1 = null,
                    Sum_1 = bom1.ManufacturingCost.FirstOrDefault(p=>p.CostType == CostType.Total).Subtotal,

                     Price_2 = null,
                    Count_2 = null,
                    Rate_2 = null,
                    Sum_2 = bom2.ManufacturingCost.FirstOrDefault(p=>p.CostType == CostType.Total).Subtotal
                },

                new SolutionContrast { ItemName="物流成本",
                    Price_1 = null,
                    Count_1 = null,
                    Rate_1 = null,
                    Sum_1 = bom1.OtherCostItem.LogisticsFee,

                    Price_2 = null,
                    Count_2 = null,
                    Rate_2 = null,
                    Sum_2 = bom2.OtherCostItem.LogisticsFee,
                },
                new SolutionContrast { ItemName="质量成本",
                    Price_1 = null,
                    Count_1 = null,
                    Rate_1 = null,
                    Sum_1 = bom1.OtherCostItem.QualityCost,

                    Price_2 = null,
                    Count_2 = null,
                    Rate_2 = null,
                    Sum_2 = bom2.OtherCostItem.QualityCost,
                },
                new SolutionContrast { ItemName="其他成本",
                    Price_1 = null,
                    Count_1 = null,
                    Rate_1 = null,
                    Sum_1 = bom1.OtherCostItem2.FirstOrDefault(p=>p.ItemName == "单颗成本").Total,

                    Price_2 = null,
                    Count_2 = null,
                    Rate_2 = null,
                    Sum_2 = bom2.OtherCostItem2.FirstOrDefault(p=>p.ItemName == "单颗成本").Total,
                },
                new SolutionContrast { ItemName="总成本",
                    Price_1 = null,
                    Count_1 = null,
                    Rate_1 = null,
                    Sum_1 = bom1.TotalCost,

                    Price_2 = null,
                    Count_2 = null,
                    Rate_2 = null,
                    Sum_2 = bom2.TotalCost
                },
            };

            var list = new List<string> { "Sensor芯片", "串行芯片", "镜头" };

            var data = from o in bom1.Material
                       join t in bom2.Material on o.Id equals t.Id
                       where list.Contains(o.TypeName)
                       select new SolutionContrast
                       {
                           ItemName = $"{o.TypeName}：{o.MaterialName}",
                           Price_1 = o?.MaterialPrice,
                           Count_1 = o?.AssemblyCount.To<decimal>(),
                           Rate_1 = o?.ExchangeRate,
                           Sum_1 = o?.TotalMoneyCynNoCustomerSupply,

                           Price_2 = t?.MaterialPrice,
                           Count_2 = t?.AssemblyCount.To<decimal>(),
                           Rate_2 = t?.ExchangeRate,
                           Sum_2 = t?.TotalMoneyCynNoCustomerSupply,
                       };
            result.AddRange(data);
            result.ForEach(p => p.Change = p.Sum_2 - p.Sum_1);

            return result;
        }

        /// <summary>
        /// 方案成本对比表下载
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async virtual Task<FileResult> GetSolutionContrastDonwload(GetSolutionContrastInput input)
        {

            var solution1 = await _solutionRepository.FirstOrDefaultAsync(p => p.Id == input.SolutionId_1);
            var solution2 = await _solutionRepository.FirstOrDefaultAsync(p => p.Id == input.SolutionId_2);

            if (solution1 is null)
            {
                throw new FriendlyException($"系统中没有请求的方案Id：{input.SolutionId_1}");
            }

            if (solution2 is null)
            {
                throw new FriendlyException($"系统中没有请求的方案Id：{input.SolutionId_2}");
            }

            var data = await GetSolutionContrast(input);


            var dto = new SolutionContrastExcel { Name1 = solution1.ModuleName, Name2 = solution2.ModuleName, SolutionContrast = data };

            var memoryStream = new MemoryStream();

            //await MiniExcel.SaveAsAsync(memoryStream, data);
            await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/SolutionContrast.xlsx", dto);


            return new FileContentResult(memoryStream.ToArray(), "application/octet-stream") { FileDownloadName = "方案成本对比表.xlsx" };
        }

        #endregion
    }
}
