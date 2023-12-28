using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Finance.Authorization.Roles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EntityFrameworkCore.Seed.Tenants
{
    public class AdminRoleBuilder
    {
        private readonly FinanceDbContext _context;
        public AdminRoleBuilder(FinanceDbContext context)
        {
            _context = context;
        }
        public void Create()
        {
            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == MultiTenancyConsts.DefaultTenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser is null)
            {
                return;
            }

            var timelinessRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == MultiTenancyConsts.DefaultTenantId && r.Name == StaticRoleNames.Host.Timeliness);
            if (timelinessRole is not null)
            {
                if (!_context.UserRoles.IgnoreQueryFilters().Any(p => p.TenantId == MultiTenancyConsts.DefaultTenantId && p.UserId == adminUser.Id && p.RoleId == timelinessRole.Id))
                {
                    _context.UserRoles.Add(new UserRole(MultiTenancyConsts.DefaultTenantId, adminUser.Id, timelinessRole.Id));
                }
            }

            var projectManagerRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == MultiTenancyConsts.DefaultTenantId && r.Name == StaticRoleNames.Host.ProjectManager);
            if (projectManagerRole is not null)
            {
                if (!_context.UserRoles.IgnoreQueryFilters().Any(p => p.TenantId == MultiTenancyConsts.DefaultTenantId && p.UserId == adminUser.Id && p.RoleId == projectManagerRole.Id))
                {
                    _context.UserRoles.Add(new UserRole(MultiTenancyConsts.DefaultTenantId, adminUser.Id, projectManagerRole.Id));
                }
            }
        }
    }
}
