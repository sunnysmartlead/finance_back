using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Finance.EntityFrameworkCore.Seed.Host;
using Finance.EntityFrameworkCore.Seed.Tenants;
using Finance.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Uow;

namespace Finance.Tests.DataSend
{
    internal class DataSeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<FinanceDbContext>(iocResolver, SeedHostDb);
        }

        public static void SeedHostDb(FinanceDbContext context, IUnitOfWorkManager unitOfWorkManager)
        {
            context.SuppressAutoSetTenantId = true;

            // Host seed
            new DataInitialHostDbBuilder(context, unitOfWorkManager).Create();
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext, IUnitOfWorkManager> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                //uowManager.Object
                using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
                {
                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                    contextAction(context, uowManager.Object);

                    uow.Complete();
                }
            }
        }
    }
}
