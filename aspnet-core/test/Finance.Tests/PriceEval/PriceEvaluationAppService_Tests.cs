using Abp.Json;

using Finance.EntityFrameworkCore.Seed;

using Finance.FinanceMaintain;

using Finance.PriceEval;
using Finance.PriceEval.Dto;
using NPOI.SS.Formula.Functions;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests.PriceEval
{
    //[TestCaseOrderer("Finance.Tests.PriorityOrderer", "Finance.Tests")]
    public class PriceEvaluationAppService_Tests : FinanceTestBase
    {
        private readonly PriceEvaluationAppService _priceEvaluationAppService;

        public PriceEvaluationAppService_Tests()
        {
            _priceEvaluationAppService = Resolve<PriceEvaluationAppService>();
        }

        /// <summary>
        /// 测试当前年份是否使用了其他年份的成本
        /// </summary>
        /// <returns></returns>
        [Fact]//, TestPriority(2)
        public async Task IsHasCostFunc_Test()
        {
            _priceEvaluationAppService.IsHasCostFunc(2023, YearType.Year, 2023, 1.5M).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2024, YearType.Year, 2023, 1.5M).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2025, YearType.Year, 2023, 1.5M).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2026, YearType.Year, 2023, 1.5M).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2027, YearType.Year, 2023, 1.5M).CompareTo(false);

            _priceEvaluationAppService.IsHasCostFunc(2023, YearType.Year, 2023, 2).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2024, YearType.Year, 2023, 2).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2025, YearType.Year, 2023, 2).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2026, YearType.Year, 2023, 2).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2027, YearType.Year, 2023, 2).CompareTo(false);

            _priceEvaluationAppService.IsHasCostFunc(2023, YearType.FirstHalf, 2023, 2).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2023, YearType.SecondHalf, 2023, 2).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2024, YearType.FirstHalf, 2023, 2).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2024, YearType.SecondHalf, 2023, 2).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2025, YearType.FirstHalf, 2023, 2).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2025, YearType.SecondHalf, 2023, 2).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2026, YearType.FirstHalf, 2023, 2).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2026, YearType.SecondHalf, 2023, 2).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2027, YearType.FirstHalf, 2023, 2).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2027, YearType.SecondHalf, 2023, 2).CompareTo(false);

            _priceEvaluationAppService.IsHasCostFunc(2023, YearType.FirstHalf, 2023, 1.5M).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2023, YearType.SecondHalf, 2023, 1.5M).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2024, YearType.FirstHalf, 2023, 1.5M).CompareTo(true);
            _priceEvaluationAppService.IsHasCostFunc(2024, YearType.SecondHalf, 2023, 1.5M).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2025, YearType.FirstHalf, 2023, 1.5M).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2025, YearType.SecondHalf, 2023, 1.5M).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2026, YearType.FirstHalf, 2023, 1.5M).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2026, YearType.SecondHalf, 2023, 1.5M).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2027, YearType.FirstHalf, 2023, 1.5M).CompareTo(false);
            _priceEvaluationAppService.IsHasCostFunc(2027, YearType.SecondHalf, 2023, 1.5M).CompareTo(false);
        }

        /// <summary>
        /// 测试核价需求录入和核价需求录入数据获取
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PriceEvaluationStart_Test()
        {
            var input = " {\r\n    \"files\": [\r\n      {\r\n        \"fileName\": \"NRE核价表-247.xlsx\",\r\n        \"fileUrl\": null,\r\n        \"fileLength\": 0,\r\n        \"fileType\": null,\r\n        \"fileId\": 939\r\n      }\r\n    ],\r\n    \"isSubmit\": true,\r\n    \"quickQuoteAuditFlowId\": null,\r\n    \"title\": \"2024-01-30营销二部关于YY广丰舱内第2版的核价报价申请\",\r\n    \"drafter\": \"吴昕桐\",\r\n    \"drafterNumber\": 20057744,\r\n    \"draftingDepartment\": \"营销二部\",\r\n    \"draftingCompany\": \"未成功获取\",\r\n    \"draftDate\": \"2024-01-30T11:14:50.747\",\r\n    \"number\": \"BJHJ-ZL20240130-001\",\r\n    \"projectName\": \"广丰舱内\",\r\n    \"projectCode\": \"JDT-005\",\r\n    \"quoteVersion\": 2,\r\n    \"customerCode\": null,\r\n    \"customerName\": \"YY\",\r\n    \"customerNature\": \"CustomerNature_Algorithm\",\r\n    \"terminalName\": null,\r\n    \"terminalNature\": null,\r\n    \"sopTime\": 2025,\r\n    \"projectCycle\": 3,\r\n    \"updateFrequency\": \"UpdateFrequency_Year\",\r\n    \"priceEvalType\": \"PriceEvalType_Quantity\",\r\n    \"isHasSample\": true,\r\n    \"sample\": [\r\n      {\r\n        \"name\": \"SampleName_B\",\r\n        \"pcs\": 66\r\n      }\r\n    ],\r\n    \"kValue\": 0.7,\r\n    \"pcs\": [\r\n      {\r\n        \"pcsType\": 0,\r\n        \"carFactory\": \"YY\",\r\n        \"carModel\": \"Y1\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 123\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 122\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 133\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"pcsType\": 0,\r\n        \"carFactory\": \"YY\",\r\n        \"carModel\": \"Y2\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 123\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 122\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 133\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"pcsType\": 1,\r\n        \"carFactory\": \"YY\",\r\n        \"carModel\": \"Y1\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 86.1\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 85.4\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 93.1\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"pcsType\": 1,\r\n        \"carFactory\": \"YY\",\r\n        \"carModel\": \"Y2\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 86.1\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 85.4\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 93.1\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"carModelCount\": [\r\n      {\r\n        \"carFactory\": null,\r\n        \"carModel\": \"Y1\",\r\n        \"order\": 1,\r\n        \"partNumber\": \"123\",\r\n        \"code\": \"JDT-005-OM1\",\r\n        \"product\": \"广丰OMS\",\r\n        \"productType\": \"ProductType_CabinMonitoring\",\r\n        \"pixel\": \"2M\",\r\n        \"ourRole\": \"1\",\r\n        \"marketShare\": 100,\r\n        \"moduleCarryingRate\": 100,\r\n        \"singleCarProductsQuantity\": 1,\r\n        \"modelCountYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 86.1\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 85.4\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 93.1\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"carFactory\": null,\r\n        \"carModel\": \"Y2\",\r\n        \"order\": 2,\r\n        \"partNumber\": \"123\",\r\n        \"code\": \"JDT-005-OM1\",\r\n        \"product\": \"广丰OMS\",\r\n        \"productType\": \"ProductType_CabinMonitoring\",\r\n        \"pixel\": \"2M\",\r\n        \"ourRole\": \"1\",\r\n        \"marketShare\": 100,\r\n        \"moduleCarryingRate\": 100,\r\n        \"singleCarProductsQuantity\": 1,\r\n        \"modelCountYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 86.1\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 85.4\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 93.1\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"modelCount\": [\r\n      {\r\n        \"order\": 0,\r\n        \"partNumber\": \"123\",\r\n        \"code\": \"JDT-005-OM1\",\r\n        \"product\": \"广丰OMS\",\r\n        \"productType\": \"ProductType_CabinMonitoring\",\r\n        \"productTypeName\": \"舱内监测\",\r\n        \"pixel\": \"2M\",\r\n        \"modelCountYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 172.2\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 170.8\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 186.2\r\n          }\r\n        ],\r\n        \"sumQuantity\": 529.2\r\n      }\r\n    ],\r\n    \"isHasGradient\": false,\r\n    \"gradient\": [\r\n      {\r\n        \"index\": 0,\r\n        \"gradientValue\": 176.4,\r\n        \"displayGradientValue\": 176.4,\r\n        \"systermGradientValue\": 0\r\n      }\r\n    ],\r\n    \"gradientModel\": [\r\n      {\r\n        \"gradientValue\": 176.4,\r\n        \"index\": 0,\r\n        \"number\": \"123\",\r\n        \"code\": \"JDT-005-OM1\",\r\n        \"name\": \"广丰OMS\",\r\n        \"type\": \"ProductType_CabinMonitoring\",\r\n        \"gradientModelYear\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"count\": 172.2\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"count\": 170.8\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"count\": 186.2\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"requirement\": [\r\n      {\r\n        \"annualDeclineRate\": 0,\r\n        \"annualRebateRequirements\": 0,\r\n        \"oneTimeDiscountRate\": 1,\r\n        \"commissionRate\": 1,\r\n        \"year\": 2025,\r\n        \"upDown\": 0\r\n      },\r\n      {\r\n        \"annualDeclineRate\": 2,\r\n        \"annualRebateRequirements\": 1,\r\n        \"oneTimeDiscountRate\": 1,\r\n        \"commissionRate\": 0,\r\n        \"year\": 2026,\r\n        \"upDown\": 0\r\n      },\r\n      {\r\n        \"annualDeclineRate\": 2,\r\n        \"annualRebateRequirements\": 1,\r\n        \"oneTimeDiscountRate\": 1,\r\n        \"commissionRate\": 1,\r\n        \"year\": 2027,\r\n        \"upDown\": 0\r\n      }\r\n    ],\r\n    \"isHasNre\": false,\r\n    \"shareCount\": [],\r\n    \"allocationOfMouldCost\": true,\r\n    \"allocationOfFixtureCost\": true,\r\n    \"frockCost\": true,\r\n    \"allocationOfEquipmentCost\": true,\r\n    \"fixtureCost\": true,\r\n    \"experimentCost\": true,\r\n    \"testCost\": true,\r\n    \"travelCost\": true,\r\n    \"otherCost\": true,\r\n    \"landingFactory\": \"LandingFactory_SunnySmartLead\",\r\n    \"productInformation\": [\r\n      {\r\n        \"name\": \"广丰OMS\",\r\n        \"product\": \"广丰OMS\",\r\n        \"customerTargetPrice\": 0,\r\n        \"sensor\": null,\r\n        \"sensorTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"sensorPrice\": 0,\r\n        \"sensorCurrency\": 0,\r\n        \"sensorExchangeRate\": 0,\r\n        \"lens\": null,\r\n        \"lensTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"lensPrice\": 0,\r\n        \"lensCurrency\": 0,\r\n        \"lensExchangeRate\": 0,\r\n        \"isp\": null,\r\n        \"ispTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"ispPrice\": 0,\r\n        \"ispCurrency\": 0,\r\n        \"ispExchangeRate\": 0,\r\n        \"serialChip\": null,\r\n        \"serialChipTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"serialChipPrice\": 0,\r\n        \"serialChipCurrency\": 0,\r\n        \"serialChipExchangeRate\": 0,\r\n        \"cable\": null,\r\n        \"cableTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"cablePrice\": 0,\r\n        \"cableCurrency\": 0,\r\n        \"cableExchangeRate\": 0,\r\n        \"other\": null,\r\n        \"otherTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"otherPrice\": 0,\r\n        \"otherCurrency\": 0,\r\n        \"otherExchangeRate\": 0,\r\n        \"manufactureProcess\": null,\r\n        \"installationPosition\": null\r\n      }\r\n    ],\r\n    \"customerTargetPrice\": [\r\n      {\r\n        \"kv\": 176.4,\r\n        \"product\": \"广丰OMS\",\r\n        \"targetPrice\": null,\r\n        \"currency\": null,\r\n        \"exchangeRate\": null\r\n      }\r\n    ],\r\n    \"tradeMode\": \"d3c53e2d6b1b40dc824083ab6e3ce09a\",\r\n    \"salesType\": \"SalesType_ForTheDomesticMarket\",\r\n    \"paymentMethod\": \"11\",\r\n    \"customerSpecialRequest\": \"11\",\r\n    \"shippingType\": \"ShippingType_AirTransport\",\r\n    \"packagingType\": \"PackagingType_DisposableCarton\",\r\n    \"placeOfDelivery\": \"11\",\r\n    \"country\": \"其他国家\",\r\n    \"countryType\": \"2\",\r\n    \"deadline\": \"2024-01-30T16:00:00\",\r\n    \"projectManager\": 54,\r\n    \"sorFile\": [\r\n      939\r\n    ],\r\n    \"reason\": \"11\",\r\n    \"nodeInstanceId\": 0,\r\n    \"opinion\": \"EvalReason_Schj\",\r\n    \"comment\": null\r\n  }".FromJsonString<PriceEvaluationStartInput>();

            // Act
            var output = await _priceEvaluationAppService.PriceEvaluationStart(input);

            // Assert
            output.IsSuccess.CompareTo(true);//.Items.Count.ShouldBeGreaterThan(0);

            // Act
            var outputResult = await _priceEvaluationAppService.GetPriceEvaluationStartData(output.AuditFlowId);

            // Assert
            outputResult.Title.CompareTo("2024-01-30营销二部关于YY广丰舱内第2版的核价报价申请");
        }

        /// <summary>
        /// 测试损耗成本修改项和损耗成本修改项数据获取
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetUpdateItemLossCost_Test()
        {
            var input = "{\r\n  \"auditFlowId\": 9999999,\r\n  \"gradientId\": 9999999,\r\n  \"solutionId\": 9999999,\r\n  \"year\": 2040,\r\n  \"upDown\": 0,\r\n  \"updateItem\": [\r\n    {\r\n      \"year\": 0,\r\n      \"upDown\": 0,\r\n      \"editId\": \"包材\",\r\n      \"editNotes\": \"修改\",\r\n      \"name\": \"包材\",\r\n      \"wastageCost\": 0.01,\r\n      \"moqShareCount\": 0.000031376268,\r\n      \"id\": 2026\r\n    }\r\n  ],\r\n  \"file\": 0\r\n}"
                .FromJsonString<SetUpdateItemInput<List<LossCost>>>();

            // Act
            await _priceEvaluationAppService.SetUpdateItemLossCost(input);

            // Act
            var outputResult = await _priceEvaluationAppService.GetUpdateItemLossCost(new GetUpdateItemInput
            {
                AuditFlowId = input.AuditFlowId,
                GradientId = input.GradientId,
                SolutionId = input.SolutionId,
                UpDown = input.UpDown,
                Year = input.Year
            });
            // Assert
            outputResult.Count.ShouldBeEquivalentTo(1);
        }

        /// <summary>
        /// 测试制造成本修改项和制造成本修改项数据获取
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetUpdateItemManufacturingCost_Test()
        {
            var input = "{\r\n  \"auditFlowId\": 9999998,\r\n  \"gradientId\": 9999998,\r\n  \"solutionId\": 9999998,\r\n  \"year\": 2040,\r\n  \"upDown\": 0,\r\n  \"updateItem\":[{\"EditId\":\"SMT\",\"EditNotes\":\"测试数据\",\"CostType\":1,\"CostItem\":\"SMT制造成本（元）\",\"GradientKy\":0.0,\"ManufacturingCostDirect\":{\"DirectLabor\":0.0,\"EquipmentDepreciation\":0.0,\"LineChangeCost\":0.0,\"ManufacturingExpenses\":2.0,\"Subtotal\":2.0,\"Id\":2025},\"ManufacturingCostIndirect\":{\"DirectLabor\":0.0,\"EquipmentDepreciation\":0.0,\"ManufacturingExpenses\":0.0,\"Subtotal\":0.0,\"Id\":2025},\"Subtotal\":2.0,\"Id\":2025},{\"EditId\":\"GroupTest\",\"EditNotes\":\"测试数据\",\"CostType\":0,\"CostItem\":\"组测制造成本（元）\",\"GradientKy\":85400.0,\"ManufacturingCostDirect\":{\"DirectLabor\":0.5394325097791417,\"EquipmentDepreciation\":2.0,\"LineChangeCost\":2.0,\"ManufacturingExpenses\":2.0,\"Subtotal\":6.539432509779141,\"Id\":2025},\"ManufacturingCostIndirect\":{\"DirectLabor\":0.06144444444444445,\"EquipmentDepreciation\":0.027444444444444445,\"ManufacturingExpenses\":0.2758888888888889,\"Subtotal\":0.3647777777777778,\"Id\":2025},\"Subtotal\":6.904210287556919,\"Id\":2025}],\r\n  \"file\": 0\r\n}"
    .FromJsonString<SetUpdateItemInput<List<ManufacturingCost>>>();

            // Act
            await _priceEvaluationAppService.SetUpdateItemManufacturingCost(input);

            // Act
            var outputResult = await _priceEvaluationAppService.GetUpdateItemManufacturingCost(new GetUpdateItemInput
            {
                AuditFlowId = input.AuditFlowId,
                GradientId = input.GradientId,
                SolutionId = input.SolutionId,
                UpDown = input.UpDown,
                Year = input.Year
            });
            // Assert
            outputResult.Count.ShouldBeEquivalentTo(2);
        }

        /// <summary>
        /// 测试物流成本修改项和物流成本修改项数据获取
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetUpdateItemLogisticsCost_Test()
        {
            var input = "{\r\n  \"auditFlowId\": 9999997,\r\n  \"gradientId\": 9999997,\r\n  \"solutionId\": 9999997,\r\n  \"year\": 2040,\r\n  \"upDown\": 0,\r\n  \"updateItem\":[{\"EditId\":\"36\",\"EditNotes\":\"修改\",\"Year\":\"2026\",\"UpDown\":0,\"PerPackagingPrice\":0.0,\"Freight\":5000.0,\"StorageExpenses\":0.0,\"MonthEndDemand\":26.0,\"PerFreight\":0.48,\"PerTotalLogisticsCost\":0.48}],\r\n  \"file\": 0\r\n}"
                .FromJsonString<SetUpdateItemInput<List<ProductionControlInfoListDto>>>();

            // Act
            await _priceEvaluationAppService.SetUpdateItemLogisticsCost(input);

            // Act
            var outputResult = await _priceEvaluationAppService.GetUpdateItemLogisticsCost(new GetUpdateItemInput
            {
                AuditFlowId = input.AuditFlowId,
                GradientId = input.GradientId,
                SolutionId = input.SolutionId,
                UpDown = input.UpDown,
                Year = input.Year
            });
            // Assert
            outputResult.Count.ShouldBeEquivalentTo(1);
        }

        /// <summary>
        /// 测试质量成本修改项和质量成本修改项数据获取
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetUpdateItemQualityCost_Test()
        {
            var input = "{\r\n  \"auditFlowId\": 9999996,\r\n  \"gradientId\": 9999996,\r\n  \"solutionId\": 9999996,\r\n  \"year\": 2040,\r\n  \"upDown\": 0,\r\n  \"updateItem\":[{\"EditId\":\"QualityCost\",\"EditNotes\":null,\"ProductCategory\":\"环境感知\",\"CostProportion\":2.0,\"QualityCost\":2.6017050864866}],\r\n  \"file\": 0\r\n}"
                .FromJsonString<SetUpdateItemInput<List<QualityCostListDto>>>();

            // Act
            await _priceEvaluationAppService.SetUpdateItemQualityCost(input);

            // Act
            var outputResult = await _priceEvaluationAppService.GetUpdateItemQualityCost(new GetUpdateItemInput
            {
                AuditFlowId = input.AuditFlowId,
                GradientId = input.GradientId,
                SolutionId = input.SolutionId,
                UpDown = input.UpDown,
                Year = input.Year
            });
            // Assert
            outputResult.Count.ShouldBeEquivalentTo(1);
        }

        /// <summary>
        /// 测试其他成本修改项和其他成本修改项数据获取
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SetUpdateItemOtherCost_Test()
        {
            var input = "{\r\n  \"auditFlowId\": 9999995,\r\n  \"gradientId\": 9999995,\r\n  \"solutionId\": 9999995,\r\n  \"year\": 2040,\r\n  \"upDown\": 0,\r\n  \"updateItem\":[{\"EditId\":\"模具费分摊\",\"CostType\":\"NRE费用\",\"ItemName\":\"模具费分摊\",\"Total\":113133.0,\"Count\":218000.0,\"Cost\":0.5189587155963302,\"Note\":\"NRE费用需分摊\",\"IsShare\":true,\"YearCount\":1.0},{\"EditId\":\"治具费分摊\",\"CostType\":\"NRE费用\",\"ItemName\":\"治具费分摊\",\"Total\":20510.0,\"Count\":218000.0,\"Cost\":0.09408256880733945,\"Note\":\"NRE费用需分摊\",\"IsShare\":true,\"YearCount\":1.0},{\"EditId\":\"检具费用分摊\",\"CostType\":\"NRE费用\",\"ItemName\":\"检具费用分摊\",\"Total\":37200.0,\"Count\":218000.0,\"Cost\":0.1706422018348624,\"Note\":\"NRE费用需分摊\",\"IsShare\":true,\"YearCount\":1.0},{\"EditId\":\"实验费分摊\",\"CostType\":\"NRE费用\",\"ItemName\":\"实验费分摊\",\"Total\":43297.8,\"Count\":218000.0,\"Cost\":0.19861376146788992,\"Note\":\"NRE费用需分摊\",\"IsShare\":true,\"YearCount\":1.0}],\r\n  \"file\": 0\r\n}"
    .FromJsonString<SetUpdateItemInput<List<OtherCostItem2List>>>();

            // Act
            await _priceEvaluationAppService.SetUpdateItemOtherCost(input);

            // Act
            var outputResult = await _priceEvaluationAppService.GetUpdateItemOtherCost(new GetUpdateItemInput
            {
                AuditFlowId = input.AuditFlowId,
                GradientId = input.GradientId,
                SolutionId = input.SolutionId,
                UpDown = input.UpDown,
                Year = input.Year
            });
            // Assert
            outputResult.Count.ShouldBeEquivalentTo(4);
        }

        /// <summary>
        /// 测试获取当前新增的项目的版本号
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetQuoteVersion_Test()
        {
            PriceEvaluationStart_Test();
            // Act
            var outputResult = await _priceEvaluationAppService.GetQuoteVersion("JDT-005");

            // Assert
            outputResult.ShouldBeEquivalentTo(2);
        }

        /// <summary>
        /// 测试获取核价表模组的InputCount（投入量）和年份
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPriceEvaluationTableInputCount_Test()
        {
            PriceEvaluationStart_Test();

            // Act
            var outputResult = await _priceEvaluationAppService.GetPriceEvaluationTableInputCount(1);

            // Assert
            outputResult.ShouldBeEquivalentTo(1);
        }


    }
}
