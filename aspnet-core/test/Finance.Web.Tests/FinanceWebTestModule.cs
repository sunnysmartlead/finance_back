using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Finance.EntityFrameworkCore;
using Finance.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Finance.Web.Tests
{
    [DependsOn(
        typeof(FinanceWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class FinanceWebTestModule : AbpModule
    {
        public FinanceWebTestModule(FinanceEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FinanceWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(FinanceWebMvcModule).Assembly);
        }
    }
}