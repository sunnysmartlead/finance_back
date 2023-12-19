using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Finance.DemandApplyAudit;
using Finance.PriceEval;
using Finance.Processes;
using Finance.PropertyDepartment.UnitPriceLibrary;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Finance.WorkFlows;
using Interface.Expends;
using Microsoft.EntityFrameworkCore;
using NPOI.POIFS.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Job
{
    /// <summary>
    /// 中标后添加公用物料库
    /// </summary>
    public class PublicMateriaJob : AsyncBackgroundJob<long>, ITransientDependency
    {
        private readonly UnitPriceLibraryAppService _unitPriceLibraryAppService;
        private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;
        private readonly IRepository<ModelCount, long> _modelCountRepository;
        private readonly IRepository<ModelCountYear, long> _modelCountYearRepository;
        private readonly IRepository<Solution, long> _solutionRepository;
        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;
        private readonly IRepository<Gradient, long> _gradientRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public PublicMateriaJob(UnitPriceLibraryAppService unitPriceLibraryAppService, PriceEvaluationGetAppService priceEvaluationGetAppService, IRepository<ModelCount, long> modelCountRepository, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<Solution, long> solutionRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<Gradient, long> gradientRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _unitPriceLibraryAppService = unitPriceLibraryAppService;
            _priceEvaluationGetAppService = priceEvaluationGetAppService;
            _modelCountRepository = modelCountRepository;
            _modelCountYearRepository = modelCountYearRepository;
            _solutionRepository = solutionRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _gradientRepository = gradientRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// 中标后添加公用物料库
        /// </summary>
        /// <param name="workFlowInstanceId"></param>
        /// <returns></returns>
        public async override Task ExecuteAsync(long workFlowInstanceId)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    //如果是样品核价，就不执行
                    var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == workFlowInstanceId);
                    if (priceEvaluation is null || (!priceEvaluation.IsHasSample.HasValue) || priceEvaluation.IsHasSample.Value)
                    {
                        return;
                    }

                    var products = (from m in _modelCountRepository.GetAll()
                                    join y in _modelCountYearRepository.GetAll() on m.Id equals y.ProductId
                                    join s in _solutionRepository.GetAll() on m.Id equals s.Productld
                                    join g in _gradientRepository.GetAll() on m.AuditFlowId equals g.AuditFlowId
                                    where m.AuditFlowId == workFlowInstanceId
                                    select new
                                    {
                                        m.Id,
                                        ProductName = m.Product,
                                        s.SolutionName,
                                        SolutionId = s.Id,
                                        m.Code,
                                        y.Year,
                                        y.UpDown,
                                        GradientId = g.Id,
                                    }).ToList().DistinctBy(p => p.Id);

                    var data = new List<SharedMaterialWarehouseMode>();
                    foreach (var item in products)
                    {
                        var moduleThroughputs = (from m in _modelCountRepository.GetAll()
                                                 join y in _modelCountYearRepository.GetAll() on m.Id equals y.ProductId
                                                 where m.Id == item.Id
                                                 select new
                                                 {
                                                     y.Year,
                                                     y.UpDown,
                                                     Value = y.Quantity,
                                                 }).ToList().GroupBy(p => p.Year)
                                                 .Select(p => new YearOrValueModeCanNull
                                                 {
                                                     Year = p.Key,
                                                     Value = p.Sum(p => p.Value)
                                                 }).ToList();

                        var bom = await _priceEvaluationGetAppService.GetBomCost(new PriceEval.Dto.GetBomCostInput
                        {
                            AuditFlowId = workFlowInstanceId,
                            GradientId = item.GradientId,
                            InputCount = 0,
                            SolutionId = item.SolutionId,
                            Year = item.Year,
                            UpDown = item.UpDown
                        });

                        var dto = bom.Select(p => new SharedMaterialWarehouseMode
                        {
                            EntryName = priceEvaluation.ProjectName,
                            ProjectSubcode = item.Code,
                            ProductName = item.ProductName,
                            SolutionName = item.SolutionName,
                            MaterialCode = p.Sap,
                            MaterialName = p.MaterialName,
                            AssemblyQuantity = p.AssemblyCount.To<decimal>(),
                            ModuleThroughputs = moduleThroughputs
                        });

                        data.AddRange(dto);
                    }
                    await _unitPriceLibraryAppService.AddPublicMaterialWarehouse(data);
                }
                uow.Complete();
            }
        }
    }
}
