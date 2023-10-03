using Finance.Audit;
using Finance.Audit.Dto;
using Finance.Dto;
using Finance.TradeCompliance.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TradeCompliance
{
    /// <summary>
    /// 财务贸易合规判定后端
    /// </summary>
    public class FinanceCheckAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="flowAppService"></param>
        public FinanceCheckAppService(AuditFlowAppService flowAppService)
        {
            _flowAppService = flowAppService;
        }

        /// <summary>
        /// 财务贸易合规判定
        /// </summary>
        public async virtual Task<ReturnDto> IsTradeComplianceCheck(FinanceCheckDto financeCheck)
        {
            ReturnDto returnDto = new();
            if (AbpSession.UserId is null)
            {
                throw new FriendlyException("请先登录");
            }
            //AuditFlowDetailDto flowDetailDto = new()
            //{
            //    AuditFlowId = financeCheck.AuditFlowId,
            //    ProcessIdentifier = AuditFlowConsts.AF_TradeApproval,
            //    UserId = AbpSession.UserId.Value,
            //    OpinionDescription = OpinionDescription.OD_TradeComplianceJudge + financeCheck.OpinionDescription
            //};
            //if (financeCheck.IsAgree)
            //{
            //    flowDetailDto.Opinion = OPINIONTYPE.Submit_Agreee;
            //}
            //else
            //{
            //    flowDetailDto.Opinion = OPINIONTYPE.Reject;
            //}
            //returnDto = await _flowAppService.UpdateAuditFlowInfo(flowDetailDto);

            return new ReturnDto { IsSuccess = true, Message = "提交成功！" };
        }
    }
}
