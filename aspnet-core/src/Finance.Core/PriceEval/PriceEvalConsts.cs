using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 核价部分使用到的常量
    /// </summary>
    public class PriceEvalConsts
    {
        /// <summary>
        /// 核价表全生命周期使用的数字
        /// </summary>
        public const int AllYear = 0;

        /// <summary>
        /// 组测制造成本（元）
        /// </summary>
        public const string GroupTest = "组测制造成本（元）";

        /// <summary>
        /// SMT制造成本（元）
        /// </summary>
        public const string SMT = "SMT制造成本（元）";

        /// <summary>
        /// COB制造成本（元）
        /// </summary>
        public const string COB = "COB制造成本（元）";

        /// <summary>
        /// 制造成本合计（元）
        /// </summary>
        public const string Total = "制造成本合计（元）";

        /// <summary>
        /// 其他制造成本合计（元）
        /// </summary>
        public const string Other = "其他制造成本（元）";


        /// <summary>
        /// 组测
        /// </summary>
        public const string Zc = "组测（元）";
    }
}
