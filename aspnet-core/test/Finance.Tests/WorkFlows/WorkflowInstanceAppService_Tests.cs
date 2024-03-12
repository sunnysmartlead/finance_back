using Abp.Json;
using Finance.EntityFrameworkCore.Seed.Host;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests.WorkFlows
{
    public class WorkflowInstanceAppService_Tests : FinanceTestBase
    {
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        private readonly PriceEvaluationAppService _priceEvaluationAppService;

        public WorkflowInstanceAppService_Tests()
        {
            _priceEvaluationAppService = Resolve<PriceEvaluationAppService>();
            _workflowInstanceAppService = Resolve<WorkflowInstanceAppService>();
        }

        /// <summary>
        /// 测试核价开始函数
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task StartWorkflowInstance_Test()
        {
            var output = await _workflowInstanceAppService.StartWorkflowInstance(new StartWorkflowInstanceInput
            {
                WorkflowId = WorkFlowCreator.MainFlowId,
                FinanceDictionaryDetailId = FinanceConsts.Done,
                Title = "这是一个测试的流程实例"
            });
            output.CompareTo(1);
        }

        /// <summary>
        /// 测试核价开始函数
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SubmitNode_Test()
        {
            //var erer = await _workflowInstanceAppService.StartWorkflowInstance(new StartWorkflowInstanceInput
            //{
            //    WorkflowId = WorkFlowCreator.MainFlowId,
            //    FinanceDictionaryDetailId = FinanceConsts.EvalReason_Bb1,
            //    Title = "这是一个测试的流程实例"
            //});
            var input = " {\r\n    \"files\": [\r\n      {\r\n        \"fileName\": \"NRE核价表-247.xlsx\",\r\n        \"fileUrl\": null,\r\n        \"fileLength\": 0,\r\n        \"fileType\": null,\r\n        \"fileId\": 939\r\n      }\r\n    ],\r\n    \"isSubmit\": true,\r\n    \"quickQuoteAuditFlowId\": null,\r\n    \"title\": \"2024-01-30营销二部关于YY广丰舱内第2版的核价报价申请\",\r\n    \"drafter\": \"吴昕桐\",\r\n    \"drafterNumber\": 20057744,\r\n    \"draftingDepartment\": \"营销二部\",\r\n    \"draftingCompany\": \"未成功获取\",\r\n    \"draftDate\": \"2024-01-30T11:14:50.747\",\r\n    \"number\": \"BJHJ-ZL20240130-001\",\r\n    \"projectName\": \"广丰舱内\",\r\n    \"projectCode\": \"JDT-005\",\r\n    \"quoteVersion\": 2,\r\n    \"customerCode\": null,\r\n    \"customerName\": \"YY\",\r\n    \"customerNature\": \"CustomerNature_Algorithm\",\r\n    \"terminalName\": null,\r\n    \"terminalNature\": null,\r\n    \"sopTime\": 2025,\r\n    \"projectCycle\": 3,\r\n    \"updateFrequency\": \"UpdateFrequency_Year\",\r\n    \"priceEvalType\": \"PriceEvalType_Quantity\",\r\n    \"isHasSample\": true,\r\n    \"sample\": [\r\n      {\r\n        \"name\": \"SampleName_B\",\r\n        \"pcs\": 66\r\n      }\r\n    ],\r\n    \"kValue\": 0.7,\r\n    \"pcs\": [\r\n      {\r\n        \"pcsType\": 0,\r\n        \"carFactory\": \"YY\",\r\n        \"carModel\": \"Y1\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 123\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 122\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 133\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"pcsType\": 0,\r\n        \"carFactory\": \"YY\",\r\n        \"carModel\": \"Y2\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 123\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 122\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 133\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"pcsType\": 1,\r\n        \"carFactory\": \"YY\",\r\n        \"carModel\": \"Y1\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 86.1\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 85.4\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 93.1\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"pcsType\": 1,\r\n        \"carFactory\": \"YY\",\r\n        \"carModel\": \"Y2\",\r\n        \"kv\": 0,\r\n        \"pcsYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 86.1\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 85.4\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 93.1\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"carModelCount\": [\r\n      {\r\n        \"carFactory\": null,\r\n        \"carModel\": \"Y1\",\r\n        \"order\": 1,\r\n        \"partNumber\": \"123\",\r\n        \"code\": \"JDT-005-OM1\",\r\n        \"product\": \"广丰OMS\",\r\n        \"productType\": \"ProductType_CabinMonitoring\",\r\n        \"pixel\": \"2M\",\r\n        \"ourRole\": \"1\",\r\n        \"marketShare\": 100,\r\n        \"moduleCarryingRate\": 100,\r\n        \"singleCarProductsQuantity\": 1,\r\n        \"modelCountYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 86.1\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 85.4\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 93.1\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"carFactory\": null,\r\n        \"carModel\": \"Y2\",\r\n        \"order\": 2,\r\n        \"partNumber\": \"123\",\r\n        \"code\": \"JDT-005-OM1\",\r\n        \"product\": \"广丰OMS\",\r\n        \"productType\": \"ProductType_CabinMonitoring\",\r\n        \"pixel\": \"2M\",\r\n        \"ourRole\": \"1\",\r\n        \"marketShare\": 100,\r\n        \"moduleCarryingRate\": 100,\r\n        \"singleCarProductsQuantity\": 1,\r\n        \"modelCountYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 86.1\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 85.4\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 93.1\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"modelCount\": [\r\n      {\r\n        \"order\": 0,\r\n        \"partNumber\": \"123\",\r\n        \"code\": \"JDT-005-OM1\",\r\n        \"product\": \"广丰OMS\",\r\n        \"productType\": \"ProductType_CabinMonitoring\",\r\n        \"productTypeName\": \"舱内监测\",\r\n        \"pixel\": \"2M\",\r\n        \"modelCountYearList\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"quantity\": 172.2\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"quantity\": 170.8\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"quantity\": 186.2\r\n          }\r\n        ],\r\n        \"sumQuantity\": 529.2\r\n      }\r\n    ],\r\n    \"isHasGradient\": false,\r\n    \"gradient\": [\r\n      {\r\n        \"index\": 0,\r\n        \"gradientValue\": 176.4,\r\n        \"displayGradientValue\": 176.4,\r\n        \"systermGradientValue\": 0\r\n      }\r\n    ],\r\n    \"gradientModel\": [\r\n      {\r\n        \"gradientValue\": 176.4,\r\n        \"index\": 0,\r\n        \"number\": \"123\",\r\n        \"code\": \"JDT-005-OM1\",\r\n        \"name\": \"广丰OMS\",\r\n        \"type\": \"ProductType_CabinMonitoring\",\r\n        \"gradientModelYear\": [\r\n          {\r\n            \"year\": 2025,\r\n            \"upDown\": 0,\r\n            \"count\": 172.2\r\n          },\r\n          {\r\n            \"year\": 2026,\r\n            \"upDown\": 0,\r\n            \"count\": 170.8\r\n          },\r\n          {\r\n            \"year\": 2027,\r\n            \"upDown\": 0,\r\n            \"count\": 186.2\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"requirement\": [\r\n      {\r\n        \"annualDeclineRate\": 0,\r\n        \"annualRebateRequirements\": 0,\r\n        \"oneTimeDiscountRate\": 1,\r\n        \"commissionRate\": 1,\r\n        \"year\": 2025,\r\n        \"upDown\": 0\r\n      },\r\n      {\r\n        \"annualDeclineRate\": 2,\r\n        \"annualRebateRequirements\": 1,\r\n        \"oneTimeDiscountRate\": 1,\r\n        \"commissionRate\": 0,\r\n        \"year\": 2026,\r\n        \"upDown\": 0\r\n      },\r\n      {\r\n        \"annualDeclineRate\": 2,\r\n        \"annualRebateRequirements\": 1,\r\n        \"oneTimeDiscountRate\": 1,\r\n        \"commissionRate\": 1,\r\n        \"year\": 2027,\r\n        \"upDown\": 0\r\n      }\r\n    ],\r\n    \"isHasNre\": false,\r\n    \"shareCount\": [],\r\n    \"allocationOfMouldCost\": true,\r\n    \"allocationOfFixtureCost\": true,\r\n    \"frockCost\": true,\r\n    \"allocationOfEquipmentCost\": true,\r\n    \"fixtureCost\": true,\r\n    \"experimentCost\": true,\r\n    \"testCost\": true,\r\n    \"travelCost\": true,\r\n    \"otherCost\": true,\r\n    \"landingFactory\": \"LandingFactory_SunnySmartLead\",\r\n    \"productInformation\": [\r\n      {\r\n        \"name\": \"广丰OMS\",\r\n        \"product\": \"广丰OMS\",\r\n        \"customerTargetPrice\": 0,\r\n        \"sensor\": null,\r\n        \"sensorTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"sensorPrice\": 0,\r\n        \"sensorCurrency\": 0,\r\n        \"sensorExchangeRate\": 0,\r\n        \"lens\": null,\r\n        \"lensTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"lensPrice\": 0,\r\n        \"lensCurrency\": 0,\r\n        \"lensExchangeRate\": 0,\r\n        \"isp\": null,\r\n        \"ispTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"ispPrice\": 0,\r\n        \"ispCurrency\": 0,\r\n        \"ispExchangeRate\": 0,\r\n        \"serialChip\": null,\r\n        \"serialChipTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"serialChipPrice\": 0,\r\n        \"serialChipCurrency\": 0,\r\n        \"serialChipExchangeRate\": 0,\r\n        \"cable\": null,\r\n        \"cableTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"cablePrice\": 0,\r\n        \"cableCurrency\": 0,\r\n        \"cableExchangeRate\": 0,\r\n        \"other\": null,\r\n        \"otherTypeSelect\": \"TypeSelect_Recommend\",\r\n        \"otherPrice\": 0,\r\n        \"otherCurrency\": 0,\r\n        \"otherExchangeRate\": 0,\r\n        \"manufactureProcess\": null,\r\n        \"installationPosition\": null\r\n      }\r\n    ],\r\n    \"customerTargetPrice\": [\r\n      {\r\n        \"kv\": 176.4,\r\n        \"product\": \"广丰OMS\",\r\n        \"targetPrice\": null,\r\n        \"currency\": null,\r\n        \"exchangeRate\": null\r\n      }\r\n    ],\r\n    \"tradeMode\": \"d3c53e2d6b1b40dc824083ab6e3ce09a\",\r\n    \"salesType\": \"SalesType_ForTheDomesticMarket\",\r\n    \"paymentMethod\": \"11\",\r\n    \"customerSpecialRequest\": \"11\",\r\n    \"shippingType\": \"ShippingType_AirTransport\",\r\n    \"packagingType\": \"PackagingType_DisposableCarton\",\r\n    \"placeOfDelivery\": \"11\",\r\n    \"country\": \"其他国家\",\r\n    \"countryType\": \"2\",\r\n    \"deadline\": \"2024-01-30T16:00:00\",\r\n    \"projectManager\": 54,\r\n    \"sorFile\": [\r\n      939\r\n    ],\r\n    \"reason\": \"11\",\r\n    \"nodeInstanceId\": 0,\r\n    \"opinion\": \"EvalReason_Schj\",\r\n    \"comment\": null\r\n  }".FromJsonString<PriceEvaluationStartInput>();

            // Act
            var output = await _priceEvaluationAppService.PriceEvaluationStart(input);

            //var result = await _workflowInstanceAppService.GetTaskByWorkflowInstanceId(output.AuditFlowId, 39);

            await _workflowInstanceAppService.SubmitNode(new SubmitNodeInput
            {
                NodeInstanceId = 39,
                FinanceDictionaryDetailId = FinanceConsts.YesOrNo_Yes,
            });
        }
    }
}
