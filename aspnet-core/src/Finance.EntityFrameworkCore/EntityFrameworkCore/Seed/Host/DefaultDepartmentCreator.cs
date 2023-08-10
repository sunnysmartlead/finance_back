using Abp.MultiTenancy;
using Finance.Authorization.Roles;
using Finance.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EntityFrameworkCore.Seed.Host
{
    public class DefaultDepartmentCreator
    {
        private readonly FinanceDbContext _context;

        public DefaultDepartmentCreator(FinanceDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateRoles();
        }

        private void CreateRoles()
        {
            var tenantId = FinanceConsts.MultiTenancyEnabled ? null : (int?)MultiTenancyConsts.DefaultTenantId;

            var deptList = new List<Department>
            {
                new Department{  Name="总经理室" },
                new Department{  Name="管理部" },
                new Department{  Name="工程技术部" },
                new Department{  Name="品质保证部" },
                new Department{  Name="生产管理部" },
                new Department{  Name="产品开发部" },
                new Department{  Name="项目管理部" },
                new Department{  Name="财务部" },
                new Department{  Name="营销二部" },
                new Department{  Name="营销一部" },
                new Department{  Name="市场部" },
                new Department{  Name="系统集成部" },
            };

            var dept = _context.Department.Select(p => p.Name).ToList();
            var noDb = deptList.Select(p => p.Name).Where(p => !dept.Contains(p));
            if (noDb.Any())
            {
                _context.Department.AddRange(deptList.Where(p => noDb.Contains(p.Name)));
                _context.SaveChanges();
            }
        }
    }
}
