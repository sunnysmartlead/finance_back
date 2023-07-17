using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 整体差异
    /// </summary>
    public class CostDetailVariance
    {
        /// <summary>
        /// 模块名
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 版本1
        /// </summary>
        public virtual decimal Version1 { get; set; }

        /// <summary>
        /// 版本2
        /// </summary>
        public virtual decimal Version2 { get; set; }

        /// <summary>
        /// 差异
        /// </summary>
        public virtual string Variance { get; set; }
    }

    /// <summary>
    /// 整体差异信息
    /// </summary>
    public class CostDetailVarianceInfo
    {
        /// <summary>
        /// bom成本
        /// </summary>
        public virtual decimal BomCost { get; set; }

        /// <summary>
        /// 制造成本
        /// </summary>
        public virtual decimal ManufacturingCost { get; set; }

        /// <summary>
        /// 物流成本
        /// </summary>
        public virtual decimal LogisticsCost { get; set; }

        /// <summary>
        /// 质量成本
        /// </summary>
        public virtual decimal QualityCost { get; set; }

        /// <summary>
        /// 总成本
        /// </summary>
        public virtual decimal TotalCost { get; set; }

        /// <summary>
        /// 单位售价
        /// </summary>
        public virtual decimal SellingPrice { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public virtual decimal GrossProfitMargin { get; set; }
    }
}

