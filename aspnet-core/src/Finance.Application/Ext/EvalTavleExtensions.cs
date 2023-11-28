using Abp.Extensions;
using Castle.MicroKernel.SubSystems.Conversion;
using Finance.FinanceMaintain;
using Finance.PriceEval.Dto;
using NPOI.HSSF.Record.AutoFilter;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// 核价表Excel导入扩展方法
    /// </summary>
    public static class EvalTavleExtensions
    {
        //在核价表Excel中，BOM成本开始的行
        private const int StartRow = 8;

        /// <summary>
        /// 获取BOM成本
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static IEnumerable<Material> GetMaterials(this ISheet sheet)
        {
            //获取核价表结束的行
            var bomEndRow = sheet.GetBomEndRow();

            // 遍历行
            for (int rowIndex = StartRow; rowIndex <= bomEndRow; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);

                var dto = new Material();
                dto.SuperType = row.Cells[1].ToString();
                dto.CategoryName = row.Cells[2].ToString();
                dto.TypeName = row.Cells[3].ToString();
                dto.Sap = row.Cells[4].ToString();
                dto.MaterialName = row.Cells[5].ToString();
                dto.IsCustomerSupply = row.Cells[6].ToString().GetIsCustomerSupply();
                dto.AssemblyCount = row.Cells[7].ToString().ToDouble();
                dto.MaterialPrice = row.Cells[8].ToString().ToDecimal();
                dto.CurrencyText = row.Cells[9].ToString();
                dto.ExchangeRate = row.Cells[10].ToString().ToDecimal();
                dto.MaterialPriceCyn = row.Cells[11].ToString().ToDecimal();
                dto.TotalMoneyCyn = row.Cells[12].ToString().ToDecimal();
                dto.TotalMoneyCynNoCustomerSupply = row.Cells[13].ToString().ToDecimal();
                dto.Loss = row.Cells[14].ToString().ToDecimal();
                dto.MaterialCost = row.Cells[15].ToString().ToDecimal();
                dto.InputCount = row.Cells[16].ToString().ToDecimal();
                dto.PurchaseCount = row.Cells[17].ToString().ToDecimal();
                dto.MoqShareCount = row.Cells[18].ToString().ToDecimal();
                dto.Moq = row.Cells[19].ToString().ToDecimal();
                dto.AvailableInventory = row.Cells[20].ToString().ToInt();
                dto.Remarks = row.Cells[21].ToString();
                yield return dto;
            }
        }




        /// <summary>
        /// 获取核价表结束的行
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public static int GetBomEndRow(this ISheet sheet)
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
        /// 获取是否客供
        /// </summary>
        /// <returns></returns>
        public static bool GetIsCustomerSupply(this string str)
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
        public static decimal ToDecimal(this string str)
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
        public static double ToDouble(this string str)
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
        public static int ToInt(this string str)
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
    }
}
