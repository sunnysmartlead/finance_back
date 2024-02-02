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
using System.ComponentModel.DataAnnotations;
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
                    SuperType = row.Try(1, p => p.StringCellValue.ToString()),
                    CategoryName = row.Try(2, p => p.StringCellValue.ToString()),
                    TypeName = row.Try(3, p => p.StringCellValue.ToString()),
                    Sap = row.Try(4, p => p.StringCellValue.ToString()),
                    MaterialName = row.Try(5, p => p.StringCellValue.ToString()),
                    IsCustomerSupply = row.Try(6, p => p.ToString().GetIsCustomerSupply()),
                    AssemblyCount = row.Try(7, p => p.NumericCellValue.ToString().ToDouble()),
                    MaterialPrice = row.Try(8, p => p.NumericCellValue.ToString().ToDecimal()),
                    CurrencyText = row.Try(9, p => p.StringCellValue.ToString()),
                    ExchangeRate = row.Try(10, p => p.NumericCellValue.ToString().ToDecimal()),
                    MaterialPriceCyn = row.Try(11, p => p.NumericCellValue.ToString().ToDecimal()),
                    TotalMoneyCyn = row.Try(12, p => p.NumericCellValue.ToString().ToDecimal()),
                    TotalMoneyCynNoCustomerSupply = row.Try(13, p => p.NumericCellValue.ToString().ToDecimal()),
                    Loss = row.Try(14, p => p.NumericCellValue.ToString().ToDecimal()),
                    MaterialCost = row.Try(15, p => p.NumericCellValue.ToString().ToDecimal()),
                    InputCount = row.Try(16, p => p.NumericCellValue.ToString().ToDecimal()),
                    PurchaseCount = row.Try(17, p => p.NumericCellValue.ToString().ToDecimal()),
                    MoqShareCount = row.Try(18, p => p.NumericCellValue.ToString().ToDecimal()),
                    Moq = row.Try(19, p => p.NumericCellValue.ToString().ToDecimal()),
                    AvailableInventory = row.Try(20, p => p.NumericCellValue.ToString().ToInt()),
                    Remarks = row.Try(21, p => p.StringCellValue.ToString()),
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
                    CostItem = row.Try(0, p => p.ToString()),
                    ManufacturingCostDirect = new ManufacturingCostDirect
                    {
                        Id = year,
                        UpDown = upDonw,
                        DirectLabor = row.Try(7, p => p.NumericCellValue.ToString().ToDecimal()),
                        EquipmentDepreciation = row.Try(8, p => p.NumericCellValue.ToString().ToDecimal()),
                        LineChangeCost = row.Try(9, p => p.NumericCellValue.ToString().ToDecimal()),
                        ManufacturingExpenses = row.Try(10, p => p.NumericCellValue.ToString().ToDecimal()),
                        Subtotal = row.Try(11, p => p.NumericCellValue.ToString().ToDecimal()),
                    },
                    ManufacturingCostIndirect = new ManufacturingCostIndirect
                    {
                        Id = year,
                        UpDown = upDonw,
                        DirectLabor = row.Try(12, p => p.NumericCellValue.ToString().ToDecimal()),
                        EquipmentDepreciation = row.Try(13, p => p.NumericCellValue.ToString().ToDecimal()),
                        ManufacturingExpenses = row.Try(14, p => p.NumericCellValue.ToString().ToDecimal()),
                        Subtotal = row.Try(15, p => p.NumericCellValue.ToString().ToDecimal()),
                    },
                    Subtotal = row.Try(16, p => p.NumericCellValue.ToString().ToDecimal()),
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
                    Name = nameRow.Try(i, p => p.StringCellValue.ToString()),
                    WastageCost = lossRow.Try(i, p => p.NumericCellValue.ToString().ToDecimal()),
                    MoqShareCount = moqRow.Try(i, p => p.NumericCellValue.ToString().ToDecimal()),
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
                    ItemName = row.Try(0, p => p.StringCellValue.ToString()),
                    Total = row.Try(7, p => p.NumericCellValue.To<decimal>()),
                    MoldCosts = row.Try(8, p => p.NumericCellValue.To<decimal>()),
                    FixtureCost = row.Try(9, p => p.NumericCellValue.To<decimal>()),
                    ToolCost = row.Try(10, p => p.NumericCellValue.To<decimal>()),
                    InspectionCost = row.Try(11, p => p.NumericCellValue.To<decimal>()),
                    ExperimentCost = row.Try(12, p => p.NumericCellValue.To<decimal>()),
                    SpecializedEquipmentCost = row.Try(13, p => p.NumericCellValue.To<decimal>()),
                    TestSoftwareCost = row.Try(14, p => p.NumericCellValue.To<decimal>()),
                    OtherExpensesCost = row.Try(15, p => p.NumericCellValue.To<decimal>()),
                    TravelCost = row.Try(16, p => p.NumericCellValue.To<decimal>()),
                    SluggishCost = row.Try(17, p => p.NumericCellValue.To<decimal>()),
                    RetentionCost = row.Try(18, p => p.NumericCellValue.To<decimal>()),
                    LineCost = row.Try(19, p => p.NumericCellValue.To<decimal>()),
                    OtherCost = row.Try(20, p => p.NumericCellValue.To<decimal>()),
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
                Fixture = row.Try(7, p => p.ToString().IsNullOrWhiteSpace()) ? null : row.Try(7, p => p.NumericCellValue.ToString().ToDecimal()),
                LogisticsFee = row.Try(8, p => p.NumericCellValue.ToString().ToDecimal()),
                ProductCategory = row.Try(9, p => p.StringCellValue.ToString()),
                CostProportion = row.Try(10, p => p.ToString().Replace("%", string.Empty).ToDecimal()),
                QualityCost = row.Try(11, p => p.NumericCellValue.ToString().ToDecimal()),
                AccountingPeriod = row.Try(12, p => p.ToString().ToInt()),
                CapitalCostRate = row.Try(14, p => p.ToString().IsNullOrWhiteSpace()) ? null : row.Try(7, p => p.NumericCellValue.ToString().ToDecimal()),
                TaxCost = row.Try(16, p => p.ToString().IsNullOrWhiteSpace()) ? null : row.Try(7, p => p.NumericCellValue.ToString().ToDecimal()),
                Total = row.Try(17, p => p.NumericCellValue.ToString().ToDecimal()),
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
                ProductCategory = row.Try(9, p => p.StringCellValue.ToString()),
                CostProportion = row.Try(10, p => p.ToString().Replace("%", string.Empty).ToDecimal() * 100),
                QualityCost = row.Try(11, p => p.NumericCellValue.ToString().ToDecimal()),
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
                PerTotalLogisticsCost = row.Try(8, p => p.NumericCellValue.ToString().ToDecimal()),
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
                if (!Regex.IsMatch(row.Try(0, p => p.ToString()), @"^\d+$"))
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
                if (row.Try(0, p => p.ToString().IsNullOrWhiteSpace()))
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

        #region 尝试读取Excel

        /// <summary>
        /// 尝试读取Excel的值
        /// 如果读取失败，提示失败的位置
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public static T Try<T>(this IRow row, int columnIndex, Func<ICell, T> func)
        {
            try
            {
                return func(row.Cells[columnIndex]);
            }
            catch (Exception e)
            {
                throw new FriendlyException($"{row.Sheet.SheetName}核价表{row.RowNum + 1}行{columnIndex.ToName()}列读取错误。\r\n错误原因：{e.Message}。如果在此位置未发现错误，请检查核价表序号是否正确。", e.StackTrace);
            }
        }

        /// <summary>
        /// 获取Excel列名根据列索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ToName([Range(0, 16383)] this int index)
        {
            index++;
            var builder = new StringBuilder();
            while (index > 0)
            {
                index--;
                builder.Insert(0, Convert.ToChar(index % 26 + 65));
                index /= 26;
            }
            return builder.ToString();
        }

        #endregion
    }
}