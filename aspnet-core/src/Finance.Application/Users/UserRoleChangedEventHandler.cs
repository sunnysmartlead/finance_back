using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Finance.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Finance.Users
{
    /// <summary>
    /// 用户角色更新事件
    /// </summary>
    public class UserRoleChangedEventHandler : IEventHandler<EntityChangedEventData<UserRole>>, ITransientDependency
    {
        /// <summary>
        /// 用户角色更新事件
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(EntityChangedEventData<UserRole> eventData)
        {
            QueryCacheManager.ExpireTag(FinanceConsts.UserRoleCacheName);
        }
    }
}
