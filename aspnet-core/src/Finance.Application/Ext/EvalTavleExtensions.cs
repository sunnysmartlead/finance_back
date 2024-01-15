using Abp.Application.Features;
using Abp.Extensions;
using Castle.MicroKernel.SubSystems.Conversion;
using Finance.FinanceMaintain;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using NPOI.HSSF.Record.AutoFilter;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using Spire.Pdf.Exporting.XPS.Schema;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Finance.Ext
{
    /// <summary>
    /// 核价表Excel导入扩展方法
    /// </summary>
    public static class EvalTavleExtensions
    {
        #region 公用参数

        //在核价表Excel中，【BOM成本】开始的行
        private const int StartRow = 8;

        //在核价表Excel中，【制造成本】在【BOM成本】下的行数
        private const int ManufacturingCostRow = 8;

        //在核价表Excel中，【损耗成本】在【制造成本】下的行数
        private const int LossCostRow = 3;

        #endregion

        #region 获取核价表的信息

        /// <summary>
        /// 获取BOM成本
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Material> GetMaterials(this ISheet sheet, int year, YearType upDonw)
        {
            //获取核价表结束的行
            var bomEndRow = sheet.GetBomEndRow();

            // 遍历行
            for (int rowIndex = StartRow; rowIndex <= bomEndRow; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);

                yield return new Material
                {
                    Id = $"import{rowIndex}",
                    Year = year,
                    UpDown = upDonw,
                    SuperType = row.Cells[1].ToString(),
                    CategoryName = row.Cells[2].ToString(),
                    TypeName = row.Cells[3].ToString(),
                    Sap = row.Cells[4].ToString(),
                    MaterialName = row.Cells[5].ToString(),
                    IsCustomerSupply = row.Cells[6].ToString().GetIsCustomerSupply(),
                    AssemblyCount = row.Cells[7].NumericCellValue.ToString().ToDouble(),
                    MaterialPrice = row.Cells[8].NumericCellValue.ToString().ToDecimal(),
                    CurrencyText = row.Cells[9].ToString(),
                    ExchangeRate = row.Cells[10].NumericCellValue.ToString().ToDecimal(),
                    MaterialPriceCyn = row.Cells[11].NumericCellValue.ToString().ToDecimal(),
                    TotalMoneyCyn = row.Cells[12].NumericCellValue.ToString().ToDecimal(),
                    TotalMoneyCynNoCustomerSupply = row.Cells[13].NumericCellValue.ToString().ToDecimal(),
                    Loss = row.Cells[14].NumericCellValue.ToString().ToDecimal(),
                    MaterialCost = row.Cells[15].NumericCellValue.ToString().ToDecimal(),
                    InputCount = row.Cells[16].NumericCellValue.ToString().ToDecimal(),
                    PurchaseCount = row.Cells[17].NumericCellValue.ToString().ToDecimal(),
                    MoqShareCount = row.Cells[18].NumericCellValue.ToString().ToDecimal(),
                    Moq = row.Cells[19].NumericCellValue.ToString().ToDecimal(),
                    AvailableInventory = row.Cells[20].NumericCellValue.ToString().ToInt(),
                    Remarks = row.Cells[21].ToString(),
                };
            }
        }

        /// <summary>
        /// 获取制造成本
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ManufacturingCost> GetManufacturingCosts(this ISheet sheet, int year, YearType upDonw)
        {
            //获取制造成本开始和结束的行
            var startRow = GetManufacturingCostStartEndRow(sheet);

            for (int rowIndex = startRow.startRow; rowIndex <= startRow.endRow; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);

                var dto = new ManufacturingCost
                {
                    Id = year,
                    Year = year,
                    UpDown = upDonw,
                    CostItem = row.Cells[0].ToString(),
                    ManufacturingCostDirect = new ManufacturingCostDirect
                    {
                        Id = year,
                        UpDown = upDonw,
                        DirectLabor = row.Cells[7].NumericCellValue.ToString().ToDecimal(),
                        EquipmentDepreciation = row.Cells[8].NumericCellValue.ToString().ToDecimal(),
                        LineChangeCost = row.Cells[9].NumericCellValue.ToString().ToDecimal(),
                        ManufacturingExpenses = row.Cells[10].NumericCellValue.ToString().ToDecimal(),
                        Subtotal = row.Cells[11].NumericCellValue.ToString().ToDecimal(),
                    },
                    ManufacturingCostIndirect = new ManufacturingCostIndirect
                    {
                        Id = year,
                        UpDown = upDonw,
                        DirectLabor = row.Cells[12].NumericCellValue.ToString().ToDecimal(),
                        EquipmentDepreciation = row.Cells[13].NumericCellValue.ToString().ToDecimal(),
                        ManufacturingExpenses = row.Cells[14].NumericCellValue.ToString().ToDecimal(),
                        Subtotal = row.Cells[15].NumericCellValue.ToString().ToDecimal(),
                    },
                    Subtotal = row.Cells[16].NumericCellValue.ToString().ToDecimal(),
                };


                dto.CostType = dto.CostItem switch
                {
                    PriceEvalConsts.GroupTest => CostType.GroupTest,
                    PriceEvalConsts.SMT => CostType.SMT,
                    PriceEvalConsts.COB => CostType.COB,
                    PriceEvalConsts.Total => CostType.Total,
                    PriceEvalConsts.Other => CostType.Other,
                    _ => throw new FriendlyException($"核价表的制造成本类型不正确。错误的成本类型为：{dto.CostItem}"),
                };
                dto.EditId = dto.CostType.ToString();

                yield return dto;
            }
        }

        /// <summary>
        /// 获取损耗成本
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<LossCost> GetLossCosts(this ISheet sheet, int year, YearType upDonw)
        {
            //获取损耗成本开始和结束的行
            var startRow = GetLossCostStartEndRow(sheet);

            var nameRow = sheet.GetRow(startRow.startRow);
            var lossRow = sheet.GetRow(startRow.startRow + 1);
            var moqRow = sheet.GetRow(startRow.startRow + 2);

            for (int i = 8; i <= 12; i++)
            {
                yield return new LossCost
                {
                    Id = year,
                    Year = year,
                    UpDown = upDonw,
                    EditId = i.ToString(),
                    Name = nameRow.Cells[i].ToString(),
                    WastageCost = lossRow.Cells[i].NumericCellValue.ToString().ToDecimal(),
                    MoqShareCount = moqRow.Cells[i].NumericCellValue.ToString().ToDecimal(),
                };
            }
        }

        /// <summary>
        /// 获取其他成本项目2
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<OtherCostItem2> GetOtherCostItem2s(this ISheet sheet, int year, YearType upDonw, decimal quantity)
        {
            //获取其他成本项目2开始和结束的行
            var startRow = GetOtherCostItem2StartEndRow(sheet);

            for (int i = startRow.startRow; i <= startRow.endRow; i++)
            {
                var row = sheet.GetRow(i);

                yield return new OtherCostItem2
                {
                    Year = year,
                    UpDown = upDonw,
                    Quantity = quantity,
                    ItemName = row.Cells[0].ToString(),
                    Total = row.Cells[7].NumericCellValue.To<decimal>(),
                    MoldCosts = row.Cells[8].NumericCellValue.To<decimal>(),
                    FixtureCost = row.Cells[9].NumericCellValue.To<decimal>(),
                    ToolCost = row.Cells[10].NumericCellValue.To<decimal>(),
                    InspectionCost = row.Cells[11].NumericCellValue.To<decimal>(),
                    ExperimentCost = row.Cells[12].NumericCellValue.To<decimal>(),
                    SpecializedEquipmentCost = row.Cells[13].NumericCellValue.To<decimal>(),
                    TestSoftwareCost = row.Cells[14].NumericCellValue.To<decimal>(),
                    OtherExpensesCost = row.Cells[15].NumericCellValue.To<decimal>(),
                    TravelCost = row.Cells[16].NumericCellValue.To<decimal>(),
                    SluggishCost = row.Cells[17].NumericCellValue.To<decimal>(),
                    RetentionCost = row.Cells[18].NumericCellValue.To<decimal>(),
                    LineCost = row.Cells[19].NumericCellValue.To<decimal>(),
                    OtherCost = row.Cells[20].NumericCellValue.To<decimal>(),
                };
            }
        }

        /// <summary>
        /// 获取其他成本项目
        /// </summary>
        /// <returns></returns>
        public static OtherCostItem GetOtherCostItems(this ISheet sheet, int year, YearType upDonw)
        {
            //获取其他成本项目2开始和结束的行
            var startRow = GetOtherCostItem2StartEndRow(sheet);

            var row = sheet.GetRow(startRow.endRow + 4);

            return new OtherCostItem
            {
                Id = year,
                Year = year,
                UpDown = upDonw,
                Fixture = row.Cells[7].ToString().IsNullOrWhiteSpace() ? null : row.Cells[7].NumericCellValue.ToString().ToDecimal(),
                LogisticsFee = row.Cells[8].NumericCellValue.ToString().ToDecimal(),
                ProductCategory = row.Cells[9].ToString(),
                CostProportion = row.Cells[10].ToString().Replace("%", string.Empty).ToDecimal(),
                QualityCost = row.Cells[11].NumericCellValue.ToString().ToDecimal(),
                AccountingPeriod = row.Cells[12].ToString().ToInt(),
                CapitalCostRate = row.Cells[14].ToString().IsNullOrWhiteSpace() ? null : row.Cells[7].NumericCellValue.ToString().ToDecimal(),
                TaxCost = row.Cells[16].ToString().IsNullOrWhiteSpace() ? null : row.Cells[7].NumericCellValue.ToString().ToDecimal(),
                Total = row.Cells[17].NumericCellValue.ToString().ToDecimal(),
            };
        }

        /// <summary>
        /// 获取质量成本
        /// </summary>
        /// <returns></returns>
        public static QualityCostListDto GetQualityCostListDto(this ISheet sheet, int year, YearType upDonw)
        {
            //获取其他成本项目2开始和结束的行
            var startRow = GetOtherCostItem2StartEndRow(sheet);

            var row = sheet.GetRow(startRow.endRow + 4);

            return new QualityCostListDto
            {
                Year = year,
                UpDown = upDonw,
                EditId = UpdateItemType.QualityCost.ToString(),
                ProductCategory = row.Cells[9].ToString(),
                CostProportion = row.Cells[10].ToString().Replace("%", string.Empty).ToDecimal() * 100,
                QualityCost = row.Cells[11].NumericCellValue.ToString().ToDecimal(),
            };
        }

        /// <summary>
        /// 获取物流成本汇总表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ProductionControlInfoListDto> GetLogisticsCosts(this ISheet sheet, int year, YearType upDonw)
        {
            //获取其他成本项目2开始和结束的行
            var startRow = GetOtherCostItem2StartEndRow(sheet);

            var row = sheet.GetRow(startRow.endRow + 4);

            yield return new ProductionControlInfoListDto
            {
                Year = year.ToString(),
                UpDown = upDonw,
                EditId = "logisticsCost",
                PerTotalLogisticsCost = row.Cells[8].NumericCellValue.ToString().ToDecimal(),
            };
        }
        #endregion

        #region 数据转换及行列信息

        /// <summary>
        /// 获取核价表结束的行
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        private static int GetBomEndRow(this ISheet sheet)
        {
            // 遍历行
            for (int rowIndex = StartRow; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (!Regex.IsMatch(row.Cells[0].ToString(), @"^\d+$"))
                {
                    return rowIndex - 1;
                }
            }
            throw new FriendlyException($"上传的核价表格式不合规！");
        }

        /// <summary>
        /// 获取制造成本开始的行
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        private static int GetManufacturingCostStartRow(this ISheet sheet)
        {
            //获取核价表结束的行
            var bomEndRow = sheet.GetBomEndRow();

            //获取制造成本开始的行
            return bomEndRow + ManufacturingCostRow;
        }

        /// <summary>
        /// 获取制造成本开始和结束的行
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        private static (int startRow, int endRow) GetManufacturingCostStartEndRow(this ISheet sheet)
        {
            //获取制造成本开始的行
            var start = GetManufacturingCostStartRow(sheet);

            // 遍历行
            for (int rowIndex = start; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row.Cells[0].ToString().IsNullOrWhiteSpace())
                {
                    return (start, rowIndex - 1);
                }
            }
            throw new FriendlyException($"上传的核价表格式不合规，读取制造成本时失败。");
        }

        /// <summary>
        /// 获取损耗成本开始和结束的行
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        private static (int startRow, int endRow) GetLossCostStartEndRow(this ISheet sheet)
        {
            //获取制造成本开始和结束的行
            var startRow = GetManufacturingCostStartEndRow(sheet);
            return (startRow.endRow + 2, startRow.endRow + 4);
        }

        /// <summary>
        /// 获取其他成本项目2开始和结束的行
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static (int startRow, int endRow) GetOtherCostItem2StartEndRow(this ISheet sheet)
        {
            //获取损耗成本开始和结束的行
            var startRow = GetLossCostStartEndRow(sheet);
            return (startRow.endRow + 3, startRow.endRow + 5);
        }

        /// <summary>
        /// 获取是否客供
        /// </summary>
        /// <returns></returns>
        private static bool GetIsCustomerSupply(this string str)
        {
            switch (str)
            {
                case "是": return true;
                case "否": return false;
                default: throw new FriendlyException($"上传的核价表【是否客供】列的【{str}】内容不合法。");
            }
        }

        /// <summary>
        /// 支持科学计数法的数值转换
        /// </summary>
        /// <returns></returns>
        private static decimal ToDecimal(this string str)
        {
            if (str.Contains('E') || str.Contains('e'))
            {
                return decimal.Parse(str, System.Globalization.NumberStyles.Float);
            }
            else
            {
                return str.To<decimal>();
            }
        }

        /// <summary>
        /// 支持科学计数法的数值转换
        /// </summary>
        /// <returns></returns>
        private static double ToDouble(this string str)
        {
            if (str.Contains('E') || str.Contains('e'))
            {
                return double.Parse(str, System.Globalization.NumberStyles.Float);
            }
            else
            {
                return str.To<double>();
            }
        }

        /// <summary>
        /// 支持科学计数法的数值转换
        /// </summary>
        /// <returns></returns>
        private static int ToInt(this string str)
        {
            if (str.Contains('E') || str.Contains('e'))
            {
                return int.Parse(str, System.Globalization.NumberStyles.Float);
            }
            else
            {
                return str.To<int>();
            }
        }

        #endregion
    }
}
