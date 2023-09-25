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
using Finance.ProductDevelopment;

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
        private readonly ElectronicBomAppService _electronicBomAppService;
        private readonly StructionBomAppService _structionBomAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tradeComplianceAppService"></param>
        /// <param name="workflowInstanceAppService"></param>
        /// <param name="unitOfWorkManager"></param>
        public TradeComplianceEventHandler(TradeComplianceAppService tradeComplianceAppService, WorkflowInstanceAppService workflowInstanceAppService, IUnitOfWorkManager unitOfWorkManager, ElectronicBomAppService electronicBomAppService, StructionBomAppService structionBomAppService)
        {
            _tradeComplianceAppService = tradeComplianceAppService;
            _workflowInstanceAppService = workflowInstanceAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _electronicBomAppService = electronicBomAppService;
            _structionBomAppService = structionBomAppService;
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
                //必须被激活才能执行
                if (eventData.Entity.NodeInstanceStatus != NodeInstanceStatus.Current)
                {
                    return;
                }
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
                var autoFlows = new List<string> {  "主流程_系统生成报价审批表报价单" };//"主流程_生成报价分析界面选择报价方案",
                if (autoFlows.Contains(eventData.Entity.NodeId))
                {
                    await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                    {
                        NodeInstanceId = eventData.Entity.Id,
                        FinanceDictionaryDetailId = FinanceConsts.Done,
                        Comment = "系统自动流转"
                    });
                }

                //如果是流转到电子BOM退回页面的
                if (eventData.Entity.NodeId == "主流程_上传电子BOM")
                {
                    await _electronicBomAppService.ClearElecBomImportState(eventData.Entity.WorkFlowInstanceId);
                }

                //如果是流转到结构BOM退回页面的
                if (eventData.Entity.NodeId == "主流程_上传结构BOM")
                {
                    await _structionBomAppService.ClearStructBomImportState(eventData.Entity.WorkFlowInstanceId);
                }

            }

        }
    }
}
