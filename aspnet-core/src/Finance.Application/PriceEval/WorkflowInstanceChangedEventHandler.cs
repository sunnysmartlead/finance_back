using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Authorization.Roles;
using Finance.WorkFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Finance.PriceEval
{
    /// <summary>
    /// 流程实例事件
    /// </summary>
    public class WorkflowInstanceChangedEventHandler : IEventHandler<EntityChangedEventData<WorkflowInstance>>, ITransientDependency
    {
        /// <summary>
        /// 流程实例事件
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityChangedEventData<WorkflowInstance> eventData)
        {
            QueryCacheManager.ExpireTag($"{FinanceConsts.WorkflowInstanceCacheName}{eventData.Entity.Id}");
        }
    }
}
