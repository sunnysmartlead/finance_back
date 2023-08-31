using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.TradeCompliance;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    public class TradeComplianceEventHandler : IEventHandler<EntityUpdatingEventData<NodeInstance>>, ITransientDependency
    {

        private readonly TradeComplianceAppService _tradeComplianceAppService;
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;

        public TradeComplianceEventHandler(TradeComplianceAppService tradeComplianceAppService, WorkflowInstanceAppService workflowInstanceAppService)
        {
            _tradeComplianceAppService = tradeComplianceAppService;
            _workflowInstanceAppService = workflowInstanceAppService;
        }

        public async void HandleEvent(EntityUpdatingEventData<NodeInstance> eventData)
        {
            if (eventData.Entity.NodeId == "主流程_贸易合规")
            {
                await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                {
                    NodeInstanceId = eventData.Entity.Id,
                    FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                    Comment = "系统判断"
                });
            }
        }
    }
}
