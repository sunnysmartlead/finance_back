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
        private readonly IRepository<TradeComplianceCheck, long> _tradeComplianceCheckRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TradeComplianceEventHandler(WorkflowInstanceAppService workflowInstanceAppService, TradeComplianceAppService tradeComplianceAppService, IRepository<TradeComplianceCheck, long> tradeComplianceCheckRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _workflowInstanceAppService = workflowInstanceAppService;
            _tradeComplianceAppService = tradeComplianceAppService;
            _tradeComplianceCheckRepository = tradeComplianceCheckRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

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

        ///// <summary>
        ///// 是否贸易合规，合规返回true，不合规返回false
        ///// </summary>
        ///// <returns></returns>
        //public virtual bool GetIsTradeCompliance(long auditFlowId)
        //{
        //    var tradeComplianceCheckList = _tradeComplianceCheckRepository.GetAllList(p => p.AuditFlowId == auditFlowId);
        //    foreach (var tradeComplianceCheck in tradeComplianceCheckList)
        //    {
        //        if (tradeComplianceCheck.AnalysisConclusion.Equals(GeneralDefinition.TRADE_COMPLIANCE_NOT_OK))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
    }
}
