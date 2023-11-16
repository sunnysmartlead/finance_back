using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Entering;
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

        public LineEventHandler(ResourceEnteringAppService resourceEnteringAppService, IUnitOfWorkManager unitOfWorkManager, NrePricingAppService nrePricingAppService)
        {
            _resourceEnteringAppService = resourceEnteringAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _nrePricingAppService = nrePricingAppService;
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
                    ////必须是已重置才能执行
                    //if (eventData.Entity.NodeInstanceStatus != NodeInstanceStatus.Passed)
                    //{
                    //    return;
                    //}

                    //如果是流转到主流程_电子BOM匹配修改
                    //if (eventData.Entity.LineId == "主流程_核价看板_主流程_电子BOM匹配修改" || eventData.Entity.LineId == "主流程_不合规是否退回_主流程_电子BOM匹配修改")
                    //{
                    //    await _resourceEnteringAppService.GetElectronicConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    //}
                    if ((eventData.Entity.NodeId == "主流程_核价看板" && eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Dzbomppxg) || (eventData.Entity.NodeId == "主流程_不合规是否退回" && eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.MybhgSelect_Dzbomppxg))
                    {
                        await _resourceEnteringAppService.GetElectronicConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    //如果是流转到主流程_结构BOM匹配修改
                    //if (eventData.Entity.LineId == "主流程_核价看板_主流程_结构BOM匹配修改" || eventData.Entity.LineId == "主流程_不合规是否退回_主流程_结构BOM匹配修改")
                    //{
                    //    await _resourceEnteringAppService.GetStructuralConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    //}

                    if ((eventData.Entity.NodeId == "主流程_核价看板" && eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Jgbomppxg) || (eventData.Entity.NodeId == "主流程_不合规是否退回" && eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.MybhgSelect_Jgbomppxg))
                    {
                        await _resourceEnteringAppService.GetStructuralConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    //如果是流转到主流程_NRE模具费录入
                    //if (eventData.Entity.LineId == "主流程_核价看板_主流程_NRE模具费录入" || eventData.Entity.LineId == "主流程_不合规是否退回_主流程_NRE模具费录入")
                    //{
                    //    await _nrePricingAppService.GetResourcesManagementConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    //}
                    if ((eventData.Entity.NodeId == "主流程_核价看板" && eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.HjkbSelect_Nremjflr) || (eventData.Entity.NodeId == "主流程_不合规是否退回" && eventData.Entity.FinanceDictionaryDetailId == FinanceConsts.MybhgSelect_Nremjflr))
                    {
                        await _nrePricingAppService.GetResourcesManagementConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }
                }
                uow.Complete();
            }
        }
    }
}
