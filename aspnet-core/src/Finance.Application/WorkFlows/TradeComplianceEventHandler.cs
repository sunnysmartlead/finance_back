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
    /// 系统判断贸易贸易合规等其他需要自动流转的流程并对接工作流
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
        /// 贸易合规等节点被激活时触发
        /// </summary>
        /// <param name="eventData"></param>
        public async void HandleEvent(EntityUpdatedEventData<NodeInstance> eventData)
        {
            using var uow = _unitOfWorkManager.Begin();
            using (_unitOfWorkManager.Current.SetTenantId(1))
            {
                if (eventData.Entity.NodeId == "主流程_贸易合规")
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

                //如果是自动生成相关的流转页面
                var autoFlows = new List<string> { "主流程_生成报价分析界面选择报价方案", "主流程_系统生成报价审批表报价单" };
                if (autoFlows.Contains(eventData.Entity.NodeId))
                {
                    await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                    {
                        NodeInstanceId = eventData.Entity.Id,
                        FinanceDictionaryDetailId = FinanceConsts.Done,
                        Comment = "系统自动流转"
                    });
                }
            }

        }
    }
}
