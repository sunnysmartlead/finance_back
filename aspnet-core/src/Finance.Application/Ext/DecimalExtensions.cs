using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// decimal扩展方法
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// 获取可控类型的decimal值，如果为空返回默认值
        /// </summary>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public static decimal GetValueOrDefault(this decimal? nullable)
        {
            return nullable.HasValue ? nullable.Value : 0;
        }
    }
}
