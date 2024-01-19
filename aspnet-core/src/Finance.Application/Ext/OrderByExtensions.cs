using NPOI.SS.Formula.Functions;
using NUglify.JavaScript.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// OrderBy排序扩展
    /// </summary>
    public static class OrderByExtensions
    {
        /// <summary>
        /// 对象集合按照执行的顺序排序
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="func">委托</param>
        /// <param name="strings">排序的字符串集合</param>
        /// <returns></returns>
        public static List<T> OrderByFunc<T>(this List<T> @objects,Func<T, string> func,List<string> strings) where T : class
        {
            return @objects.OrderBy(p =>
            {
                int index = strings.IndexOf(func(p));
                return index >= 0 ? index + 1 : int.MaxValue;
            }).ToList();
        }
    }
}
