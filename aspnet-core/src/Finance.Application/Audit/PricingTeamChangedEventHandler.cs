using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
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
    /// 核价团队更新事件
    /// </summary>
    public class PricingTeamChangedEventHandler : IEventHandler<EntityChangedEventData<PricingTeam>>, ITransientDependency
    {
        /// <summary>
        /// 核价团队更新事件
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityChangedEventData<PricingTeam> eventData)
        {
            QueryCacheManager.ExpireTag($"{FinanceConsts.PricingTeamCacheName}{eventData.Entity.AuditFlowId}");
        }
    }
}
