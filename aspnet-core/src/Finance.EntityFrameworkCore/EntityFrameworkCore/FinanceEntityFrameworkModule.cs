using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Finance.EntityFrameworkCore.Seed;

namespace Finance.EntityFrameworkCore
{
    [DependsOn(
        typeof(FinanceCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class FinanceEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }
        /// <summary>
        /// 这是在应用程序启动时调用的第一个事件。代码可以放在这里在依赖项注入注册之前运行。
        /// </summary>
        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<FinanceDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        FinanceDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        FinanceDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                    //TODO: Debug 用完需注释，避免影响整体性能
                    options.DbContextOptions.EnableSensitiveDataLogging();
                });
            }
        }
        /// <summary>
        /// 此方法用于注册此模块的依赖项。
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FinanceEntityFrameworkModule).GetAssembly());
        }
        /// <summary>
        /// 此方法最后在应用程序启动时调用。
        /// </summary>
        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
