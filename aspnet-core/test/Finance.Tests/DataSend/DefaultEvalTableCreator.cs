using Finance.BaseLibrary;
using Finance.DemandApplyAudit;
using Finance.EngineeringDepartment;
using Finance.EntityFrameworkCore;
using Finance.FinanceMaintain;
using Finance.FinanceParameter;
using Finance.Processes;
using Finance.TradeCompliance;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Tests.DataSend
{
    public class DefaultEvalTableCreator
    {
        private readonly FinanceDbContext _context;
        public DefaultEvalTableCreator(FinanceDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateEvalTable();
        }

        private void CreateEvalTable()
        {
            //添加solution表
            var solutionList = new List<Solution>
            {
                new Solution { Id = 1,  AuditFlowId = 1, Productld = 1, ModuleName = "广丰OMS", SolutionName = "AA", Product = "广丰OMS-AA", IsCOB = true, ElecEngineerId = 1, StructEngineerId = 1, IsFirst = true },
                new Solution { Id = 2,  AuditFlowId = 1, Productld = 1, ModuleName = "广丰OMS", SolutionName = "BB", Product = "广丰OMS-BB", IsCOB = true, ElecEngineerId = 1, StructEngineerId = 1, IsFirst = false },
            };
            var noDb = solutionList.Where(p => !_context.Solution.Contains(p));
            if (noDb.Any())
            {
                _context.Solution.AddRange(noDb);
            }

            //添加UPH值
            var uphList = new List<ProcessHoursEnterUph>
            {
                new ProcessHoursEnterUph {  Id = 1, AuditFlowId = 1, SolutionId=1, Uph= "zcuph",Value=180, ModelCountYearId=2 },
            };
            var uphNoDb = uphList.Where(p => !_context.ProcessHoursEnterUph.Contains(p));
            if (uphNoDb.Any())
            {
                _context.ProcessHoursEnterUph.AddRange(uphNoDb);
            }

            //添加ProcessHoursEnter
            var processHoursEnterList = new List<ProcessHoursEnter>
            {
                new ProcessHoursEnter {  Id = 1, AuditFlowId = 1, SolutionId=1,  DevelopTotalPrice = "900000",DeviceTotalPrice = 800000,
                FixtureName = "0",FixtureNumber = 0, FixturePrice = 0,FrockName = "0",FrockNumber = 0,FrockPrice= 0 ,
               HardwareDeviceTotalPrice = 10000, HardwareTotalPrice = 0,OpenDrawingSoftware = "上下壳组件组装",  ProcessNumber="18",
                SoftwarePrice = 0,TestLineName = "0",TestLineNumber = 0,TestLinePrice = 0,TraceabilitySoftware = "追溯软件1", TraceabilitySoftwareCost=10000 },
            };
            var processHoursEnterNoDb = processHoursEnterList.Where(p => !_context.ProcessHoursEnter.Contains(p));
            if (processHoursEnterNoDb.Any())
            {
                _context.ProcessHoursEnter.AddRange(processHoursEnterNoDb);
            }

            //添加制造成本参数
            var manufacturingCostInfoList = new List<ManufacturingCostInfo>
            {
                new () {Id = 1, MonthlyWorkingDays=26, AverageWage = 6684, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2023},
                new () {Id = 2, MonthlyWorkingDays=26, AverageWage = 7085, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2024},
                new () {Id = 3, MonthlyWorkingDays=26, AverageWage = 7510, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2025},
                new () {Id = 4, MonthlyWorkingDays=26, AverageWage = 7961, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2026},
                new () {Id = 5, MonthlyWorkingDays=26, AverageWage = 8439, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2027},
                new () {Id = 6, MonthlyWorkingDays=26, AverageWage = 8945, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2028},
                new () {Id = 7, MonthlyWorkingDays=26, AverageWage = 9482, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2029},
                new () {Id = 8, MonthlyWorkingDays=26, AverageWage = 10051, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2030},
                new () {Id = 9, MonthlyWorkingDays=26, AverageWage = 10654, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2031},
                new (){Id = 10, MonthlyWorkingDays=26, AverageWage = 11293, WorkingHours = 10.5, RateOfMobilization = 85,  UsefulLifeOfFixedAssets = 5, DailyShift = 2, VatRate = 13,  TraceLineOfPerson=2,ProcessCost = 0.2M, CapacityUtilizationRate = 80, Year = 2032},
            };
            var manufacturingCostInfoNoDb = manufacturingCostInfoList.Where(p => !_context.ManufacturingCostInfo.Contains(p));
            if (manufacturingCostInfoNoDb.Any())
            {
                _context.ManufacturingCostInfo.AddRange(manufacturingCostInfoNoDb);
            }


            //添加FollowLineTangent
            var followLineTangentList = new List<FollowLineTangent>
            {
                new FollowLineTangent {Id = 1,Year=2022, LaborHour = 12, PerFollowUpQuantity=2},
                new FollowLineTangent {Id = 2,Year=2023, LaborHour = 12, PerFollowUpQuantity=2},
                new FollowLineTangent {Id = 3,Year=2024, LaborHour = 10, PerFollowUpQuantity=2},
                new FollowLineTangent {Id = 4,Year=2025, LaborHour = 8, PerFollowUpQuantity=2},
                new FollowLineTangent {Id = 5,Year=2026, LaborHour = 4, PerFollowUpQuantity=2},
                new FollowLineTangent {Id = 6,Year=2027, LaborHour = 4, PerFollowUpQuantity=2},
                new FollowLineTangent {Id = 7,Year=2028, LaborHour = 4, PerFollowUpQuantity=2},
                new FollowLineTangent {Id = 8,Year=2029, LaborHour = 4, PerFollowUpQuantity=2},
                new FollowLineTangent {Id = 9,Year=2030, LaborHour = 4, PerFollowUpQuantity=2},
            };
            var followLineTangentNoDb = followLineTangentList.Where(p => !_context.FollowLineTangent.Contains(p));
            if (followLineTangentNoDb.Any())
            {
                _context.FollowLineTangent.AddRange(followLineTangentNoDb);
            }

            //添加财务费率
            var rateEntryInfoList = new List<RateEntryInfo>
            {
                new RateEntryInfo {Id = 14, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2023},
                new RateEntryInfo {Id = 15, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2024},
                new RateEntryInfo {Id = 16, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2025},
                new RateEntryInfo {Id = 17, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2026},
                new RateEntryInfo {Id = 18, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2027},
                new RateEntryInfo {Id = 19, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2028},
                new RateEntryInfo {Id = 20, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2029},
                new RateEntryInfo {Id = 21, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2030},
                new RateEntryInfo {Id = 41, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2031},
                new RateEntryInfo {Id = 42, DirectManufacturingRate=11.18M,IndirectLaborRate=20.23M,IndirectDepreciationRate=3.46M,IndirectManufacturingRate=41.45M,Year=2032},
            };
            var rateEntryInfoNoDb = rateEntryInfoList.Where(p => !_context.RateEntryInfo.Contains(p));
            if (rateEntryInfoNoDb.Any())
            {
                _context.RateEntryInfo.AddRange(rateEntryInfoNoDb);
            }

            //添加汇率录入表
            var exchangeRateList = new List<ExchangeRate>
            {
                new ExchangeRate {Id = 1,  ExchangeRateKind="EUR", ExchangeRateValue="[{\"Year\":2023,\"UpDown\":null,\"Value\":8.0},{\"Year\":2024,\"UpDown\":null,\"Value\":7.8},{\"Year\":2025,\"UpDown\":null,\"Value\":7.6},{\"Year\":2026,\"UpDown\":null,\"Value\":7.6},{\"Year\":2027,\"UpDown\":null,\"Value\":7.6},{\"Year\":2028,\"UpDown\":null,\"Value\":7.6},{\"Year\":2029,\"UpDown\":null,\"Value\":7.6},{\"Year\":2030,\"UpDown\":null,\"Value\":7.6},{\"Year\":2031,\"UpDown\":null,\"Value\":7.6}]"},
                new ExchangeRate {Id = 2,  ExchangeRateKind="USD", ExchangeRateValue="[{\"Year\":2023,\"UpDown\":null,\"Value\":7.0},{\"Year\":2024,\"UpDown\":null,\"Value\":7.1},{\"Year\":2025,\"UpDown\":null,\"Value\":7.15},{\"Year\":2026,\"UpDown\":null,\"Value\":7.2},{\"Year\":2027,\"UpDown\":null,\"Value\":7.2},{\"Year\":2028,\"UpDown\":null,\"Value\":7.2},{\"Year\":2029,\"UpDown\":null,\"Value\":7.2},{\"Year\":2030,\"UpDown\":null,\"Value\":7.2},{\"Year\":2031,\"UpDown\":null,\"Value\":7.2}]"},
                new ExchangeRate {Id = 3,  ExchangeRateKind="CNY", ExchangeRateValue="[{\"Year\":2023,\"UpDown\":null,\"Value\":1.0},{\"Year\":2024,\"UpDown\":null,\"Value\":1.0},{\"Year\":2025,\"UpDown\":null,\"Value\":1.0},{\"Year\":2026,\"UpDown\":null,\"Value\":1.0},{\"Year\":2027,\"UpDown\":null,\"Value\":1.0},{\"Year\":2028,\"UpDown\":null,\"Value\":1.0},{\"Year\":2029,\"UpDown\":null,\"Value\":1.0},{\"Year\":2030,\"UpDown\":null,\"Value\":1.0},{\"Year\":2031,\"UpDown\":null,\"Value\":1.0}]"},
            };
            var exchangeRateNoDb = exchangeRateList.Where(p => !_context.ExchangeRate.Contains(p));
            if (exchangeRateNoDb.Any())
            {
                _context.ExchangeRate.AddRange(exchangeRateNoDb);
            }

            //logisticscost
            var logisticscostList = new List<Logisticscost>
            {
                new Logisticscost {Id = 1,   AuditFlowId=1, Classification="176.4", FreightPrice =1000, MonthlyDemandPrice =4.37M,
                PackagingPrice=0.01M, SolutionId=1,SinglyDemandPrice=343.38M, StoragePrice=500, TransportPrice=343.39M, Status=1, ModelCountYearId=2},
            };
            var logisticscostNoDb = logisticscostList.Where(p => !_context.Logisticscost.Contains(p));
            if (logisticscostNoDb.Any())
            {
                _context.Logisticscost.AddRange(logisticscostNoDb);
            }


            _context.SaveChanges();

        }
    }
}
