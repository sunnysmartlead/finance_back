using Abp.Domain.Uow;
using Finance.NerPricing;
using Finance.NrePricing.Dto;
using Finance.NrePricing.Model;
using Finance.PropertyDepartment.DemandApplyAudit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests.NRE
{
    /// <summary>
    /// NRE测试用例
    /// </summary>
    public class NreAppService_Tests: FinanceTestBase
    { 
        private readonly NrePricingAppService _nrePricingAppService;
        public NreAppService_Tests()
        {
            var unitOfWorkManager = Resolve<IUnitOfWorkManager>().Begin();
            _nrePricingAppService = Resolve<NrePricingAppService>();
        }
        [Fact]
        public async Task Nre_Tests()
        {
            /// <summary>
            /// Nre 品保部=>环境实验费 录入(单个零件)
            /// </summary>
            /// <returns></returns>
            await _nrePricingAppService.PostExperimentItemsSingle(new ExperimentItemsSingleDto() { AuditFlowId=1, SolutionId=1, EnvironmentalExperimentFeeModels=new List<EnvironmentalExperimentFeeModel>() { new EnvironmentalExperimentFeeModel() { ProjectName="测试",IsThirdParty=true,UnitPrice=55,AdjustmentCoefficient=1,Unit="测试",CountBottomingOut=3,CountDV=5,CountPV=7,AllCost=9,Remark="测试"} } });
            /// <summary>
            /// Nre 品保部=>环境实验费 录入过的值(单个零件)
            /// </summary>
            /// <returns></returns>
            await _nrePricingAppService.GetReturnExperimentItemsSingle(1, 1);      
            /// <summary>
            ///  Nre 品保部=>环境实验费 导出数据(传数据)
            /// </summary>
            /// <param name="environmentalExperimentFeeModels"></param>
            /// <returns></returns>
            /// <exception cref="FriendlyException"></exception>
            await _nrePricingAppService.PostExportOfEnvironmentalExperimentFeeForm(new List<EnvironmentalExperimentFeeModel>() { new EnvironmentalExperimentFeeModel() { ProjectName = "测试", IsThirdParty = true, UnitPrice = 55, AdjustmentCoefficient = 1, Unit = "测试", CountBottomingOut = 3, CountDV = 5, CountPV = 7, AllCost = 9, Remark = "测试" } });
            /// <summary>
            ///  Nre 品保部=>环境实验费 导出数据(不传数据)
            /// </summary>      
            /// <returns></returns>
            /// <exception cref="FriendlyException"></exception>
            await _nrePricingAppService.GetExportOfEnvironmentalExperimentFeeForm(1, 1);
        }
    }
}
