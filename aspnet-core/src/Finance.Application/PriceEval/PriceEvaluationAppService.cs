using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Json;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using AutoMapper;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.BaseLibrary;
using Finance.DemandApplyAudit;
using Finance.EngineeringDepartment;
using Finance.Entering;
using Finance.EntityFrameworkCore.Seed.Host;
using Finance.Ext;
using Finance.FinanceMaintain;
using Finance.FinanceParameter;
using Finance.Hr;
using Finance.Infrastructure;
using Finance.NerPricing;
using Finance.Nre;
using Finance.NrePricing.Dto;
using Finance.PriceEval.Dto;
using Finance.PriceEval.Dto.AllManufacturingCost;
using Finance.Processes;
using Finance.ProductDevelopment;
using Finance.ProductionControl;
using Finance.ProjectManagement;
using Finance.ProjectManagement.Dto;
using Finance.PropertyDepartment.Entering.Method;
using Finance.TradeCompliance;
using Finance.WorkFlows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Asn1.Ocsp;
using Rougamo;
using Spire.Xls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using test;

namespace Finance.PriceEval
{
    /// <summary>
    /// 核价服务
    /// </summary>
    //[ParameterValidator]
    public class PriceEvaluationAppService : PriceEvaluationGetAppService
    {
        #region 类初始化

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<PriceEvaluationStartData, long> _priceEvaluationStartDataRepository;
        private readonly IRepository<NodeInstance, long> _nodeInstanceRepository;

        private readonly IRepository<ProductInformation, long> _productInformationRepository;
        private readonly IRepository<Department, long> _departmentRepository;
        private readonly NrePricingAppService _nrePricingAppService;
        private readonly IRepository<AuditFlow, long> _auditFlowRepository;
        private readonly IRepository<FileManagement, long> _fileManagementRepository;
        private readonly IRepository<CustomerTargetPrice, long> _customerTargetPriceRepository;
        private readonly IRepository<Sample, long> _sampleRepository;


        private readonly IRepository<Gradient, long> _gradientRepository;
        private readonly IRepository<GradientModel, long> _gradientModelRepository;
        private readonly IRepository<GradientModelYear, long> _gradientModelYearRepository;
        private readonly IRepository<ShareCount, long> _shareCountRepository;

        //protected readonly IRepository<Gradient, long> _gradientRepository;
        //protected readonly IRepository<GradientModel, long> _gradientModelRepository;
        //protected readonly IRepository<GradientModelYear, long> _gradientModelYearRepository;

        private readonly IRepository<CarModelCount, long> _carModelCountRepository;
        private readonly IRepository<CarModelCountYear, long> _carModelCountYearRepository;

        //private readonly IRepository<EditItem, long> _editItemRepository;
        private readonly IRepository<UpdateItem, long> _updateItemRepository;


        private readonly WorkflowInstanceAppService _workflowInstanceAppService;

        private readonly IRepository<CountryLibrary, long> _countryLibraryRepository;

        /// <summary>
        ///  零件是否全部录入 依据实体类
        /// </summary>
        private readonly IRepository<NreIsSubmit, long> _productIsSubmit;

        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;

        private readonly IRepository<AfterUpdateSumInfo, long> _afterUpdateSumInfoRepository;


        public PriceEvaluationAppService(ICacheManager cacheManager, IRepository<NodeInstance, long> nodeInstanceRepository, IRepository<PriceEvaluationStartData, long> priceEvaluationStartDataRepository, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<Pcs, long> pcsRepository, IRepository<PcsYear, long> pcsYearRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<Requirement, long> requirementRepository, IRepository<ElectronicBomInfo, long> electronicBomInfoRepository, IRepository<StructureBomInfo, long> structureBomInfoRepository,
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
            IRepository<UPHInfo, long> uphInfoRepository, IRepository<AllManufacturingCost, long> allManufacturingCostRepository,
            IRepository<ProductInformation, long> productInformationRepository, IRepository<Department, long> departmentRepository, NrePricingAppService nrePricingAppService, IRepository<AuditFlow, long> auditFlowRepository, IRepository<FileManagement, long> fileManagementRepository, AuditFlowAppService flowAppService, IRepository<NreIsSubmit, long> productIsSubmit,
            IRepository<CustomerTargetPrice, long> customerTargetPriceRepository, IRepository<Sample, long> sampleRepository,
            IRepository<Gradient, long> gradientRepository, IRepository<GradientModel, long> gradientModelRepository,
            IRepository<GradientModelYear, long> gradientModelYearRepository, IRepository<ShareCount, long> shareCountRepository,
           IRepository<CarModelCount, long> carModelCountRepository, IRepository<CarModelCountYear, long> carModelCountYearRepository,
           WorkflowInstanceAppService workflowInstanceAppService, IRepository<UpdateItem, long> updateItemRepository, IRepository<Solution, long> solutionRepository, IRepository<BomEnter, long> bomEnterRepository,
           IRepository<CountryLibrary, long> countryLibraryRepository, IRepository<BomEnterTotal, long> bomEnterTotalRepository, IRepository<Logisticscost, long> logisticscostRepository,
           IRepository<QualityCostRatio, long> qualityCostRatioRepository, IRepository<QualityCostRatioYear, long> qualityCostRatioYearRepository, IRepository<FollowLineTangent, long> followLineTangentRepository,
           IRepository<ProcessHoursEnterUph, long> processHoursEnterUphRepository,
           IRepository<ProcessHoursEnterDevice, long> processHoursEnterDeviceRepository,
           IRepository<ProcessHoursEnter, long> processHoursEnterRepository,
           IRepository<PanelJson, long> panelJsonRepository,
           IRepository<AfterUpdateSumInfo, long> afterUpdateSumInfoRepository,
           IRepository<BomMaterial, long> bomMaterialRepository,
           IRepository<Fu_Bom, long> fu_BomRepository,
          IRepository<Fu_ManufacturingCost, long> fu_ManufacturingCostRepository,
          IRepository<Fu_LossCost, long> fu_LossCostRepository,
         IRepository<Fu_OtherCostItem2, long> fu_OtherCostItem2Repository,
         IRepository<Fu_OtherCostItem, long> fu_OtherCostItemRepository,
         IRepository<Fu_QualityCostListDto, long> fu_QualityCostListDtoRepository,
         IRepository<Fu_LogisticsCost, long> fu_LogisticsCostRepository)
            : base(financeDictionaryDetailRepository, priceEvaluationRepository, pcsRepository, pcsYearRepository, modelCountRepository, modelCountYearRepository, requirementRepository, electronicBomInfoRepository, structureBomInfoRepository, enteringElectronicRepository, structureElectronicRepository, lossRateInfoRepository, lossRateYearInfoRepository, exchangeRateRepository, manufacturingCostInfoRepository, yearInfoRepository, workingHoursInfoRepository, rateEntryInfoRepository, productionControlInfoRepository, qualityCostProportionEntryInfoRepository, userInputInfoRepository, qualityCostProportionYearInfoRepository, uphInfoRepository, allManufacturingCostRepository,
                  gradientRepository, gradientModelRepository, gradientModelYearRepository, updateItemRepository, solutionRepository, bomEnterRepository, bomEnterTotalRepository, nrePricingAppService, shareCountRepository, logisticscostRepository,
                  qualityCostRatioRepository, qualityCostRatioYearRepository, customerTargetPriceRepository, followLineTangentRepository, processHoursEnterUphRepository,
                  processHoursEnterDeviceRepository, processHoursEnterRepository, panelJsonRepository, bomMaterialRepository,
                  fu_BomRepository,
         fu_ManufacturingCostRepository,
          fu_LossCostRepository,
          fu_OtherCostItem2Repository,
          fu_OtherCostItemRepository,
          fu_QualityCostListDtoRepository,
        fu_LogisticsCostRepository, nodeInstanceRepository)
        {
            _cacheManager = cacheManager;
            _nodeInstanceRepository = nodeInstanceRepository;
            _priceEvaluationStartDataRepository = priceEvaluationStartDataRepository;
            _productInformationRepository = productInformationRepository;
            _departmentRepository = departmentRepository;
            _nrePricingAppService = nrePricingAppService;
            _auditFlowRepository = auditFlowRepository;
            _fileManagementRepository = fileManagementRepository;
            _flowAppService = flowAppService;
            _productIsSubmit = productIsSubmit;
            _customerTargetPriceRepository = customerTargetPriceRepository;
            _sampleRepository = sampleRepository;

            _gradientRepository = gradientRepository;
            _gradientModelRepository = gradientModelRepository;
            _gradientModelYearRepository = gradientModelYearRepository;
            _shareCountRepository = shareCountRepository;

            _carModelCountRepository = carModelCountRepository;
            _carModelCountYearRepository = carModelCountYearRepository;

            _workflowInstanceAppService = workflowInstanceAppService;
            _updateItemRepository = updateItemRepository;

            _countryLibraryRepository = countryLibraryRepository;
            _afterUpdateSumInfoRepository = afterUpdateSumInfoRepository;
        }



        #endregion

        /// <summary>
        /// 手动刷新国家类型
        /// </summary>
        /// <returns></returns>
        private async Task RRRRRRRRRRRRRRRRRRRRR()
        {
            var data = await _priceEvaluationRepository.GetAllListAsync();
            foreach (var item in data)
            {
                var myhg = await (from d in _financeDictionaryDetailRepository.GetAll()
                                  join c in _countryLibraryRepository.GetAll() on d.DisplayName equals c.Country
                                  where d.Id == item.Country || c.NationalType == "二级管制国家"
                                  select new
                                  {
                                      c.Id,
                                      c.NationalType,
                                      DictionaryId = d.Id
                                  }).ToListAsync();
                var myhggj = myhg.FirstOrDefault(p => p.DictionaryId == item.Country);
                var countryLibraryId = myhggj == null ? myhg.FirstOrDefault().Id : myhggj.Id;
                item.CountryLibraryId = countryLibraryId;
                await _priceEvaluationRepository.UpdateAsync(item);
            }
        }

        #region 核价开始
        /// <summary>
        /// 开始核价：报价核价需求录入界面（第一步）：保存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async virtual Task<PriceEvaluationStartResult> PriceEvaluationStartSave(PriceEvaluationStartSaveInput input)
        {
            //if (!input.IsSubmit)
            //{
            long auid;
            if (input.NodeInstanceId == default)
            {
                auid = await _workflowInstanceAppService.StartWorkflowInstance(new WorkFlows.Dto.StartWorkflowInstanceInput
                {
                    WorkflowId = WorkFlowCreator.MainFlowId,
                    Title = input.Title,
                    FinanceDictionaryDetailId = FinanceConsts.Save,
                });
            }
            else
            {
                var nodeInstance = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.Id == input.NodeInstanceId);
                auid = nodeInstance.WorkFlowInstanceId;
            }


            var json = JsonConvert.SerializeObject(input);

            var priceEvaluationStartData = await _priceEvaluationStartDataRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auid);
            if (priceEvaluationStartData is null)
            {
                await _priceEvaluationStartDataRepository.InsertAsync(new PriceEvaluationStartData
                {
                    AuditFlowId = auid,
                    DataJson = json
                });
            }
            else
            {
                priceEvaluationStartData.DataJson = json;
            }

            return new PriceEvaluationStartResult { AuditFlowId = auid, IsSuccess = true, Message = "添加成功！" };
            //}
        }

        /// <summary>
        /// 开始核价：报价核价需求录入界面（第一步）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async virtual Task<PriceEvaluationStartResult> PriceEvaluationStart(PriceEvaluationStartInput input)
        {

            #region 流程防抖

            var cacheJson = JsonConvert.SerializeObject(input);
            var code = cacheJson.GetHashCode().ToString();

            var cache = await _cacheManager.GetCache("PriceEvaluationStartInput").GetOrDefaultAsync(code);
            if (cache is null)
            {
                await _cacheManager.GetCache("PriceEvaluationStartInput").SetAsync(code, code, new TimeSpan(24, 0, 0));

            }
            else
            {
                throw new FriendlyException($"您重复提交了流程！");
            }

            #endregion

            #region 通用参数校验

            if (input.ProjectCycle > 8)
            {
                throw new FriendlyException($"项目周期不得大于8年！");
            }

            var isProductInformation = input.ProductInformation.GroupBy(p => p.Product).Any(p => p.Count() > 1);
            if (isProductInformation)
            {
                throw new FriendlyException($"产品信息有重复的零件名称！");
            }

            var modelMinYeay = input.ModelCount?.SelectMany(p => p.ModelCountYearList).Min(p => p.Year);
            var pcsMinYeay = input.Pcs?.SelectMany(p => p.PcsYearList).Min(p => p.Year);
            var requirementMinYeay = input?.Requirement.Min(p => p.Year);


            if ((modelMinYeay is not null && modelMinYeay < input.SopTime) || (pcsMinYeay is not null && pcsMinYeay < input.SopTime) || (requirementMinYeay is not null && requirementMinYeay < input.SopTime))
            {
                throw new FriendlyException($"SOP年份和实际录入的模组数量、产品信息、PCS不吻合！");
            }


            if (input.ModelCount.GroupBy(p => p.Product).Any(p => p.Count() > 1))
            {

                throw new FriendlyException($"模组数量合计有重复的产品名称！");
            }

            //if (input.GradientModel.GroupBy(p => p.Name).Any(p => p.Count() > 1))
            //{

            //    throw new FriendlyException($"梯度模组有重复的产品名称！");
            //}

            if (input.ModelCount.Any(p => p.Product.IsNullOrEmpty()))
            {
                throw new FriendlyException($"模组数量合计有为空的产品名称！");
            }

            if (input.GradientModel.Any(p => p.Name.IsNullOrEmpty()))
            {
                throw new FriendlyException($"梯度模组有为空的产品名称！");
            }


            if (input.ModelCount.GroupBy(p => p.PartNumber).Any(p => p.Count() > 1))
            {

                throw new FriendlyException($"模组数量合计有重复的客户零件号！");
            }

            if (input.CustomerTargetPrice.Any(p => p.Currency == 0) || input.CustomerTargetPrice.Any(p => p.ExchangeRate == 0))
            {
                throw new FriendlyException($"需要填写客户目标价的汇率和币种");
            }

            if (input.Pcs.GroupBy(p => new { p.CarFactory, p.CarModel, p.PcsType }).Count() != input.Pcs.Count)
            {
                throw new FriendlyException($"终端走量的车厂车型不能完全相同！");
            }

            if (input.SopTime < DateTime.Now.Year)
            {
                throw new FriendlyException($"SOP年份不能小于当年年份！");
            }

            if (input.Gradient == null || input.Gradient.Count == 0 || input.Gradient.Any(p => p.GradientValue == 0))
            {
                throw new FriendlyException($"梯度数量不能为0！");
            }

            if (input.Gradient.GroupBy(p => p.GradientValue).Any(p => p.Count() > 1))
            {
                throw new FriendlyException($"梯度数量不能重复！");
            }

            #endregion

            if (!input.IsSubmit)
            {
                long auid;
                if (input.NodeInstanceId == default)
                {
                    auid = await _workflowInstanceAppService.StartWorkflowInstance(new WorkFlows.Dto.StartWorkflowInstanceInput
                    {
                        WorkflowId = WorkFlowCreator.MainFlowId,
                        Title = input.Title,
                        FinanceDictionaryDetailId = FinanceConsts.Save,
                    });
                }
                else
                {
                    var nodeInstance = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.Id == input.NodeInstanceId);
                    auid = nodeInstance.WorkFlowInstanceId;
                }


                var json = JsonConvert.SerializeObject(input);

                var priceEvaluationStartData = await _priceEvaluationStartDataRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auid);
                if (priceEvaluationStartData is null)
                {
                    await _priceEvaluationStartDataRepository.InsertAsync(new PriceEvaluationStartData
                    {
                        AuditFlowId = auid,
                        DataJson = json
                    });
                }
                else
                {
                    priceEvaluationStartData.DataJson = json;
                }

                return new PriceEvaluationStartResult { AuditFlowId = auid, IsSuccess = true, Message = "添加成功！" };
            }
            else
            {
                #region 参数校验

                if (input.Opinion.IsNullOrWhiteSpace())
                {
                    input.Opinion = FinanceConsts.EvalReason_Fabg;
                }

                if (input.CountryType.IsNullOrWhiteSpace())
                {
                    input.CountryType = "空";
                }


                //var myhg = await (from d in _financeDictionaryDetailRepository.GetAll()
                //                  join c in _countryLibraryRepository.GetAll() on d.DisplayName equals c.Country
                //                  where d.Id == input.Country || c.NationalType == "2"
                //                  select new
                //                  {
                //                      c.Id,
                //                      c.NationalType,
                //                      DictionaryId = d.Id
                //                  }).ToListAsync();
                //var myhggj = myhg.FirstOrDefault(p => p.DictionaryId == input.Country);

                var country = await _countryLibraryRepository.FirstOrDefaultAsync(p => p.Country == input.Country);
                var otherCountry = await _countryLibraryRepository.FirstOrDefaultAsync(p => p.Country == "其他国家");


                long countryLibraryId;
                if (country == null)
                {
                    if (otherCountry == null)
                    {
                        throw new FriendlyException($"国家库不存在此国家，也没有【其他国家】");
                    }
                    countryLibraryId = otherCountry.Id;
                }
                else
                {
                    countryLibraryId = country.Id;
                }


                long auditFlowId;

                if (!input.SorFile.Any())
                {
                    throw new FriendlyException($"SOR文件没有上传！");
                }

                #endregion



                //PriceEvaluation
                var priceEvaluation = ObjectMapper.Map<PriceEvaluation>(input);
                priceEvaluation.CountryLibraryId = countryLibraryId;

                var user = await UserManager.GetUserByIdAsync(AbpSession.UserId.Value);

                var department = await _departmentRepository.FirstOrDefaultAsync(user.DepartmentId);

                if (department is not null)
                {
                    priceEvaluation.DraftingCompanyId = department.CompanyId;
                }
                priceEvaluation.DraftingDepartmentId = user.DepartmentId;

                //long flowId = await _flowAppService.SavaNewAuditFlowInfo(new Audit.Dto.AuditFlowDto()
                //{
                //    QuoteProjectName = input.ProjectName,
                //    QuoteProjectNumber = input.ProjectCode
                //});
                //auditFlowId = flowId;
                if (input.NodeInstanceId == default)
                {
                    auditFlowId = await _workflowInstanceAppService.StartWorkflowInstance(new WorkFlows.Dto.StartWorkflowInstanceInput
                    {
                        WorkflowId = WorkFlowCreator.MainFlowId,
                        Title = input.Title,
                        FinanceDictionaryDetailId = input.Opinion
                    });
                }
                else
                {



                    var nodeInstance = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.Id == input.NodeInstanceId);
                    auditFlowId = nodeInstance.WorkFlowInstanceId;

                    await _workflowInstanceAppService.SubmitNode(new WorkFlows.Dto.SubmitNodeInput
                    {
                        FinanceDictionaryDetailId = input.Opinion,
                        Comment = input.Comment,
                        NodeInstanceId = input.NodeInstanceId,
                    });


                    #region 清空数据

                    var priceEvaluationEntity = await _priceEvaluationRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in priceEvaluationEntity)
                    {
                        await _priceEvaluationRepository.DeleteAsync(item);
                    }

                    var sampleEntity = await _sampleRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in sampleEntity)
                    {
                        await _sampleRepository.DeleteAsync(item);
                    }

                    var pcsEntity = await _pcsRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in pcsEntity)
                    {
                        await _pcsRepository.DeleteAsync(item);
                    }

                    var pcsYearEntity = await _pcsYearRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in pcsYearEntity)
                    {
                        await _pcsYearRepository.DeleteAsync(item);
                    }

                    var carModelCountEntity = await _carModelCountRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in carModelCountEntity)
                    {
                        await _carModelCountRepository.DeleteAsync(item);
                    }

                    var carModelCountYearEntity = await _carModelCountYearRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in carModelCountYearEntity)
                    {
                        await _carModelCountYearRepository.DeleteAsync(item);
                    }

                    var modelCountEntity = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in modelCountEntity)
                    {
                        await _modelCountRepository.DeleteAsync(item);
                    }

                    var modelCountYearEntity = await _modelCountYearRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in modelCountYearEntity)
                    {
                        await _modelCountYearRepository.DeleteAsync(item);
                    }

                    var requirementEntity = await _requirementRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in requirementEntity)
                    {
                        await _requirementRepository.DeleteAsync(item);
                    }

                    var productInformationEntity = await _productInformationRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in productInformationEntity)
                    {
                        await _productInformationRepository.DeleteAsync(item);
                    }

                    var customerTargetPriceEntity = await _customerTargetPriceRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in customerTargetPriceEntity)
                    {
                        await _customerTargetPriceRepository.DeleteAsync(item);
                    }

                    var gradientEntity = await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in gradientEntity)
                    {
                        await _gradientRepository.DeleteAsync(item);
                    }

                    var gradientModelEntity = await _gradientModelRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in gradientModelEntity)
                    {
                        await _gradientModelRepository.DeleteAsync(item);
                    }

                    var gradientModelYearEntity = await _gradientModelYearRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in gradientModelYearEntity)
                    {
                        await _gradientModelYearRepository.DeleteAsync(item);
                    }

                    var shareCountEntity = await _shareCountRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
                    foreach (var item in shareCountEntity)
                    {
                        await _shareCountRepository.DeleteAsync(item);
                    }
                    #endregion
                }


                priceEvaluation.AuditFlowId = auditFlowId;
                var priceEvaluationId = await _priceEvaluationRepository.InsertAndGetIdAsync(priceEvaluation);


                //Sample
                var samples = ObjectMapper.Map<List<Sample>>(input.Sample);
                foreach (var sample in samples)
                {
                    sample.PriceEvaluationId = priceEvaluationId;
                    sample.AuditFlowId = auditFlowId;
                    await _sampleRepository.InsertAsync(sample);
                }


                //Pcs
                foreach (var createPcsDto in input.Pcs)
                {
                    var pcs = ObjectMapper.Map<Pcs>(createPcsDto);
                    pcs.PriceEvaluationId = priceEvaluationId;
                    pcs.AuditFlowId = auditFlowId;
                    var pcsId = await _pcsRepository.InsertAndGetIdAsync(pcs);
                    //Pcs 年份
                    foreach (var createPcsYearDto in createPcsDto.PcsYearList)
                    {
                        var pcsYear = ObjectMapper.Map<PcsYear>(createPcsYearDto);
                        pcsYear.AuditFlowId = auditFlowId;
                        pcsYear.PcsId = pcsId;
                        await _pcsYearRepository.InsertAsync(pcsYear);
                    }
                }


                //车型下的模组数量
                foreach (var createCarModelCountDto in input.CarModelCount)
                {
                    var carModelCount = ObjectMapper.Map<CarModelCount>(createCarModelCountDto);
                    carModelCount.PriceEvaluationId = priceEvaluationId;
                    carModelCount.AuditFlowId = auditFlowId;
                    var productId = await _carModelCountRepository.InsertAndGetIdAsync(carModelCount);

                    foreach (var createModelCountYearDto in createCarModelCountDto.ModelCountYearList)
                    {
                        var carModelCountYear = ObjectMapper.Map<CarModelCountYear>(createModelCountYearDto);
                        carModelCountYear.AuditFlowId = auditFlowId;
                        carModelCountYear.CarModelCountId = productId;
                        await _carModelCountYearRepository.InsertAsync(carModelCountYear);
                    }
                }

                var modelCountIds = new List<(string number, string product, long productId)>();

                //模组数量合计
                foreach (var createModelCountDto in input.ModelCount)
                {
                    var modelCount = ObjectMapper.Map<ModelCount>(createModelCountDto);
                    modelCount.PriceEvaluationId = priceEvaluationId;
                    modelCount.AuditFlowId = auditFlowId;
                    var productId = await _modelCountRepository.InsertAndGetIdAsync(modelCount);

                    modelCountIds.Add((createModelCountDto.PartNumber, createModelCountDto.Product, productId));

                    foreach (var createModelCountYearDto in createModelCountDto.ModelCountYearList)
                    {
                        var modelCountYear = ObjectMapper.Map<ModelCountYear>(createModelCountYearDto);
                        modelCountYear.ProductId = productId;
                        modelCountYear.AuditFlowId = auditFlowId;
                        await _modelCountYearRepository.InsertAsync(modelCountYear);
                    }
                }

                //要求
                var requirements = ObjectMapper.Map<List<Requirement>>(input.Requirement);
                foreach (var requirement in requirements)
                {
                    requirement.PriceEvaluationId = priceEvaluationId;
                    requirement.AuditFlowId = auditFlowId;
                    await _requirementRepository.InsertAsync(requirement);
                }

                //产品信息
                var productInformations = ObjectMapper.Map<List<ProductInformation>>(input.ProductInformation);
                foreach (var productInformation in productInformations)
                {
                    productInformation.PriceEvaluationId = priceEvaluationId;
                    productInformation.AuditFlowId = auditFlowId;
                    await _productInformationRepository.InsertAsync(productInformation);
                }


                //客户目标价
                var customerTargetPrices = ObjectMapper.Map<List<CustomerTargetPrice>>(input.CustomerTargetPrice);
                foreach (var customerTargetPrice in customerTargetPrices)
                {
                    customerTargetPrice.PriceEvaluationId = priceEvaluationId;
                    customerTargetPrice.AuditFlowId = auditFlowId;
                    customerTargetPrice.ProductId = modelCountIds.First(p => p.product == customerTargetPrice.Product).productId;
                    await _customerTargetPriceRepository.InsertAsync(customerTargetPrice);
                }

                //梯度
                var gradients = ObjectMapper.Map<List<Gradient>>(input.Gradient);
                var gradientIds = new List<(long id, decimal gradientValue)>();
                foreach (var gradient in gradients)
                {
                    gradient.PriceEvaluationId = priceEvaluationId;
                    gradient.AuditFlowId = auditFlowId;
                    var id = await _gradientRepository.InsertAndGetIdAsync(gradient);
                    //gradientIds.Add((id, gradient.GradientValue));
                    gradientIds.Add((id, gradient.GradientValue));
                }

                //梯度模组
                foreach (var gradientModel in input.GradientModel)
                {
                    var entity = ObjectMapper.Map<GradientModel>(gradientModel);
                    entity.PriceEvaluationId = priceEvaluationId;
                    entity.AuditFlowId = auditFlowId;
                    entity.GradientId = gradientIds.First(p => p.gradientValue == gradientModel.GradientValue).id;
                    //entity.ProductId = modelCountIds.First(p => p.number == gradientModel.Number).productId;
                    entity.ProductId = modelCountIds.First(p => p.product == gradientModel.Name).productId;

                    var gradientModelId = await _gradientModelRepository.InsertAndGetIdAsync(entity);
                    var gradientModelYears = ObjectMapper.Map<List<GradientModelYear>>(gradientModel.GradientModelYear);
                    foreach (var gradient in gradientModelYears)
                    {
                        gradient.PriceEvaluationId = priceEvaluationId;
                        gradient.AuditFlowId = auditFlowId;
                        gradient.GradientModelId = gradientModelId;
                        //gradient.ProductId = modelCountIds.First(p => p.number == gradientModel.Number).productId;
                        gradient.ProductId = modelCountIds.First(p => p.product == gradientModel.Name).productId;

                        await _gradientModelYearRepository.InsertAsync(gradient);
                    }
                }

                //分摊数量
                var shareCounts = ObjectMapper.Map<List<ShareCount>>(input.ShareCount);
                foreach (var shareCount in shareCounts)
                {
                    shareCount.PriceEvaluationId = priceEvaluationId;
                    shareCount.AuditFlowId = auditFlowId;
                    shareCount.ProductId = modelCountIds.First(p => p.product == shareCount.Name).productId;
                    await _shareCountRepository.InsertAsync(shareCount);
                }

                //_flowAppService.SavaProjectManagerInfo(input.ProjectManager);
                //await _flowAppService.UpdateAuditFlowInfo(new AuditFlowDetailDto()
                //{
                //    AuditFlowId = auditFlowId,
                //    ProcessIdentifier = AuditFlowConsts.AF_RequirementInput,
                //    UserId = user.Id,
                //    Opinion = OPINIONTYPE.Submit_Agreee
                //});
                return new PriceEvaluationStartResult { AuditFlowId = auditFlowId, IsSuccess = true, Message = "添加成功！" };
            }
        }

        /// <summary>
        /// 获取【PriceEvaluationStart】（开始核价：报价核价需求录入界面（第一步））接口输入的数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async virtual Task<PriceEvaluationStartInputResult> GetPriceEvaluationStartData(long auditFlowId)
        {
            var priceEvaluationStartData1 = await _priceEvaluationStartDataRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
            if (priceEvaluationStartData1 is not null)
            {
                var result = JsonConvert.DeserializeObject<PriceEvaluationStartInputResult>(priceEvaluationStartData1.DataJson);

                var fileNames1 = await _fileManagementRepository.GetAll().Where(p => result.SorFile.Contains(p.Id))
                .Select(p => new FileUploadOutputDto { FileId = p.Id, FileName = p.Name, })
                .ToListAsync();
                result.Files = fileNames1;

                foreach (var item in result.ModelCount)
                {
                    var dictionaryDetail = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == item.ProductType);
                    item.ProductTypeName = dictionaryDetail.DisplayName;
                }


                return result;
            }

            var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
            var priceEvaluationDto = ObjectMapper.Map<PriceEvaluationStartInputResult>(priceEvaluation);

            //var quoteVersion = await _auditFlowRepository.GetAll().Where(p => p.Id == auditFlowId).Select(p => p.QuoteVersion).FirstOrDefaultAsync();
            var quoteVersion = priceEvaluationDto.QuoteVersion;


            var fileNames = await _fileManagementRepository.GetAll().Where(p => priceEvaluationDto.SorFile.Contains(p.Id))
                .Select(p => new FileUploadOutputDto { FileId = p.Id, FileName = p.Name, })
                .ToListAsync();

            var pcsDto = (await _pcsRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
                   .Join(_pcsYearRepository.GetAll(), p => p.Id, p => p.PcsId, (pcs, pcstYear) => new { pcs, pcstYear }).ToListAsync()).GroupBy(p => p.pcs).Select(p =>
                   {
                       var dto = ObjectMapper.Map<CreatePcsDto>(p.Key);
                       dto.PcsYearList = ObjectMapper.Map<List<CreatePcsYearDto>>(p.Select(o => o.pcstYear));
                       return dto;
                   }).ToList();

            var modelCountDto = (await _modelCountRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
                   .Join(_modelCountYearRepository.GetAll(), p => p.Id, p => p.ProductId, (modelCount, modelCountYear) => new { modelCount, modelCountYear }).ToListAsync()).GroupBy(p => p.modelCount).Select(p =>
                   {
                       var dto = ObjectMapper.Map<CreateModelCountDto>(p.Key);
                       dto.ModelCountYearList = ObjectMapper.Map<List<CreateModelCountYearDto>>(p.Select(o => o.modelCountYear));
                       return dto;
                   }).ToList();

            var carModelCountDto = (await _carModelCountRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
                   .Join(_carModelCountYearRepository.GetAll(), p => p.Id, p => p.CarModelCountId, (carModelCount, carModelCountYear) => new { carModelCount, carModelCountYear }).ToListAsync()).GroupBy(p => p.carModelCount).Select(p =>
                   {
                       var dto = ObjectMapper.Map<CreateCarModelCountDto>(p.Key);
                       dto.ModelCountYearList = ObjectMapper.Map<List<CreateCarModelCountYearDto>>(p.Select(o => o.carModelCountYear));
                       return dto;
                   }).ToList();

            var requirementDto = ObjectMapper.Map<List<CreateRequirementDto>>(await _requirementRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId));


            var productInformation = await _productInformationRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            var productInformationDto = ObjectMapper.Map<List<CreateColumnFormatProductInformationDto>>(productInformation);

            var customerTargetPrice = await _customerTargetPriceRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            var customerTargetPriceDto = ObjectMapper.Map<List<CreateCustomerTargetPriceDto>>(customerTargetPrice);

            var sample = await _sampleRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            var sampleDto = ObjectMapper.Map<List<CreateSampleDto>>(sample);

            var gradients = await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            var gradientsDto = ObjectMapper.Map<List<GradientInput>>(gradients);


            var shareCounts = await _shareCountRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            var shareCountsDto = ObjectMapper.Map<List<ShareCountInput>>(shareCounts);

            var gradient = await _gradientRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);

            var gradientModelDto = (await _gradientModelRepository.GetAll().Where(p => p.AuditFlowId == auditFlowId)
                   .Join(_gradientModelYearRepository.GetAll(), p => p.Id, p => p.GradientModelId, (gradientModel, gradientModelYear) => new { gradientModel, gradientModelYear }).ToListAsync()).GroupBy(p => p.gradientModel).Select(p =>
                   {
                       var dto = ObjectMapper.Map<GradientModelInput>(p.Key);
                       dto.GradientValue = gradient.FirstOrDefault(o => o.Id == p.Key.GradientId).GradientValue;
                       dto.GradientModelYear = ObjectMapper.Map<List<GradientModelYearInput>>(p.Select(o => o.gradientModelYear));
                       return dto;
                   }).ToList();

            priceEvaluationDto.Pcs = pcsDto;
            priceEvaluationDto.ModelCount = modelCountDto;
            priceEvaluationDto.CarModelCount = carModelCountDto;
            priceEvaluationDto.Requirement = requirementDto;
            priceEvaluationDto.ProductInformation = productInformationDto;
            priceEvaluationDto.CustomerTargetPrice = customerTargetPriceDto;
            priceEvaluationDto.Sample = sampleDto;

            priceEvaluationDto.Gradient = gradientsDto;
            priceEvaluationDto.ShareCount = shareCountsDto;
            priceEvaluationDto.GradientModel = gradientModelDto;

            priceEvaluationDto.QuoteVersion = quoteVersion;
            priceEvaluationDto.Files = fileNames;

            foreach (var item in modelCountDto)
            {
                var dictionaryDetail = await _financeDictionaryDetailRepository.FirstOrDefaultAsync(p => p.Id == item.ProductType);
                item.ProductTypeName = dictionaryDetail.DisplayName;
            }

            //var node = await _nodeInstanceRepository
            //    .FirstOrDefaultAsync(p => p.WorkFlowInstanceId == auditFlowId && p.NodeId == "主流程_核价需求录入");

            //priceEvaluationDto.NodeInstanceId = node.Id;
            //priceEvaluationDto.Opinion = priceEvaluation.Opinion;

            return priceEvaluationDto;
        }

        #region 修改项




        /// <summary>
        /// 设置修改项（物料成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task SetUpdateItemMaterial(SetUpdateItemInput<List<Material>> input)
        {
            var entity = await _updateItemRepository.GetAll()
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                && p.UpdateItemType == UpdateItemType.Material
                && p.GradientId == input.GradientId && p.SolutionId == input.SolutionId && p.Year == input.Year && p.UpDown == input.UpDown);
            if (entity is null)
            {
                var data = ObjectMapper.Map<UpdateItem>(input);
                data.UpdateItemType = UpdateItemType.Material;
                data.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);
                await _updateItemRepository.InsertAsync(data);
            }
            else
            {
                entity.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                entity.File = input.File;

                await _updateItemRepository.UpdateAsync(entity);
                //ObjectMapper.Map(input, entity);
            }
        }

        /// <summary>
        /// 获取修改项（物料成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<Material>> GetUpdateItemMaterial(GetUpdateItemInput input)
        {
            var entity = await _updateItemRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
            && p.UpdateItemType == UpdateItemType.Material
            //&& p.ProductId == input.ProductId
            && p.GradientId == input.GradientId
            && p.SolutionId == input.SolutionId
            && p.Year == input.Year
            && p.UpDown == input.UpDown);
            if (entity is null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<List<Material>>(entity.MaterialJson);
        }

        /// <summary>
        /// 设置修改项（损耗成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task SetUpdateItemLossCost(SetUpdateItemInput<List<LossCost>> input)
        {
            if (input.UpdateItem.Any(p => p.EditNotes.IsNullOrWhiteSpace()))
            {
                throw new FriendlyException($"必须填写修改备注！");
            }

            var entity = await _updateItemRepository.GetAll()
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                && p.UpdateItemType == UpdateItemType.LossCost
                && p.GradientId == input.GradientId && p.SolutionId == input.SolutionId && p.Year == input.Year && p.UpDown == input.UpDown);
            if (entity is null)
            {
                var data = ObjectMapper.Map<UpdateItem>(input);
                data.UpdateItemType = UpdateItemType.LossCost;
                data.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                await _updateItemRepository.InsertAsync(data);
            }
            else
            {
                entity.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                entity.File = input.File;
                //ObjectMapper.Map(input, entity);
                await _updateItemRepository.UpdateAsync(entity);
            }
        }

        /// <summary>
        /// 获取修改项（损耗成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<LossCost>> GetUpdateItemLossCost(GetUpdateItemInput input)
        {
            var entity = await _updateItemRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
           && p.UpdateItemType == UpdateItemType.LossCost
           //&& p.ProductId == input.ProductId
           && p.GradientId == input.GradientId
           && p.SolutionId == input.SolutionId
           && p.Year == input.Year
           && p.UpDown == input.UpDown);
            if (entity is null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<List<LossCost>>(entity.MaterialJson);

        }

        /// <summary>
        /// 设置修改项（制造成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task SetUpdateItemManufacturingCost(SetUpdateItemInput<List<ManufacturingCost>> input)
        {
            if (input.UpdateItem.Any(p => p.CostType == CostType.Total))
            {
                throw new FriendlyException($"制造成本合计不允许修改！");
            }
            if (input.Year == PriceEvalConsts.AllYear && input.UpdateItem.Any(p => p.CostType == CostType.GroupTest))
            {
                throw new FriendlyException($"组测成本的全生命周期数据不允许修改！");
            }

            if (input.UpdateItem.Any(p => p.EditNotes.IsNullOrWhiteSpace()))
            {
                throw new FriendlyException($"必须填写修改备注！");
            }

            var entity = await _updateItemRepository.GetAll()
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                && p.UpdateItemType == UpdateItemType.ManufacturingCost
                && p.GradientId == input.GradientId && p.SolutionId == input.SolutionId && p.Year == input.Year && p.UpDown == input.UpDown);
            if (entity is null)
            {
                var data = ObjectMapper.Map<UpdateItem>(input);
                data.UpdateItemType = UpdateItemType.ManufacturingCost;
                data.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                await _updateItemRepository.InsertAsync(data);
            }
            else
            {
                entity.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                entity.File = input.File;
                //ObjectMapper.Map(input, entity);
                await _updateItemRepository.UpdateAsync(entity);
            }
        }

        /// <summary>
        /// 获取修改项（制造成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<ManufacturingCost>> GetUpdateItemManufacturingCost(GetUpdateItemInput input)
        {
            var entity = await _updateItemRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
          && p.UpdateItemType == UpdateItemType.ManufacturingCost
          //&& p.ProductId == input.ProductId
          && p.GradientId == input.GradientId
          && p.SolutionId == input.SolutionId
          && p.Year == input.Year
          && p.UpDown == input.UpDown);
            if (entity is null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<List<ManufacturingCost>>(entity.MaterialJson);

        }


        /// <summary>
        /// 设置修改项（物流成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task SetUpdateItemLogisticsCost(SetUpdateItemInput<List<ProductionControlInfoListDto>> input)
        {
            if (input.Year == PriceEvalConsts.AllYear)
            {
                throw new FriendlyException($"物流成本的全生命周期数据不允许修改！");
            }

            if (input.UpdateItem.Any(p => p.EditNotes.IsNullOrWhiteSpace()))
            {
                throw new FriendlyException($"必须填写修改备注！");
            }

            var entity = await _updateItemRepository.GetAll()
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                && p.UpdateItemType == UpdateItemType.LogisticsCost
                && p.GradientId == input.GradientId && p.SolutionId == input.SolutionId && p.Year == input.Year && p.UpDown == input.UpDown);
            if (entity is null)
            {
                var data = ObjectMapper.Map<UpdateItem>(input);
                data.UpdateItemType = UpdateItemType.LogisticsCost;
                data.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                await _updateItemRepository.InsertAsync(data);
            }
            else
            {
                entity.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                entity.File = input.File;
                //ObjectMapper.Map(input, entity);
                await _updateItemRepository.UpdateAsync(entity);
            }
        }

        /// <summary>
        /// 获取修改项（物流成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<ProductionControlInfoListDto>> GetUpdateItemLogisticsCost(GetUpdateItemInput input)
        {
            var entity = await _updateItemRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
        && p.UpdateItemType == UpdateItemType.LogisticsCost
        //&& p.ProductId == input.ProductId
        && p.GradientId == input.GradientId
        && p.SolutionId == input.SolutionId
        && p.Year == input.Year
        && p.UpDown == input.UpDown);
            if (entity is null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<List<ProductionControlInfoListDto>>(entity.MaterialJson);
        }

        /// <summary>
        /// 设置修改项（质量成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task SetUpdateItemQualityCost(SetUpdateItemInput<List<QualityCostListDto>> input)
        {
            if (input.Year == PriceEvalConsts.AllYear)
            {
                throw new FriendlyException($"质量成本的全生命周期数据不允许修改！");
            }

            var entity = await _updateItemRepository.GetAll()
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                && p.UpdateItemType == UpdateItemType.QualityCost
                && p.GradientId == input.GradientId && p.SolutionId == input.SolutionId && p.Year == input.Year && p.UpDown == input.UpDown);
            if (entity is null)
            {
                var data = ObjectMapper.Map<UpdateItem>(input);
                data.UpdateItemType = UpdateItemType.QualityCost;
                data.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                await _updateItemRepository.InsertAsync(data);
            }
            else
            {
                entity.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                entity.File = input.File;
                //ObjectMapper.Map(input, entity);
                await _updateItemRepository.UpdateAsync(entity);
            }
        }

        /// <summary>
        /// 获取修改项（质量成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<List<QualityCostListDto>> GetUpdateItemQualityCost(GetUpdateItemInput input)
        {
            var entity = await _updateItemRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
  && p.UpdateItemType == UpdateItemType.QualityCost
  //&& p.ProductId == input.ProductId
  && p.GradientId == input.GradientId
  && p.SolutionId == input.SolutionId
  && p.Year == input.Year
  && p.UpDown == input.UpDown);
            if (entity is null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<List<QualityCostListDto>>(entity.MaterialJson);

        }

        /// <summary>
        /// 设置修改项（其他成本）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task SetUpdateItemOtherCost(SetUpdateItemInput<List<OtherCostItem2List>> input)
        {
            if (input.Year == PriceEvalConsts.AllYear)
            {
                throw new FriendlyException($"其他成本的全生命周期数据不允许修改！");
            }

            if (input.UpdateItem.Any(p => p.Note.IsNullOrWhiteSpace()))
            {
                throw new FriendlyException($"必须填写修改备注！");
            }

            var entity = await _updateItemRepository.GetAll()
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                && p.UpdateItemType == UpdateItemType.OtherCostItem2List
                && p.GradientId == input.GradientId && p.SolutionId == input.SolutionId && p.Year == input.Year && p.UpDown == input.UpDown);
            if (entity is null)
            {
                var data = ObjectMapper.Map<UpdateItem>(input);
                data.UpdateItemType = UpdateItemType.OtherCostItem2List;
                data.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                await _updateItemRepository.InsertAsync(data);
            }
            else
            {
                entity.MaterialJson = JsonConvert.SerializeObject(input.UpdateItem);

                entity.File = input.File;
                //ObjectMapper.Map(input, entity);
                await _updateItemRepository.UpdateAsync(entity);
            }
        }


        #endregion

        #endregion

        #region 核价看板
        /// <summary>
        /// 核价看板-利润分布图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<ProportionOfProductCostListDto>> GetPricingPanelProfit(GetPricingPanelProfitInput input)
        {
            var data = await this.GetPriceEvaluationTable(new GetPriceEvaluationTableInput { AuditFlowId = input.AuditFlowId, GradientId = input.GradientId, InputCount = 0, SolutionId = input.SolutionId, Year = input.Year, UpDown = input.UpDown });

            //bom成本
            var bomCost = data.Material.Sum(p => p.TotalMoneyCynNoCustomerSupply);

            //损耗成本
            var costItemAll = data.Material.Sum(p => p.Loss);

            //制造成本
            var manufacturingCost = data.ManufacturingCost.FirstOrDefault(p => p.CostType == CostType.Total).Subtotal;

            //物流成本
            var logisticsFee = data.OtherCostItem.LogisticsFee;

            //质量成本
            var qualityCost = data.OtherCostItem.QualityCost;

            //其他成本
            //var other = data.OtherCostItem2.FirstOrDefault(p => p.ItemName == "单颗成本").Total.GetValueOrDefault();
            var other = data.OtherCostItem2.Where(p => p.ItemName == "单颗成本").Sum(p => p.Total).GetValueOrDefault();


            var sum = bomCost + costItemAll + manufacturingCost + logisticsFee + qualityCost + other;

            var list = new List<ProportionOfProductCostListDto>
            {
                new ProportionOfProductCostListDto{ Name="bom成本", Proportion= bomCost},
                new ProportionOfProductCostListDto{ Name="损耗成本", Proportion= costItemAll},
                new ProportionOfProductCostListDto{ Name="制造成本", Proportion= manufacturingCost},
                new ProportionOfProductCostListDto{ Name="物流成本", Proportion= logisticsFee},
                new ProportionOfProductCostListDto{ Name="质量成本", Proportion= qualityCost},
                new ProportionOfProductCostListDto{ Name="其他成本", Proportion= other},
            };

            //var customerTargetPrice = await _productInformationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);

            var solution = await _solutionRepository.FirstOrDefaultAsync(p => p.Id == input.SolutionId);
            var gradient = await _gradientRepository.FirstOrDefaultAsync(p => p.Id == input.GradientId);

            var customerTargetPrice = await _customerTargetPriceRepository
                .FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == solution.Productld && p.Kv == gradient.GradientValue);

            decimal targetPrice;
            try
            {
                targetPrice = Convert.ToDecimal(customerTargetPrice.TargetPrice);
            }
            catch (Exception)
            {
                //如果未填，或格式不正确，就设置为0
                targetPrice = 0;
                //throw new FriendlyException($"客户目标价录入的目标价格式不正确！");
            }

            decimal jq;
            if (customerTargetPrice.ExchangeRate is null)
            {
                jq = 0;
            }
            else
            {
                jq = targetPrice * customerTargetPrice.ExchangeRate.Value;
            }


            list.Add(new ProportionOfProductCostListDto { Name = "利润", Proportion = jq - sum });

            return new ListResultDto<ProportionOfProductCostListDto>(list);
        }


        /// <summary>
        /// 初版NRE核价表下载-流
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async virtual Task<MemoryStream> NreTableDownloadStream(NreTableDownloadInput input)
        {
            var solution = await _solutionRepository.GetAsync(input.SolutionId);

            PricingFormDto data = await _nrePricingAppService.GetPricingFormDownload(input.AuditFlowId, input.SolutionId);


            ExcelPricingFormDto dto = ObjectMapper.Map<ExcelPricingFormDto>(data);

            //模组名
            var modelCountName = await _modelCountRepository.GetAll().Where(p => p.Id == input.SolutionId).Select(p => p.Product).FirstOrDefaultAsync();
            dto.ProjectName = $"{modelCountName}——{dto.ProjectName}";

            dto.HandPieceCost = dto.HandPieceCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.MouldInventory = dto.MouldInventory.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.ToolingCost = dto.ToolingCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.FixtureCost = dto.FixtureCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.QAQCDepartments = dto.QAQCDepartments.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.ProductionEquipmentCost = dto.ProductionEquipmentCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.LaboratoryFeeModels = dto.LaboratoryFeeModels.Select((p, i) => { p.Index = i + 1; p.Count = (p.CountBottomingOut + p.CountDV + p.CountPV) + p.Unit; p.IsThirdPartyName = p.IsThirdParty ? "是" : "否"; return p; }).ToList();
            dto.SoftwareTestingCost = dto.SoftwareTestingCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.TravelExpense = dto.TravelExpense.Select((p, i) => { p.Index = i + 1; return p; }).ToList();
            dto.RestsCost = dto.RestsCost.Select((p, i) => { p.Index = i + 1; return p; }).ToList();

            dto.HandPieceCostSum = dto.HandPieceCost.Sum(p => p.Cost);
            dto.MouldInventorySum = dto.MouldInventory.Sum(p => p.Cost);
            dto.ToolingCostSum = dto.ToolingCost.Sum(p => p.Cost);
            dto.FixtureCostSum = dto.FixtureCost.Sum(p => p.Cost);
            dto.QAQCDepartmentsSum = dto.QAQCDepartments.Sum(p => p.Cost);
            dto.ProductionEquipmentCostSum = dto.ProductionEquipmentCost.Sum(p => p.Cost);
            dto.LaboratoryFeeModelsSum = dto.LaboratoryFeeModels.Sum(p => p.AllCost);
            dto.SoftwareTestingCostSum = dto.SoftwareTestingCost.Sum(p => p.Cost);
            dto.TravelExpenseSum = dto.TravelExpense.Sum(p => p.Cost);
            dto.RestsCostSum = dto.RestsCost.Sum(p => p.Cost);


            var memoryStream = new MemoryStream();

            await MiniExcel.SaveAsByTemplateAsync(memoryStream, "wwwroot/Excel/NRE.xlsx", dto);
            return memoryStream;
        }

        /// <summary>
        /// 初版NRE核价表下载
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async virtual Task<FileResult> NreTableDownload(NreTableDownloadInput input)
        {
            var memoryStream = await NreTableDownloadStream(input);
            ////开始行
            int StartLine = 5;
            memoryStream.Position = 0;
            // 创建工作簿
            var workbook = new XSSFWorkbook(memoryStream);
            // 获取第一个工作表
            var sheet = workbook.GetSheetAt(0);
            int[] ProductionEquipmentCost = new int[2];//生产设备费       
            int[] TravelExpense = new int[2];//差旅费           

            for (int rowIndex = StartLine + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row == null) continue;
                string ExpenseName = row.GetCell(2).ToString();
                string ExpenseNameHJ = row.GetCell(5).ToString();

                if (ExpenseName.Equals("生产设备费用"))
                {
                    ProductionEquipmentCost[0] = rowIndex + 2;
                }
                if (ExpenseNameHJ.Equals("生产设备费用合计"))
                {
                    ProductionEquipmentCost[1] = rowIndex - 1;
                }
                if (ExpenseName.Equals("差旅费"))
                {
                    TravelExpense[0] = rowIndex + 2;
                }
                if (ExpenseNameHJ.Equals("差旅费合计"))
                {
                    TravelExpense[1] = rowIndex - 1;
                }
            }
            //设备状态
            List<FinanceDictionaryDetail> ProductionEquipmentCostName = await _financeDictionaryDetailRepository.GetAllListAsync(p => p.FinanceDictionaryId.Equals(FinanceConsts.Sbzt));
            //事由
            List<FinanceDictionaryDetail> TravelExpenseName = await _financeDictionaryDetailRepository.GetAllListAsync(p => p.FinanceDictionaryId.Equals(FinanceConsts.NreReasons));
            //列数据约束 生产设备费用
            workbook.SetConstraint(sheet, 3, ProductionEquipmentCost[0], ProductionEquipmentCost[1], ProductionEquipmentCostName.Select(p => p.DisplayName).ToArray());
            //列数据约束 差旅费
            workbook.SetConstraint(sheet, 2, TravelExpense[0], TravelExpense[1], TravelExpenseName.Select(p => p.DisplayName).ToArray());
            // 保存工作簿
            using (MemoryStream fileStream = new MemoryStream())
            {
                workbook.Write(fileStream);
                return new FileContentResult(fileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"NRE核价表.xlsx"
                };
            }
        }
        #endregion

        #region 制造成本

        /// <summary>
        /// 保存 制造成本（添加和修改）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task ManufacturingCostInput(ManufacturingCostInput input)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(input.AuditFlowId) && p.SolutionId.Equals(input.ProductId) && p.EnumSole.Equals(AuditFlowConsts.AF_ProductionCostInput));
            if (productIsSubmits.Count is not 0)
            {
                throw new FriendlyException(input.ProductId + ":该零件id已经提交过了");
            }
            else
            {
                //查询核价需求导入时的零件信息
                var productIds = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId);

                #region Input data check

                if (input.Smt.IsNullOrEmpty() || input.Cob.IsNullOrEmpty() || input.Other.IsNullOrEmpty())
                {
                    throw new FriendlyException($"输入的SMT、COB或其他制造成本为空！");
                }

                //获取总年数
                var yearCount = await _modelCountYearRepository.GetAll()
                    .Where(p => p.AuditFlowId == input.AuditFlowId && p.ProductId == input.ProductId)
                    .Select(p => p.Year).OrderBy(p => p).ToListAsync();
                yearCount.Add(PriceEvalConsts.AllYear);
                yearCount = yearCount.OrderBy(p => p).ToList();
                var smtYear = input.Smt.Select(p => p.Year).OrderBy(p => p).ToList();
                var cobYear = input.Cob.Select(p => p.Year).OrderBy(p => p).ToList();
                var otherYear = input.Other.Select(p => p.Year).OrderBy(p => p).ToList();

                if (!(yearCount.Count == smtYear.Count && yearCount.Count == cobYear.Count)
                    || !yearCount.Zip(smtYear, (a, b) => a == b).All(p => p)
                    || !yearCount.Zip(cobYear, (a, b) => a == b).All(p => p)
                    || !yearCount.Zip(otherYear, (a, b) => a == b).All(p => p))
                {
                    throw new FriendlyException($"输入的SMT、COB或其他制造成本的年份和要求的录入的年份{string.Join("、", yearCount.Select(p => p == 0 ? "全生命周期" : p.ToString()))}不一致");
                }
                #endregion


                await Save(input.Smt, CostType.SMT);
                await Save(input.Cob, CostType.COB);
                await Save(input.Other, CostType.Other);

                #region 保存数据
                async Task Save<TManufacturingCostInput>(List<TManufacturingCostInput> a, CostType b) where TManufacturingCostInput : IManufacturingCostInput
                {
                    foreach (var cost in a)
                    {
                        var dbEntity = await _allManufacturingCostRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId
                                                && p.Year == cost.Year && p.ProductId == input.ProductId && p.CostType == b);
                        if (dbEntity is null)
                        {
                            var entity = ObjectMapper.Map<AllManufacturingCost>(cost);
                            entity.AuditFlowId = input.AuditFlowId;
                            entity.ProductId = input.ProductId;
                            entity.CostType = b;
                            await _allManufacturingCostRepository.InsertAsync(entity);
                        }
                        else
                        {
                            ObjectMapper.Map(cost, dbEntity);
                            await _allManufacturingCostRepository.UpdateAsync(dbEntity);
                        }
                    }
                }

                #endregion


                #region 录入完成之后
                await _productIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = input.AuditFlowId, SolutionId = input.ProductId, EnumSole = AuditFlowConsts.AF_ProductionCostInput });
                #endregion

                List<NreIsSubmit> allProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(input.AuditFlowId) && p.EnumSole.Equals(AuditFlowConsts.AF_ProductionCostInput));
                //当前已保存的确认表中零件数目等于 核价需求导入时的零件数目
                if (productIds.Count == allProductIsSubmits.Count + 1)
                {
                    //执行跳转
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }

                    await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                    {
                        AuditFlowId = input.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_ProductionCostInput,
                        UserId = AbpSession.UserId.Value,
                        Opinion = OPINIONTYPE.Submit_Agreee,
                    });
                }
            }
        }

        /// <summary>
        /// 制造成本录入 退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task ClearProductionCostInputState(long Id)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(AuditFlowConsts.AF_ProductionCostInput));
            foreach (NreIsSubmit item in productIsSubmits)
            {
                await _productIsSubmit.HardDeleteAsync(item);
            }
        }

        /// <summary>
        /// 获取录入的制造成本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Obsolete]
        public virtual async Task<ManufacturingCostInput> GetInputManufacturingCost(GetManufacturingCostInputDto input)
        {
            var data = await _allManufacturingCostRepository.GetAll().Where(p => p.AuditFlowId == input.AuditFlowId
            && p.ProductId == input.ProductId).ToListAsync();
            var cob = ObjectMapper.Map<List<AllManufacturingCostInput>>(data.Where(p => p.CostType == CostType.COB));
            var smt = ObjectMapper.Map<List<AllManufacturingCostInput>>(data.Where(p => p.CostType == CostType.SMT));
            var other = ObjectMapper.Map<List<OtherManufacturingCostInput>>(data.Where(p => p.CostType == CostType.Other));

            if (cob.Count == 0 && smt.Count == 0)
            {
                return null;
            }
            return new ManufacturingCostInput { AuditFlowId = input.AuditFlowId, ProductId = input.ProductId, Cob = cob, Smt = smt, Other = other };
        }

        #endregion

        #region 保存核价看板各成本修改后合计，用于核心器件查询使用

        public async Task SaveAfterUpdateSum(AfterUpdateSumDto input)
        {
            var afterUpdateSumInfoList = await _afterUpdateSumInfoRepository.GetAllListAsync(p => p.AuditFlowId.Equals(input.AuditFlowId) && p.SolutionId.Equals(input.SolutionId) && p.GradientId.Equals(input.GradientId) && p.Year.Equals(input.Year) && p.UpDown.Equals(input.UpDown));


            if (afterUpdateSumInfoList.Count is not 0)
            {
                AfterUpdateSumInfo afterUpdateSumInfo = afterUpdateSumInfoList.FirstOrDefault();

                if (input.QualityCostAfterSum > 0)
                {
                    afterUpdateSumInfo.QualityCostAfterSum = input.QualityCostAfterSum;
                }
                if (input.LossCostAfterSum > 0)
                {
                    afterUpdateSumInfo.LossCostAfterSum = input.LossCostAfterSum;
                }
                if (input.ManufacturingAfterSum > 0)
                {
                    afterUpdateSumInfo.ManufacturingAfterSum = input.ManufacturingAfterSum;
                }
                if (input.LogisticsAfterSum > 0)
                {
                    afterUpdateSumInfo.LogisticsAfterSum = input.LogisticsAfterSum;
                }
                if (input.OtherCosttAfterSum > 0)
                {
                    afterUpdateSumInfo.OtherCosttAfterSum = input.OtherCosttAfterSum;
                }


                await _afterUpdateSumInfoRepository.UpdateAsync(afterUpdateSumInfo);

            }
            else
            {

                AfterUpdateSumInfo afterSumInfo = new()
                {
                    AuditFlowId = input.AuditFlowId,
                    SolutionId = input.SolutionId,
                    GradientId = input.GradientId,
                    Year = input.Year,
                    UpDown = input.UpDown,
                    QualityCostAfterSum = input.QualityCostAfterSum,
                    LossCostAfterSum = input.LossCostAfterSum,
                    ManufacturingAfterSum = input.LossCostAfterSum,
                    OtherCosttAfterSum = input.OtherCosttAfterSum,

                };
                await _afterUpdateSumInfoRepository.InsertAsync(afterSumInfo);

            }



        }

        #endregion
    }
}
