using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.AllManufacturingCost
{
    public class ManufacturingCostInput
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 主表 模组数量（ModelCount） 的Id
        /// </summary>
        [Required]
        public virtual long ProductId { get; set; }

        /// <summary>
        /// SMT制造成本 CostType，填1（全生命周期制造成本，年份填0）
        /// </summary>
        public virtual List<AllManufacturingCostInput> Smt { get; set; }

        /// <summary>
        /// COB制造成本 CostType，填2（全生命周期制造成本，年份填0）
        /// </summary>
        public virtual List<AllManufacturingCostInput> Cob { get; set; }

        /// <summary>
        /// 其他制造成本 CostType，填4（全生命周期制造成本，年份填0）
        /// </summary>
        public virtual List<OtherManufacturingCostInput> Other { get; set; }
    }
}
