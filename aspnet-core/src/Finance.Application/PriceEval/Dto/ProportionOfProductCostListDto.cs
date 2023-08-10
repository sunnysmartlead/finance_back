using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 产品成本比例
    /// </summary>
    public class ProportionOfProductCostListDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 所占百分比
        /// </summary>
        public virtual decimal Proportion { get; set; }
    }
}
