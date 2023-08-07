using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static Finance.PropertyDepartment.UnitPriceLibrary.Dto.ExtensionMethods;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scoure"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static List<decimal> SplitGrossMargin(this string scoure, Func<string, List<decimal>> func)
        {
             return func(scoure);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scoure"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static List<YearOrValueMode> JsonExchangeRateValue(this string scoure, Func<string, List<YearOrValueMode>> func)
        {
            return func(scoure);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scoure"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static string StringGrossMargin(this List<decimal> scoure, Func<List<decimal>,string> func)
        {
            return func(scoure);
        }
        /// <summary>
        /// 将对象转换为 String 类型列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<string> ToListString(this object obj)
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<string>();
            foreach (var property in properties)
            {
                var value = property.GetValue(obj)?.ToString() ?? "";
                list.Add(value);
            }
            return list;
        }
        /// <summary>
        /// 将对象转换为 Int 类型列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<int> ToListInt(this object obj)
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<int>();
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                if (value != null && int.TryParse(value.ToString(), out int intValue))
                {
                    list.Add(intValue);
                }
            }
            return list;
        }
        /// <summary>
        ///  将对象转换为 decimal 类型列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<decimal> ToListDecimal(this object obj)
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<decimal>();
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                if (value != null && decimal.TryParse(value.ToString(), out decimal decimalValue))
                {
                    list.Add(decimalValue);
                }
            }
            return list;
        }
        /// <summary>
        /// 用于将数字转换为Excel列名称：
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }        

    }
   
}
