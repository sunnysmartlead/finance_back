using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    public static class VarianceExtensions
    {
        private const string Add = "新增";
        private const string Del = "删除";
        private const string None = "无";

        /// <summary>
        /// 以版本1为基本，和版本2比对。
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        public static string Variance(this decimal? version1, decimal? version2)
        {
            if (!version1.HasValue && !version2.HasValue) { return None; }
            if (!version1.HasValue && version2.HasValue) { return Add; }
            if (version1.HasValue && !version2.HasValue) { return Del; }

            var variance = version1.Value - version2.Value;
            return variance == 0 ? None : variance.ToString();
        }

        /// <summary>
        /// 以版本1为基本，和版本2比对。
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        public static string Variance(this double? version1, double? version2)
        {
            if (!version1.HasValue && !version2.HasValue) { return None; }
            if (!version1.HasValue && version2.HasValue) { return Add; }
            if (version1.HasValue && !version2.HasValue) { return Del; }
            var variance = version1.Value - version2.Value;
            return variance == 0 ? None : variance.ToString();
        }
    }
}
