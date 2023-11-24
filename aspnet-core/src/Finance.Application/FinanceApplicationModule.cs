using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Abp.Web;
using Finance.Authorization;
using Finance.MakeOffers.AnalyseBoard.Method;
using Finance.NrePricing.Method;
using Finance.PropertyDepartment.DemandApplyAudit.Method;
using Finance.PropertyDepartment.Entering.Method;
using Finance.PropertyDepartment.UnitPriceLibrary;
using Finance.UpdateLog.Method;
using Finance.WorkFlows;
using Microsoft.EntityFrameworkCore;

namespace Finance
{
    [DependsOn(
        typeof(FinanceCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpWebCommonModule))]
    public class FinanceApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<FinanceAuthorizationProvider>();

            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(UnitPriceLibraryMapper.CreateMappings);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(EnteringMapper.CreateMappings);
            Configuration.Modules.AbpAutoMapper().Configurators.Add(NrePricingMapper.CreateMappings);
            Configuration.Modules.AbpAutoMapper().Configurators.Add(AnalysisBoardMapper.CreateMappings);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(UpdateLogMapper.CreateMappings);
            //营销部审核
            Configuration.Modules.AbpAutoMapper().Configurators.Add(AuditMapper.CreateMappings);
            Configuration.Modules.AbpWebCommon().SendAllExceptionsToClients = true;

        }

        public override void Initialize()
        {
            var thisAssembly = typeof(FinanceApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }

        public override void PostInitialize()
        {
            //注册定时任务
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<TradeComplianceWorker>());
        }
    }
}
