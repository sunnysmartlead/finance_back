using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    ///  获取 制造成本汇总表 返回
    /// </summary>
    public class ProductionControlInfoListDto
    {
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 单pcs包装价格/元
        /// </summary>
        public decimal PerPackagingPrice { get; set; }

        /// <summary>
        /// 运费/月
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 仓储费用/元
        /// </summary>
        public decimal StorageExpenses { get; set; }

        /// <summary>
        /// 月底需求量
        /// </summary>
        public decimal MonthEndDemand { get; set; }

        /// <summary>
        /// 单PCS运输费
        /// </summary>
        public decimal PerFreight { get; set; }

        /// <summary>
        /// 单PCS总物流成本
        /// </summary>
        public decimal PerTotalLogisticsCost { get; set; }
    }
}
