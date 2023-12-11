using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Audit;
using Finance.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Finance.PriceEval
{
    /// <summary>
    /// 核价开始数据更新事件
    /// </summary>
    public class PriceEvaluationStartDataChangedEventHandler : IEventHandler<EntityChangedEventData<PriceEvaluationStartData>>, ITransientDependency
    {
        /// <summary>
        /// 核价开始数据更新事件
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityChangedEventData<PriceEvaluationStartData> eventData)
        {
            QueryCacheManager.ExpireTag($"{FinanceConsts.PriceEvaluationStartDataCacheName}{eventData.Entity.AuditFlowId}");
        }
    }
}
