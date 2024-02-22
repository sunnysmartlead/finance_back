using System;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Abp.Configuration.Startup;
using Abp.Net.Mail;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.EntityFrameworkCore;
using Finance.EntityFrameworkCore;
using Finance.Tests.DependencyInjection;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;
using Finance.EntityFrameworkCore.Seed;
using Abp.Domain.Uow;
using Finance.Ext;
using Finance.Web.Host.Controllers;

namespace Finance.Tests
{
    [DependsOn(
        typeof(FinanceApplicationModule),
        typeof(FinanceEntityFrameworkModule),
        typeof(AbpTestBaseModule)
        )]
    public class FinanceTestModule : AbpModule
    {
        public FinanceTestModule(FinanceEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;
        }

        public override void PreInitialize()
        {
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
            Configuration.UnitOfWork.IsTransactional = false;

            // Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            RegisterFakeService<AbpZeroDbMigrator<FinanceDbContext>>();

            Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);

            IocManager.IocContainer.Register(
                Component.For<IWebHostEnvironment>()
                    .UsingFactoryMethod(() => new TestHostEnvironment())
                    .LifestyleSingleton()

                    //, Component.For<IUnitOfWorkManager>()
                    //.UsingFactoryMethod(() => new UnitOfWorkManagerController().GetIUnitOfWorkManager())
                    //.LifestyleSingleton()
            );

        }

        public override void Initialize()
        {
            var thisAssembly = typeof(FinanceApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            ServiceCollectionRegistrar.Register(IocManager);
        }

        private void RegisterFakeService<TService>() where TService : class
        {
            IocManager.IocContainer.Register(
                Component.For<TService>()
                    .UsingFactoryMethod(() => Substitute.For<TService>())
                    .LifestyleSingleton()
            );
        }
    }
}
