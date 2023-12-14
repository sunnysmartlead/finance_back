using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Entering;
using Finance.Job;
using Finance.NerPricing;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 工作流历史事件
    /// </summary>
    public class LineEventHandler : IEventHandler<EntityCreatedEventData<InstanceHistory>>, ITransientDependency
    {
        private readonly ResourceEnteringAppService _resourceEnteringAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly NrePricingAppService _nrePricingAppService;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public LineEventHandler(ResourceEnteringAppService resourceEnteringAppService, IUnitOfWorkManager unitOfWorkManager, NrePricingAppService nrePricingAppService, IBackgroundJobManager backgroundJobManager)
        {
            _resourceEnteringAppService = resourceEnteringAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _nrePricingAppService = nrePricingAppService;
            _backgroundJobManager = backgroundJobManager;
        }



        /// <summary>
        /// 贸易合规等节点被激活时触发
        /// </summary>
        /// <param name="eventData"></param>
        public async void HandleEvent(EntityCreatedEventData<InstanceHistory> eventData)
        {

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {

                    if (
                        (eventData.Entity.NodeId == "主流程_核价看板" && (eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Dzbomppxg || eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Scdzbom))
                        || (eventData.Entity.NodeId == "主流程_不合规是否退回" && (eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.MybhgSelect_Dzbomppxg || eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.MybhgSelect_Scdzbom))
                        )
                    {
                        await _resourceEnteringAppService.GetElectronicConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    if (
                        (eventData.Entity.NodeId == "主流程_核价看板" && (eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Jgbomppxg || eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Scjgbom))
                        || (eventData.Entity.NodeId == "主流程_不合规是否退回" && (eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.MybhgSelect_Jgbomppxg || eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.MybhgSelect_Scjgbom))
                        )
                    {
                        await _resourceEnteringAppService.GetStructuralConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    if (
                        (eventData.Entity.NodeId == "主流程_核价看板" && (eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Nremjflr || eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Scjgbom))
                        || (eventData.Entity.NodeId == "主流程_不合规是否退回" && (eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.MybhgSelect_Nremjflr || eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.MybhgSelect_Scjgbom))
                        )
                    {
                        await _nrePricingAppService.GetResourcesManagementConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    //中标后录入基础库
                    if (eventData.Entity.NodeId == "主流程_确认中标金额" && eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.YesOrNo_Yes)
                    {
                        await _backgroundJobManager.EnqueueAsync<PublicMateriaJob, long>(eventData.Entity.WorkFlowInstanceId);
                    }
                }
                uow.Complete();
            }
        }
    }
}
