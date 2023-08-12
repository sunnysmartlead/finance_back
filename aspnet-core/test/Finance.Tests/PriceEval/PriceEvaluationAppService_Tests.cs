//using Finance.FinanceMaintain;
//using Finance.PriceEval;
//using Finance.PriceEval.Dto;
//using Finance.Users;
//using NPOI.SS.Formula.Functions;
//using NPOI.Util;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace Finance.Tests.PriceEval
//{
//    public class PriceEvaluationAppService_Tests : FinanceTestBase
//    {
//        private readonly PriceEvaluationAppService _priceEvaluationAppService;

//        public PriceEvaluationAppService_Tests()
//        {
//            _priceEvaluationAppService = Resolve<PriceEvaluationAppService>();
//        }

//        [Fact]
//        public async Task PriceEvaluationStart_Test()
//        {
//            // Act
//            var output = await _priceEvaluationAppService.PriceEvaluationStart(new PriceEvaluationStartInput
//            {
//                ProjectName="123",
//                ProjectCode="123",
//                Title = "程序自动化测试",
//                Drafter = "admin",
//                DrafterNumber = 1,
//                DraftingDepartment = "123",
//                DraftingCompany = "123",
//                DraftDate = DateTime.Now,
//                Number = "123",
//                CustomerNature = "123",
//                SopTime = 2023,
//                ProjectCycle = 3,
//                UpdateFrequency = "All",
//                Pcs = new List<CreatePcsDto> { new CreatePcsDto { CarFactory = "123", CarModel = "123", Kv = 1, PcsType = PcsType.Input, PcsYearList = new List<CreatePcsYearDto> { new CreatePcsYearDto { Quantity = 1, UpDown = YearType.Year, Year = 2023 } } } },
//                ModelCount = new List<CreateModelCountDto> { new CreateModelCountDto {Order = 1, Product = "123", ProductType = "123", Pixel = "123", MarketShare = 1, ModuleCarryingRate = 1, SingleCarProductsQuantity = 1, ModelCountYearList = new List<CreateModelCountYearDto> { new CreateModelCountYearDto { Year = 2023, UpDown = YearType.Year, Quantity = 1 } } } },
//                Requirement = new List<CreateRequirementDto> { new CreateRequirementDto { AnnualDeclineRate=1, AnnualRebateRequirements =1, OneTimeDiscountRate =1, CommissionRate =1, Year =2023} },
//                AllocationOfMouldCost = false,
//                AllocationOfFixtureCost = false,
//                FrockCost = false,
//                AllocationOfEquipmentCost = false,
//                FixtureCost = false,
//                ExperimentCost = false,
//                TestCost = false,
//                TravelCost = false,
//                OtherCost = false,
//                LandingFactory = "123",
//                ProductInformation = new List<CreateColumnFormatProductInformationDto> { new CreateColumnFormatProductInformationDto { Name = "123", Product = "123", CustomerTargetPrice = 1, SensorCurrency=1, SensorExchangeRate=1, LensCurrency=1, LensExchangeRate=1, IspCurrency=1, IspExchangeRate=1, SerialChipCurrency=1, SerialChipExchangeRate=1, CableCurrency=1, CableExchangeRate=1, OtherCurrency=1, OtherExchangeRate=1, } },
//                CustomerTargetPrice = new List<CreateCustomerTargetPriceDto> { new CreateCustomerTargetPriceDto { Kv =1, Product ="123", TargetPrice ="123", Currency =1, ExchangeRate =1} },
//                TradeMode = "123",
//                SalesType = "123",
//                PaymentMethod = "123",
//                ShippingType = "123",
//                PackagingType = "123",
//                PlaceOfDelivery = "123",
//                Country = "123",
//                Deadline = DateTime.Now,
//                ProjectManager = 1,
//                SorFile = new List<long> { 1 }
//            });

//            // Assert
//            output.IsSuccess.CompareTo(true);//.Items.Count.ShouldBeGreaterThan(0);

//            // Act
//            var outputResult = await _priceEvaluationAppService.GetPriceEvaluationStartData(output.AuditFlowId);

//            // Assert
//            outputResult.Title.CompareTo("程序自动化测试");
//        }

//    }
//}
