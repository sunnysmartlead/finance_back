using Abp.Application.Services;
using Abp.Authorization;
using Finance.Audit;
using Finance.DemandApplyAudit;
using Finance.NerPricing;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
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
        /// 构造函数
        /// </summary>
        /// <param name="priceEvaluationAppService"></param>
        /// <param name="applyAuditAppService"></param>
        /// <param name="nrePricingAppService"></param>
        public QuickQuotationReviewAppService(PriceEvaluationAppService priceEvaluationAppService, DemandApplyAuditAppService applyAuditAppService, NrePricingAppService nrePricingAppService)
        {
            _priceEvaluationAppService = priceEvaluationAppService;
            _demandApplyAuditAppService = applyAuditAppService;
            _nrePricingAppService = nrePricingAppService;
        }

        /// <summary>
        /// 开始快速核价：报价核价需求录入界面（第一步）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>    
        public async virtual Task<PriceEvaluationStartResult> PriceEvaluationStart(PriceEvaluationStartInputQuoteFlow input)
        {
            //核价需求录入
            PriceEvaluationStartResult priceEvaluationStartResult = await _priceEvaluationAppService.PriceEvaluationStart(input);
            dto dto = new dto
            {
                AuditFlowId = priceEvaluationStartResult.AuditFlowId,
                QuoteAuditFlowId = input.QuoteAuditFlowId,
            };
            //核价需求录入审核
            List<SolutionIdAndQuoteSolutionId> solutionIdAnds = await _demandApplyAuditAppService.FastAuditEntering(dto.AuditFlowId, dto.QuoteAuditFlowId);
            dto.SolutionIdAndQuoteSolutionId = solutionIdAnds;
            //产品开发部审核 无信息录入

            //Nre环境实验费
            await _nrePricingAppService.FastPostExperimentItemsSingle(dto.AuditFlowId, dto.QuoteAuditFlowId, dto.SolutionIdAndQuoteSolutionId);
            return priceEvaluationStartResult;
        }
        public class dto
        {
            /// <summary>
            /// 流程ID
            /// </summary>
            public long AuditFlowId { get; set; }
            /// <summary>
            /// 引用流程的流程ID
            /// </summary>
            public long QuoteAuditFlowId { get; set; }
            /// <summary>
            /// 方案ID和引用流程的方案ID
            /// </summary>
            public List<SolutionIdAndQuoteSolutionId> SolutionIdAndQuoteSolutionId { get; set; }
        }
    }
}
