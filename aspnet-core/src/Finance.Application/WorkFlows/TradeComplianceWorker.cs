using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Finance.TradeCompliance;
using System;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 3分钟刷新一次停留在待办的贸易合规项
    /// </summary>
    public class TradeComplianceWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IRepository<NodeInstance, long> _nodeInstanceRepository;
        private readonly TradeComplianceAppService _tradeComplianceAppService;
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;

        public TradeComplianceWorker(AbpTimer timer, IRepository<NodeInstance, long> nodeInstanceRepository, TradeComplianceAppService tradeComplianceAppService, WorkflowInstanceAppService workflowInstanceAppService)
        : base(timer)
        {
            _nodeInstanceRepository = nodeInstanceRepository;
            _tradeComplianceAppService = tradeComplianceAppService;
            _workflowInstanceAppService = workflowInstanceAppService;

            //3分钟执行一次
            Timer.Period = 3 * 60 * 1000;
        }

        [UnitOfWork]
        protected override async void DoWork()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var threeMinutes = Clock.Now.Subtract(TimeSpan.FromMinutes(3));

                var nodes = await _nodeInstanceRepository.GetAllListAsync(p => p.NodeId == "主流程_贸易合规" && p.NodeInstanceStatus == NodeInstanceStatus.Current && p.LastModificationTime < threeMinutes);
                foreach (var node in nodes)
                {
                    var isOk = await _tradeComplianceAppService.IsProductsTradeComplianceOK(node.WorkFlowInstanceId);
                    if (isOk)
                    {
                        await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                        {
                            NodeInstanceId = node.Id,
                            FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
                            Comment = "系统判断合规（定时刷新）"
                        });
                    }
                    else
                    {
                        await _workflowInstanceAppService.SubmitNode(new Dto.SubmitNodeInput
                        {
                            NodeInstanceId = node.Id,
                            FinanceDictionaryDetailId = FinanceConsts.YesOrNo_No,
                            Comment = "系统判断不合规（定时刷新）"
                        });
                    }
                }
                CurrentUnitOfWork.SaveChanges();
            }
        }
    }
}
