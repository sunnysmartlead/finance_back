using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.DTo
{
    /// <summary>
    /// 产品维度对比  交互类
    /// </summary>
    public class ProductDimensionalityComparisonDto
    {
        /// <summary>
        /// 产品单价
        /// </summary>
        public List<ProductDimensionalityComparisonModelDto>  UnitPrice { get; set; }
        /// <summary>
        /// 销售收入
        /// </summary>
        public List<ProductDimensionalityComparisonModelDto> SalesRevenue { get; set; }
        /// <summary>
        /// 销售成本
        /// </summary>
        public List<ProductDimensionalityComparisonModelDto> SellingCost { get; set; }
        /// <summary>
        /// 销售毛利率
        /// </summary>
        public List<ProductDimensionalityComparisonModelDto> SellinggrossProfitRate { get; set; }
    }
}
