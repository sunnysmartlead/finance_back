using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.AllManufacturingCost
{
    public class GetManufacturingCostInputDto
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
    }
}
