using Abp.Application.Services;
using Abp.Authorization;
using Finance.Audit;
using Finance.BaseLibrary;
using Finance.DemandApplyAudit;
using Finance.Dto;
using Finance.Entering;
using Finance.NerPricing;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Processes;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.DemandApplyAudit;
using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.QuickQuotationReview
{
    /// <summary>
    /// 快速核报价
    /// </summary>
    [AbpAuthorize]
    public class QuickQuotationReviewAppService : ApplicationService
    {
        /// <summary>
        /// 营销部录入审核
        /// </summary>
        private PriceEvaluationAppService _priceEvaluationAppService;
        /// <summary>
        /// 核价服务
        /// </summary>
        private DemandApplyAuditAppService _demandApplyAuditAppService;
        /// <summary>
        /// NRE服务
        /// </summary>
        private NrePricingAppService _nrePricingAppService;
        /// <summary>
        /// 单子单价录入服务
        /// </summary>
        private ResourceEnteringAppService _resourceEnteringAppService;
        /// <summary>
        /// 工序工时服务
        /// </summary>
        private ProcessHoursEnterAppService _processHoursEnterAppService;
        /// <summary>
        /// 物流成本录入
        /// </summary>
        private LogisticscostAppService _logisticscostAppService;
        /// <summary>
        /// 制造成本录入
        /// </summary>
        private BomEnterAppService _bomEnterAppService;
        /// <summary>
        /// 电子BOM录入
        /// </summary>
        private ElectronicBomAppService _electronicBomAppService;
        /// <summary>
        /// 结构BOM录入
        /// </summary>
        private StructionBomAppService _structionBomAppService;
        /// <summary>
        /// 产品开发部录入
        /// </summary>
        private ProductDevelopmentInputAppService _productDevelopmentInputAppService;

        public QuickQuotationReviewAppService(PriceEvaluationAppService priceEvaluationAppService, DemandApplyAuditAppService demandApplyAuditAppService, NrePricingAppService nrePricingAppService, ResourceEnteringAppService resourceEnteringAppService, ProcessHoursEnterAppService processHoursEnterAppService, LogisticscostAppService logisticscostAppService, BomEnterAppService bomEnterAppService, ElectronicBomAppService electronicBomAppService, StructionBomAppService structionBomAppService, ProductDevelopmentInputAppService productDevelopmentInputAppService)
        {
            _priceEvaluationAppService = priceEvaluationAppService;
            _demandApplyAuditAppService = demandApplyAuditAppService;
            _nrePricingAppService = nrePricingAppService;
            _resourceEnteringAppService = resourceEnteringAppService;
            _processHoursEnterAppService = processHoursEnterAppService;
            _logisticscostAppService = logisticscostAppService;
            _bomEnterAppService = bomEnterAppService;
            _electronicBomAppService = electronicBomAppService;
            _structionBomAppService = structionBomAppService;
            _productDevelopmentInputAppService = productDevelopmentInputAppService;
        }
        /// <summary>
        /// 开始快速核价：报价核价需求录入界面（第一步）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>    
        public async virtual Task<PriceEvaluationStartResult> PriceEvaluationStart(PriceEvaluationStartInputQuoteFlow input)
        {
            input.QuickQuoteAuditFlowId = input.QuoteAuditFlowId;
            //核价需求录入
            PriceEvaluationStartResult priceEvaluationStartResult = new();
            if(input.IsSubmit)
            {
                priceEvaluationStartResult = await _priceEvaluationAppService.PriceEvaluationStart(input);
            }
            else
            {
                PriceEvaluationStartSaveInput priceEvaluationStartSaveInpu = ObjectMapper.Map<PriceEvaluationStartSaveInput>(input);
                priceEvaluationStartResult = await _priceEvaluationAppService.PriceEvaluationStartSave(priceEvaluationStartSaveInpu);
                return priceEvaluationStartResult;              
            }                
            dto dto = new dto
            {
                NewAuditFlowId = priceEvaluationStartResult.AuditFlowId,
                QuoteAuditFlowId = input.QuoteAuditFlowId,
            };
            //核价需求录入审核
            List<SolutionIdAndQuoteSolutionId> solutionIdAnds = await _demandApplyAuditAppService.FastAuditEntering(dto.NewAuditFlowId, dto.QuoteAuditFlowId);
            dto.SolutionIdAndQuoteSolutionId = solutionIdAnds;
            //工时工序  
            IdMappingListHoursEnter idMappingListHoursEnter= await _processHoursEnterAppService.ProcessHoursEnterCopyAsync(dto.QuoteAuditFlowId, dto.NewAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //物流成本
            await _logisticscostAppService.LogisticscostsCopyAsync(dto.QuoteAuditFlowId, dto.NewAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //制造成本
            await _bomEnterAppService.GetBomEntersCopyAsync(dto.QuoteAuditFlowId, dto.NewAuditFlowId, dto.SolutionIdAndQuoteSolutionId);            
            //电子BOM录入复制表
            List<BomIdAndQuoteBomId> electronicBomIdAndQuoteBomIds =  await _electronicBomAppService.FastPostElectronicEntering(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //结构录入
            List<BomIdAndQuoteBomId> structionBomIdAndQuoteBomIds = await _structionBomAppService.FastPostStruralEntering(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //电子单价录入
            await _resourceEnteringAppService.FastPostElectronicMaterialEntering(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId, electronicBomIdAndQuoteBomIds);
            //结构单价录入
            await _resourceEnteringAppService.FastPostStructuralMemberEntering(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId, structionBomIdAndQuoteBomIds);
            //电子单价录入复制表
            await _resourceEnteringAppService.FastPostElectronicMaterialEnteringCopy(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId, electronicBomIdAndQuoteBomIds);
            //结构单价录入复制表
            await _resourceEnteringAppService.FastPostStructuralMemberEnteringCopy(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId, structionBomIdAndQuoteBomIds);
            //电子bak录入复制表
            await _electronicBomAppService.FastPostElectronicEnteringCopy(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //电子BOM两次上传差异化表
            await _electronicBomAppService.FastPostElectronicDifferCopy(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //拼版表快速核报价
            await _electronicBomAppService.FastPostBoardCopy(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //结构bak录入复制表
            await _structionBomAppService.FastPostStuctEnteringCopy(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //结构BOM两次上传差异化表
            await _structionBomAppService.FastPostStuctDifferCopy(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //物流基础信息表
            await _productDevelopmentInputAppService.FastPostProductDevelopmentCopy(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            AllIdMappingList allIdMappingList = new AllIdMappingList();
            //NRE环境实验费
            allIdMappingList.FastPostExperimentItemsSingle = await _nrePricingAppService.FastPostExperimentItemsSingle(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //NRE模具费
            allIdMappingList.FastPostResourcesManagementSingle = await _nrePricingAppService.FastPostResourcesManagementSingle(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //NRE---EMC实验费
            allIdMappingList.FastPostEmcItemsSingle = await _nrePricingAppService.FastPostEmcItemsSingle(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            //NRE手板件、其他、差旅费
            IdMappingList FastPostProjectManagementSingle= await _nrePricingAppService.FastPostProjectManagementSingle(dto.NewAuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            allIdMappingList.HandPieceCost = FastPostProjectManagementSingle.HandPieceCost;
            allIdMappingList.RestsCost = FastPostProjectManagementSingle.RestsCost;
            allIdMappingList.TravelExpense = FastPostProjectManagementSingle.TravelExpense;
            //NRE核价看板修改项复制
            await _nrePricingAppService.FastModify(allIdMappingList, dto, idMappingListHoursEnter);
            return priceEvaluationStartResult;
        }       
    }
}
