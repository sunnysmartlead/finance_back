using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Authorization.Users;
using Finance.WorkFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Finance.Users
{
    /// <summary>
    /// 用户更新事件
    /// </summary>
    public class UserChangedEventHandler : IEventHandler<EntityChangedEventData<User>>, ITransientDependency
    {
        /// <summary>
        /// 用户更新事件
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityChangedEventData<User> eventData)
        {
            QueryCacheManager.ExpireTag(FinanceConsts.UserCacheName);
        }
    }
}
