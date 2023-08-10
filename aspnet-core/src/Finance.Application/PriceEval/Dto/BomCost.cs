using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class BomCost
    {
        /// <summary>
        /// 物料列表
        /// </summary>
        public virtual List<Material> Material { get; set; }

        /// <summary>
        /// 材料成本合计（合计金额（人民币）合计）
        /// </summary>
        public virtual decimal TotalMoneyCynCount { get; set; }

        /// <summary>
        /// 电子料大类成本合计
        /// </summary>
        public virtual decimal ElectronicCount { get; set; }
    }
}
