using System;
using System.Collections.Generic;
using Abp.Authorization.Users;
using Abp.Extensions;

namespace Finance.Authorization.Users
{
    public class User : AbpUser<User>
    {
        /// <summary>
        /// 该用户所属部门
        /// </summary>
        public virtual long DepartmentId { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public virtual string Position { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public virtual long Number { get; set; }


        public const string DefaultPassword = "123qwe";

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
