using Abp.Domain.Repositories;
using Abp.Extensions;
using Finance.Audit;
using Finance.Audit.Dto;
using Finance.Dto;
using Finance.EngineeringDepartment;
using Finance.Entering;
using Finance.NerPricing;
using Finance.PriceEval.Dto;
using Finance.ProductDevelopment;
using Finance.ProductionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 核价看板审核后端接口
    /// </summary>
    public class PriceBoardCheckAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;
        private readonly NrePricingAppService _nrePricingAppService;
        private readonly ElectronicBomAppService _electronicBomAppService;
        private readonly StructionBomAppService _structionBomAppService;
        private readonly LossRateAppService _lossRateAppService;
        private readonly WorkingHoursAppService _workingHoursAppService;
        private readonly ProductionControlAppService _productionControlAppService;
        private readonly PriceEvaluationAppService _priceEvaluationAppService;
        /// <summary>
        /// 资源单价录入服务
        /// </summary>
        private readonly ResourceEnteringAppService _resourceEnteringAppService;
        private readonly IRepository<ModelCount, long> _modelCountRepository;    
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="flowAppService"></param>
        /// <param name="nrePricingAppService"></param>
        /// <param name="resourceEnteringAppService"></param>
        /// <param name="electronicBomAppService"></param>
        /// <param name="structionBomAppService"></param>
        /// <param name="lossRateAppService"></param>
        /// <param name="workingHoursAppService"></param>
        /// <param name="productionControlAppService"></param>
        /// <param name="priceEvaluationAppService"></param>
        public PriceBoardCheckAppService(AuditFlowAppService flowAppService, NrePricingAppService nrePricingAppService, ResourceEnteringAppService resourceEnteringAppService, ElectronicBomAppService electronicBomAppService, StructionBomAppService structionBomAppService, LossRateAppService lossRateAppService, WorkingHoursAppService workingHoursAppService, ProductionControlAppService productionControlAppService, PriceEvaluationAppService priceEvaluationAppService, IRepository<ModelCount, long> modelCountRepository)
        {
            _flowAppService = flowAppService;
            _nrePricingAppService = nrePricingAppService;
            _resourceEnteringAppService = resourceEnteringAppService;
            _electronicBomAppService = electronicBomAppService;
            _structionBomAppService = structionBomAppService;
            _lossRateAppService = lossRateAppService;
            _workingHoursAppService = workingHoursAppService;
            _productionControlAppService = productionControlAppService;
            _priceEvaluationAppService = priceEvaluationAppService;
            _modelCountRepository = modelCountRepository;         
        }

        /// <summary>
        /// 核价看板审核
        /// </summary>
        public async virtual Task<ReturnDto> SetPriceBoardState(PriceBoardCheckDto priceBoardCheck)
        {
            ReturnDto returnDto = new();
            if (AbpSession.UserId is null)
            {
                throw new FriendlyException("请先登录");
            }
            AuditFlowDetailDto flowDetailDto = new()
            {
                AuditFlowId = priceBoardCheck.AuditFlowId,
                UserId = AbpSession.UserId.Value,
                ProcessIdentifier = AuditFlowConsts.AF_PriceBoardAudit
            };
            if (priceBoardCheck.IsAgree)
            {
                //判断是否生成了核价表
                List<ModelCount> dto = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == priceBoardCheck.AuditFlowId);
                foreach (ModelCount item in dto)
                {                 
                    if(item.TableJson is null || item.TableAllJson is null) throw new FriendlyException($"{item.Product}:未生成核价表!");
                }              
                flowDetailDto.Opinion = OPINIONTYPE.Submit_Agreee;             
            }
            else
            {
                if (priceBoardCheck.OpinionDescription.IsNullOrEmpty())
                {
                    throw new FriendlyException("拒绝原因不能为空");
                }

                flowDetailDto.OpinionDescription = OpinionDescription.OD_PriceBoardCheck + priceBoardCheck.OpinionDescription;
                flowDetailDto.Opinion = OPINIONTYPE.Reject;
                _flowAppService.SavaBackProcessIdentifiers(priceBoardCheck.BackProcessIdentifiers);
                foreach (var identifier in priceBoardCheck.BackProcessIdentifiers)
                {
                    if(identifier == AuditFlowConsts.AF_NreInputEmc)
                    {
                        await _nrePricingAppService.GetProductDepartmentConfigurationState(priceBoardCheck.AuditFlowId);
                    }
                    else if(identifier == AuditFlowConsts.AF_NreInputGage)
                    {
                        //await _nrePricingAppService.GetQcGaugeConfigurationState(priceBoardCheck.AuditFlowId);
                    }
                    else if(identifier == AuditFlowConsts.AF_NreInputMould)
                    {
                        await _nrePricingAppService.GetResourcesManagementConfigurationState(priceBoardCheck.AuditFlowId);
                    }
                    else if (identifier == AuditFlowConsts.AF_NreInputOther)
                    {
                        await _nrePricingAppService.GetProjectManagementConfigurationState(priceBoardCheck.AuditFlowId);
                    }
                    else if (identifier == AuditFlowConsts.AF_NreInputTest)
                    {
                        await _nrePricingAppService.GetExperimentItemsConfigurationState(priceBoardCheck.AuditFlowId);
                    }
                    else if (identifier == AuditFlowConsts.AF_StructPriceInput)
                    {
                        await _resourceEnteringAppService.GetStructuralConfigurationState(priceBoardCheck.AuditFlowId);
                    }
                    else if (identifier == AuditFlowConsts.AF_ElectronicPriceInput)
                    {
                        await _resourceEnteringAppService.GetElectronicConfigurationState(priceBoardCheck.AuditFlowId);
                    }                   
                    else if (identifier == AuditFlowConsts.AF_ElecLossRateInput)
                    {
                        await _lossRateAppService.ClearLossRateInfoState(priceBoardCheck.AuditFlowId, AuditFlowConsts.AF_ElecLossRateInput);
                    }
                    else if (identifier == AuditFlowConsts.AF_StructLossRateInput)
                    {
                        await _lossRateAppService.ClearLossRateInfoState(priceBoardCheck.AuditFlowId, AuditFlowConsts.AF_StructLossRateInput);
                    }
                    else if (identifier == AuditFlowConsts.AF_ManHourImport)
                    {
                        await _workingHoursAppService.ClearWorkHoursState(priceBoardCheck.AuditFlowId);
                        await _priceEvaluationAppService.ClearProductionCostInputState(priceBoardCheck.AuditFlowId);
                    }
                    else if (identifier == AuditFlowConsts.AF_LogisticsCostInput)
                    {
                        await _productionControlAppService.ClearLogisticsCostInputState(priceBoardCheck.AuditFlowId);
                    }
                    else if(identifier == AuditFlowConsts.AF_ElectronicBomImport)
                    {
                        await _resourceEnteringAppService.GetElectronicConfigurationState(priceBoardCheck.AuditFlowId);
                        await _electronicBomAppService.ClearElectronicBomBak(priceBoardCheck.AuditFlowId);
                        await _electronicBomAppService.ClearElecBomImportState(priceBoardCheck.AuditFlowId);
                        await _lossRateAppService.ClearLossRateInfoState(priceBoardCheck.AuditFlowId, AuditFlowConsts.AF_ElecLossRateInput);
                    }
                    else if (identifier == AuditFlowConsts.AF_StructBomImport)
                    {
                        await _nrePricingAppService.GetResourcesManagementConfigurationState(priceBoardCheck.AuditFlowId);
                        await _resourceEnteringAppService.GetStructuralConfigurationState(priceBoardCheck.AuditFlowId);
                        await _structionBomAppService.ClearStructionBomBak(priceBoardCheck.AuditFlowId);
                        await _structionBomAppService.ClearStructBomImportState(priceBoardCheck.AuditFlowId);
                        await _lossRateAppService.ClearLossRateInfoState(priceBoardCheck.AuditFlowId, AuditFlowConsts.AF_StructLossRateInput);
                        await _workingHoursAppService.ClearWorkHoursState(priceBoardCheck.AuditFlowId);
                        await _productionControlAppService.ClearLogisticsCostInputState(priceBoardCheck.AuditFlowId);
                        await _priceEvaluationAppService.ClearProductionCostInputState(priceBoardCheck.AuditFlowId);
                    }
                }
            }

            returnDto = await _flowAppService.UpdateAuditFlowInfo(flowDetailDto);
            return returnDto;
        }

        /// <summary>
        /// 项目部核价审核
        /// </summary>
        public async virtual Task<ReturnDto> SetProjectPriceState(PriceBoardCheckDto priceBoardCheck)
        {
            ReturnDto returnDto = new();
            if (AbpSession.UserId is null)
            {
                throw new FriendlyException("请先登录");
            }
            AuditFlowDetailDto flowDetailDto = new()
            {
                AuditFlowId = priceBoardCheck.AuditFlowId,
                UserId = AbpSession.UserId.Value,
                ProcessIdentifier = AuditFlowConsts.AF_ProjectPriceAudit
            };
            if (priceBoardCheck.IsAgree)
            {
                flowDetailDto.Opinion = OPINIONTYPE.Submit_Agreee;
            }
            else
            {
                if (priceBoardCheck.OpinionDescription.IsNullOrEmpty())
                {
                    throw new FriendlyException("拒绝原因不能为空");
                }

                flowDetailDto.OpinionDescription = OpinionDescription.OD_ProjectPriceCheck + priceBoardCheck.OpinionDescription;
                flowDetailDto.Opinion = OPINIONTYPE.Reject;
            }

            returnDto = await _flowAppService.UpdateAuditFlowInfo(flowDetailDto);
            return returnDto;
        }

        /// <summary>
        /// 财务部核价审核
        /// </summary>
        public async virtual Task<ReturnDto> SetFinancePriceState(PriceBoardCheckDto priceBoardCheck)
        {
            ReturnDto returnDto = new();
            if (AbpSession.UserId is null)
            {
                throw new FriendlyException("请先登录");
            }
            AuditFlowDetailDto flowDetailDto = new()
            {
                AuditFlowId = priceBoardCheck.AuditFlowId,
                UserId = AbpSession.UserId.Value,
                ProcessIdentifier = AuditFlowConsts.AF_FinancePriceAudit
            };
            if (priceBoardCheck.IsAgree)
            {
                flowDetailDto.Opinion = OPINIONTYPE.Submit_Agreee;
            }
            else
            {
                if (priceBoardCheck.OpinionDescription.IsNullOrEmpty())
                {
                    throw new FriendlyException("拒绝原因不能为空");
                }

                flowDetailDto.OpinionDescription = OpinionDescription.OD_FinancePriceCheck + priceBoardCheck.OpinionDescription;
                flowDetailDto.Opinion = OPINIONTYPE.Reject;
            }

            returnDto = await _flowAppService.UpdateAuditFlowInfo(flowDetailDto);
            return returnDto;
        }
    }
}
