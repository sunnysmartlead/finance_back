using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// Nre 工装费用 实体类
    /// </summary>
    public class ToolingCostModel
    {
        /// <summary>
        /// 费用类型 判断是工装费用还是测试线费用  1是工装费用2是测试线费用
        /// </summary>
        public int ExpenseType { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 工装名称
        /// </summary>      
        public string WorkName { get; set; }
        /// <summary>
        /// 工装单价
        /// </summary> 
        public decimal UnitPriceOfTooling { get; set; }
        /// <summary>
        /// 工装数量
        /// </summary> 
        public int ToolingCount { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 需求：名称和单价一样的话要合并数据  这个Ids 就是储存合并后所有数据的id
        /// </summary>
        public HashSet<long> Ids { get; set; }
    }
}
