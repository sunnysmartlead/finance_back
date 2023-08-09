using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 报价审核表下载 内部核价信息 模板
    /// </summary>
    public class PricingModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// BOM成本
        /// </summary>
        public decimal BOM { get; set; }
        /// <summary>
        /// 生产成本
        /// </summary>
        public decimal ProduceCost { get; set; }
        /// <summary>
        /// 良损率
        /// </summary>
        public decimal Yield { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public decimal Freight { get; set; }
        /// <summary>
        /// MOQ分摊成本
        /// </summary>
        public decimal MOQ { get; set; }
        /// <summary>
        /// 质量成本
        /// </summary>
        public decimal QualityCost { get; set; }
        /// <summary>
        /// 总成本
        /// </summary>
        public decimal SumCost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
