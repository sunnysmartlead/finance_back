using Finance.Ext;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests.Ext
{
    public class EvalTavleExtensions_Test : FinanceTestBase
    {
        /// <summary>
        /// 获取Excel核价表Sheet
        /// </summary>
        private readonly ISheet _sheet;

        public EvalTavleExtensions_Test()
        {
            using var fileStream = new FileStream("wwwroot/Excel/TestEval.xlsx", FileMode.Open);
            var workbook = new XSSFWorkbook(fileStream);
            var sheetName = $"2025年";
            _sheet = workbook.GetSheet(sheetName) ?? throw new FriendlyException($"未读取到Sheet名为“{sheetName}”的核价表");
        }

        /// <summary>
        /// 测试获取Excel核价表的BOM成本
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetMaterials_Test()
        {
            // BOM
            var materials = _sheet.GetMaterials(2025, YearType.Year).ToList();
            materials.Count.CompareTo(48);
            materials[0].SuperType.CompareTo("电子料");
        }

        /// <summary>
        /// 测试获取Excel核价表的制造成本
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetManufacturingCosts_Test()
        {
            //制造成本
            var manufacturingCosts = _sheet.GetManufacturingCosts(2025, YearType.Year).ToList();
            manufacturingCosts.Count.CompareTo(5);
            manufacturingCosts[2].ManufacturingCostDirect.DirectLabor.CompareTo(4.73M);
        }

        /// <summary>
        /// 测试获取Excel核价表的损耗成本
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLossCosts_Test()
        {
            //损耗成本
            var lossCosts = _sheet.GetLossCosts(2025, YearType.Year).ToList();
            lossCosts.Count.CompareTo(5);
            lossCosts[1].WastageCost.CompareTo(0.89M);
        }

        /// <summary>
        /// 测试获取Excel核价表的其他成本项目2
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetOtherCostItem2s_Test()
        {
            //其他成本项目2
            var otherCostItem2s = _sheet.GetOtherCostItem2s(2025, YearType.Year, 127).ToList();

            otherCostItem2s.Count.CompareTo(3);
            otherCostItem2s[1].Total.Value.CompareTo(0M);
        }

        /// <summary>
        /// 测试获取Excel核价表的其他成本
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetOtherCostItems_Test()
        {
            //其他成本
            var otherCostItems = _sheet.GetOtherCostItems(2025, YearType.Year);
            otherCostItems.LogisticsFee.CompareTo(0.8M);
            otherCostItems.QualityCost.CompareTo(0.83M);
        }

        /// <summary>
        /// 测试获取Excel核价表的质量成本
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetQualityCostListDto_Test()
        {
            //质量成本
            var qualityCostListDto = _sheet.GetQualityCostListDto(2025, YearType.Year);
            qualityCostListDto.ProductCategory.CompareTo("外摄显像");
            qualityCostListDto.QualityCost.CompareTo(0.83M);
        }

        /// <summary>
        /// 测试获取Excel核价表的物流成本汇总
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLogisticsCosts_Test()
        {
            //物流成本汇总
            var logisticsCosts = _sheet.GetLogisticsCosts(2025, YearType.Year).ToList();
            logisticsCosts.Count.CompareTo(1);
            logisticsCosts[0].Freight.CompareTo(0M);
            logisticsCosts[0].PerFreight.CompareTo(0.8M);
        }
    }
}
