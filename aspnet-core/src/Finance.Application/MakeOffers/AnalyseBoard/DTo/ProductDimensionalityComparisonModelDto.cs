using Finance.MakeOffers.AnalyseBoard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.DTo
{
    /// <summary>
    /// 产品维度对比-产品单价  交互类
    /// </summary>
    public class ProductDimensionalityComparisonModelDto : UnitPriceModel
    {
        /// <summary>
        /// 客户目标价
        /// </summary>
        public decimal CustomerTargetPrice { get; set; }
        /// <summary>
        /// 本次报价
        /// </summary>
        public decimal ThisQuotation { get; set; }
        /// <summary>
        /// 上一轮报价
        /// </summary>
        public decimal NextRound { get; set; }

    }
}
