using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 质量成本
    /// </summary>
    public class QualityCostListDto
    {
        /// <summary>
        /// 修改项Id
        /// </summary>
        public virtual string EditId { get; set; }

        /// <summary>
        /// 修改备注
        /// </summary>
        public virtual string EditNotes { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        internal virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        internal virtual YearType UpDown { get; set; }

        /// <summary>
        /// 产品类别
        /// </summary>
        public virtual string ProductCategory { get; set; }

        /// <summary>
        /// 成本比例
        /// </summary>
        public virtual decimal CostProportion { get; set; }

        /// <summary>
        /// 质量成本（MAX)
        /// </summary>
        public virtual decimal QualityCost { get; set; }
    }
}
