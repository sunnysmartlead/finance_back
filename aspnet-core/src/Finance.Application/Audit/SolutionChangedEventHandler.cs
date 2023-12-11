using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Authorization.Roles;
using Finance.DemandApplyAudit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Finance.Audit
{
    /// <summary>
    /// 方案更新事件
    /// </summary>
    public class SolutionChangedEventHandler : IEventHandler<EntityChangedEventData<Solution>>, ITransientDependency
    {
        /// <summary>
        /// 方案更新事件
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityChangedEventData<Solution> eventData)
        {
            QueryCacheManager.ExpireTag($"{FinanceConsts.SolutionCacheName}{eventData.Entity.AuditFlowId}");
        }
    }
}
