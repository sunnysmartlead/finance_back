using Abp.Json;
using Finance.FinanceMaintain;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.Users;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests.PriceEval
{
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
        [Fact]
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

        [Fact]
        public async Task PriceEvaluationStart_Test()
        {
            var input = "{\r\n    \"files\": [\r\n      {\r\n        \"fileName\": \"W04环视技术要求.docx\",\r\n        \"fileUrl\": null,\r\n        \"fileLength\": 0,\r\n        \"fileType\": null,\r\n        \"fileId\": 614\r\n      }\r\n    ],\r\n    \"isSubmit\": false,\r\n    \"quickQuoteAuditFlowId\": null,\r\n    \"title\": \"2024-01-08营销二部关于理想W04 环视第1版的核价报价申请\",\r\n    \"drafter\": \"吴昕桐\",\r\n    \"drafterNumber\": 20057744,\r\n    \"draftingDepartment\": \"营销二部\",\r\n    \"draftingCompany\": \"未成功获取\",\r\n    \"draftDate\": \"2024-01-08T02:25:39.353\",\r\n    \"number\": \"BJHJ-ZL20240108-001\",\r\n    \"projectName\": \"理想W04 环视\",\r\n    \"projectCode\": \"CEL-020\",\r\n    \"quoteVersion\": 1,\r\n    \"customerCode\": null,\r\n    \"customerName\": \"理想\",\r\n    \"customerNature\": \"CustomerNature_NewForce\",\r\n    \"terminalName\": \"理想\",\r\n    \"terminalNature\": \"TerminalNature_NewForce\",\r\n    \"sopTime\": 2025,\r\n    \"projectCycle\": 5,\r\n    \"updateFrequency\": \"UpdateFrequency_Year\",\r\n    \"priceEvalType\": \"PriceEvalType_Quantity\",\r\n    \"isHasSample\": false,\r\n    \"sample\": [],\r\n    \"kValue\": 1,\r\n    \"pcs\": [\r\n      {\r\n        \"pcsType\": 0,\r\n        \"carFactory\": \"理想\",\r\n        \"carModel\": \"/\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 250\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 270\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 400\r\n          },\r\n          {\r\n            \"year\": 2028,\r\n            \"upDown\": 0,\r\n            \"quantity\": 450\r\n          },\r\n          {\r\n            \"year\": 2029,\r\n            \"upDown\": 0,\r\n            \"quantity\": 0\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"pcsType\": 1,\r\n        \"carFactory\": \"理想\",\r\n        \"carModel\": \"/\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 250\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 270\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 400\r\n          },\r\n          {\r\n            \"year\": 2028,\r\n            \"upDown\": 0,\r\n            \"quantity\": 450\r\n          },\r\n          {\r\n            \"year\": 2029,\r\n            \"upDown\": 0,\r\n            \"quantity\": 0\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"carModelCount\": [\r\n      {\r\n        \"carFactory\": null,\r\n        \"carModel\": \"/\",\r\n        \"order\": 1,\r\n        \"partNumber\": \"/\",\r\n        \"code\": \"CEL-020-SV1\",\r\n        \"product\": \"理想W04 环视\",\r\n        \"productType\": \"ProductType_ExternalImaging\",\r\n        \"pixel\": \"3M\",\r\n        \"ourRole\": \"1\",\r\n        \"marketShare\": 100,\r\n        \"moduleCarryingRate\": 100,\r\n        \"singleCarProductsQuantity\": 4,\r\n        \"modelCountYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 1000\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 1080\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 1600\r\n          },\r\n          {\r\n            \"year\": 2028,\r\n            \"upDown\": 0,\r\n            \"quantity\": 1800\r\n          },\r\n          {\r\n            \"year\": 2029,\r\n            \"upDown\": 0,\r\n            \"quantity\": 0\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"modelCount\": [\r\n      {\r\n        \"order\": 0,\r\n        \"partNumber\": \"/\",\r\n        \"code\": \"CEL-020-SV1\",\r\n        \"product\": \"理想W04 环视\",\r\n        \"productType\": \"ProductType_ExternalImaging\",\r\n        \"productTypeName\": \"外摄显像\",\r\n        \"pixel\": \"3M\",\r\n        \"modelCountYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 1000\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 1080\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 1600\r\n          },\r\n          {\r\n            \"year\": 2028,\r\n            \"upDown\": 0,\r\n            \"quantity\": 1800\r\n          },\r\n          {\r\n            \"year\": 2029,\r\n            \"upDown\": 0,\r\n            \"quantity\": 0\r\n          }\r\n        ],\r\n        \"sumQuantity\": 5480\r\n      }\r\n    ],\r\n    \"isHasGradient\": false,\r\n    \"gradient\": [\r\n      {\r\n        \"index\": 0,\r\n        \"gradientValue\": 1096,\r\n        \"displayGradientValue\": 1096,\r\n        \"systermGradientValue\": 0\r\n      }\r\n    ],\r\n    \"gradientModel\": [\r\n      {\r\n        \"gradientValue\": 1096,\r\n        \"index\": 0,\r\n        \"number\": \"/\",\r\n        \"code\": \"CEL-020-SV1\",\r\n        \"name\": \"理想W04 环视\",\r\n        \"type\": \"ProductType_ExternalImaging\",\r\n        \"gradientModelYear\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"count\": 1000\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"count\": 1080\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"count\": 1600\r\n          },\r\n          {\r\n            \"year\": 2028,\r\n            \"upDown\": 0,\r\n            \"count\": 1800\r\n          },\r\n          {\r\n            \"year\": 2029,\r\n            \"upDown\": 0,\r\n            \"count\": 0\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"requirement\": [\r\n      {\r\n        \"annualDeclineRate\": 0,\r\n        \"annualRebateRequirements\": 0,\r\n        \"oneTimeDiscountRate\": 0,\r\n        \"commissionRate\": 0,\r\n        \"year\": 2025,\r\n        \"upDown\": 0\r\n      },\r\n      {\r\n        \"annualDeclineRate\": 0,\r\n        \"annualRebateRequirements\": 0,\r\n        \"oneTimeDiscountRate\": 0,\r\n        \"commissionRate\": 0,\r\n        \"year\": 2026,\r\n        \"upDown\": 0\r\n      },\r\n      {\r\n        \"annualDeclineRate\": 0,\r\n        \"annualRebateRequirements\": 0,\r\n        \"oneTimeDiscountRate\": 0,\r\n        \"commissionRate\": 0,\r\n        \"year\": 2027,\r\n        \"upDown\": 0\r\n      },\r\n      {\r\n        \"annualDeclineRate\": 0,\r\n        \"annualRebateRequirements\": 0,\r\n        \"oneTimeDiscountRate\": 0,\r\n        \"commissionRate\": 0,\r\n        \"year\": 2028,\r\n        \"upDown\": 0\r\n      },\r\n      {\r\n        \"annualDeclineRate\": 0,\r\n        \"annualRebateRequirements\": 0,\r\n        \"oneTimeDiscountRate\": 0,\r\n        \"commissionRate\": 0,\r\n        \"year\": 2029,\r\n        \"upDown\": 0\r\n      }\r\n    ],\r\n    \"isHasNre\": true,\r\n    \"shareCount\": [\r\n      {\r\n        \"name\": \"理想W04 环视\",\r\n        \"count\": 1000,\r\n        \"yearCount\": 1\r\n      }\r\n    ],\r\n    \"allocationOfMouldCost\": true,\r\n    \"allocationOfFixtureCost\": true,\r\n    \"frockCost\": true,\r\n    \"allocationOfEquipmentCost\": true,\r\n    \"fixtureCost\": true,\r\n    \"experimentCost\": true,\r\n    \"testCost\": true,\r\n    \"travelCost\": true,\r\n    \"otherCost\": true,\r\n    \"landingFactory\": \"LandingFactory_SunnySmartLead\",\r\n    \"productInformation\": [\r\n      {\r\n        \"name\": \"理想W04 环视\",\r\n        \"product\": \"理想W04 环视\",\r\n        \"customerTargetPrice\": 0,\r\n        \"sensor\": \"ISX031\",\r\n        \"sensorTypeSelect\": \"TypeSelect_Ask\",\r\n        \"sensorPrice\": 0,\r\n        \"sensorCurrency\": 0,\r\n        \"sensorExchangeRate\": 0,\r\n        \"lens\": \"AT315\",\r\n        \"lensTypeSelect\": \"TypeSelect_Ask\",\r\n        \"lensPrice\": 0,\r\n        \"lensCurrency\": 0,\r\n        \"lensExchangeRate\": 0,\r\n        \"isp\": null,\r\n        \"ispTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"ispPrice\": 0,\r\n        \"ispCurrency\": 0,\r\n        \"ispExchangeRate\": 0,\r\n        \"serialChip\": \"MAX96712\",\r\n        \"serialChipTypeSelect\": \"TypeSelect_Ask\",\r\n        \"serialChipPrice\": 0,\r\n        \"serialChipCurrency\": 0,\r\n        \"serialChipExchangeRate\": 0,\r\n        \"cable\": null,\r\n        \"cableTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"cablePrice\": 0,\r\n        \"cableCurrency\": 0,\r\n        \"cableExchangeRate\": 0,\r\n        \"other\": null,\r\n        \"otherTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"otherPrice\": 0,\r\n        \"otherCurrency\": 0,\r\n        \"otherExchangeRate\": 0,\r\n        \"manufactureProcess\": null,\r\n        \"installationPosition\": null\r\n      }\r\n    ],\r\n    \"customerTargetPrice\": [\r\n      {\r\n        \"kv\": 1096,\r\n        \"product\": \"理想W04 环视\",\r\n        \"targetPrice\": \"160\",\r\n        \"currency\": 3,\r\n        \"exchangeRate\": 1\r\n      }\r\n    ],\r\n    \"tradeMode\": \"d3c53e2d6b1b40dc824083ab6e3ce09a\",\r\n    \"salesType\": \"SalesType_ForTheDomesticMarket\",\r\n    \"paymentMethod\": \"TT60（50%电汇，50%银行承兑）\",\r\n    \"customerSpecialRequest\": \"无\",\r\n    \"shippingType\": \"ShippingType_LandTransportation\",\r\n    \"packagingType\": \"PackagingType_TurnoverBox\",\r\n    \"placeOfDelivery\": \"北京/常州\",\r\n    \"country\": \"其他国家\",\r\n    \"countryType\": \"2\",\r\n    \"deadline\": \"2024-01-08T16:00:00\",\r\n    \"projectManager\": 597,\r\n    \"sorFile\": [\r\n      614\r\n    ],\r\n    \"reason\": null,\r\n    \"nodeInstanceId\": 0,\r\n    \"opinion\": \"EvalReason_Schj\",\r\n    \"comment\": null\r\n  }".FromJsonString<PriceEvaluationStartInput>();
            // Act
            var output = await _priceEvaluationAppService.PriceEvaluationStart(input);

            // Assert
            output.IsSuccess.CompareTo(true);//.Items.Count.ShouldBeGreaterThan(0);

            // Act
            var outputResult = await _priceEvaluationAppService.GetPriceEvaluationStartData(output.AuditFlowId);

            // Assert
            outputResult.Title.CompareTo("2024-01-08营销二部关于理想W04 环视第1版的核价报价申请");
        }
    }
}
