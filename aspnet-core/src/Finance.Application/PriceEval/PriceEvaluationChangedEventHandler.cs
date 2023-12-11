using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Finance.PriceEval
{
    /// <summary>
    /// 核价需求录入数据更新事件
    /// </summary>
    public class PriceEvaluationChangedEventHandler : IEventHandler<EntityChangedEventData<PriceEvaluation>>, ITransientDependency
    {
        /// <summary>
        /// 核价需求录入数据更新事件
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityChangedEventData<PriceEvaluation> eventData)
        {
            QueryCacheManager.ExpireTag($"{FinanceConsts.PriceEvaluationCacheName}{eventData.Entity.AuditFlowId}");
        }
    }
}
