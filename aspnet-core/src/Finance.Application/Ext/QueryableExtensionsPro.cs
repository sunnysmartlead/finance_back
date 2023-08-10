using Abp.Extensions;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace Finance.Ext
{
    /// <summary>
    /// 查询扩展
    /// </summary>
    public static class QueryableExtensionsPro
    {
        /// <summary>
        /// 动态Where的WhereIf扩展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIfDynamic<T>(this IQueryable<T> source, bool condition, string predicate, params object[] args)
        {
            return condition ? source.Where(predicate, args) : source;
        }

        /// <summary>
        /// 分隔
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitPro(this string str, char separator)
        {
            return str.Split(separator);
        }

        /// <summary>
        /// 验证字符串是否为数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumber(this string input)
        {
            if (input.IsNullOrWhiteSpace())
            {
                return false;
            }
            Regex reg = new Regex("^[1-9]\\d*$");
            return reg.IsMatch(input);
        }
    }
}
