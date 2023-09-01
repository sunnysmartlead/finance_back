using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Audit;
using Finance.TradeCompliance;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 系统判断贸易贸易合规并对接工作流
    /// </summary>
    public class TradeComplianceEventHandler : IEventHandler<EntityUpdatedEventData<NodeInstance>>, ITransientDependency
    {

        private readonly TradeComplianceAppService _tradeComplianceAppService;
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tradeComplianceAppService"></param>
        /// <param name="workflowInstanceAppService"></param>
        /// <param name="unitOfWorkManager"></param>
        public TradeComplianceEventHandler(TradeComplianceAppService tradeComplianceAppService, WorkflowInstanceAppService workflowInstanceAppService, IUnitOfWorkManager unitOfWorkManager)
        {
            _tradeComplianceAppService = tradeComplianceAppService;
            _workflowInstanceAppService = workflowInstanceAppService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// 贸易合规节点被激活时触发
        /// </summary>
        /// <param name="eventData"></param>
        public async void HandleEvent(EntityUpdatedEventData<NodeInstance> eventData)
        {
            if (eventData.Entity.NodeId == "主流程_贸易合规")
            {
                using var uow = _unitOfWorkManager.Begin();
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    var isOk = await _tradeComplianceAppService.IsProductsTradeComplianceOK(eventData.Entity.WorkFlowInstanceId);
                    if (isOk)
                    {
                        await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                        {
                            NodeInstanceId = eventData.Entity.Id,
                            FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                            Comment = "系统判断合规"
                        });
                    }
                    else
                    {
                        await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                        {
                            NodeInstanceId = eventData.Entity.Id,
                            FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                            Comment = "系统判断不合规"
                        });
                    }
                }
            }
        }
    }
}
