using Finance.PriceEval.Dto;
using Finance.PriceEval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// 类的扩展方法
    /// </summary>
    public static class ClassExtensions
    {
        /// <summary>
        /// 成本项目排序
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int CostTypeSort(this ManufacturingCost input)
        {
            switch (input.CostType)
            {
                case CostType.COB: return 1;
                case CostType.SMT: return 2;
                case CostType.GroupTest: return 3;
                case CostType.Other: return 4;
                case CostType.Total: return 5;
                default: return 100;
            }
        }
    }
}
