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

        /// <summary>
        /// 测试Excel列名获取函数
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ToName_Test()
        {
            0.ToName().CompareTo("A");
            1.ToName().CompareTo("B");
            2.ToName().CompareTo("C");
            3.ToName().CompareTo("D");
            4.ToName().CompareTo("E");
            5.ToName().CompareTo("F");
            6.ToName().CompareTo("G");
            7.ToName().CompareTo("H");
            8.ToName().CompareTo("I");
            9.ToName().CompareTo("J");
            10.ToName().CompareTo("K");
            11.ToName().CompareTo("L");
            12.ToName().CompareTo("M");
            13.ToName().CompareTo("N");
            14.ToName().CompareTo("O");
            15.ToName().CompareTo("P");
            16.ToName().CompareTo("Q");
            17.ToName().CompareTo("R");
            18.ToName().CompareTo("S");
            19.ToName().CompareTo("T");
            20.ToName().CompareTo("U");
            21.ToName().CompareTo("V");
            22.ToName().CompareTo("W");
            23.ToName().CompareTo("X");
            24.ToName().CompareTo("Y");
            25.ToName().CompareTo("Z");
            26.ToName().CompareTo("AA");
            27.ToName().CompareTo("AB");
            28.ToName().CompareTo("AC");
            29.ToName().CompareTo("AD");
            30.ToName().CompareTo("AE");
            31.ToName().CompareTo("AF");
        }
    }
}
