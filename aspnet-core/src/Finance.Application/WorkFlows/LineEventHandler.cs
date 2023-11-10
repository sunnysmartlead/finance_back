using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Entering;
using Finance.NerPricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 工作流线事件
    /// </summary>
    public class LineEventHandler : IEventHandler<EntityUpdatedEventData<LineInstance>>, ITransientDependency
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
        public async void HandleEvent(EntityUpdatedEventData<LineInstance> eventData)
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
                    if (eventData.Entity.LineId == "主流程_核价看板_主流程_电子BOM匹配修改")
                    {
                        await _resourceEnteringAppService.GetElectronicConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    //如果是流转到主流程_结构BOM匹配修改
                    if (eventData.Entity.LineId == "主流程_核价看板_主流程_结构BOM匹配修改")
                    {
                        await _resourceEnteringAppService.GetStructuralConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                    //如果是流转到主流程_NRE模具费录入
                    if (eventData.Entity.LineId == "主流程_核价看板_主流程_NRE模具费录入")
                    {
                        await _nrePricingAppService.GetResourcesManagementConfigurationState(eventData.Entity.WorkFlowInstanceId);
                    }

                }
                uow.Complete();
            }
        }
    }
}
