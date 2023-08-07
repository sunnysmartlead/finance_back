using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 获取核价表输入参数
    /// </summary>
    public class GetPriceEvaluationSummaryTableInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 模组数量Id（即零件Id）
        /// </summary>
        [Required]
        public virtual long ProductId { get; set; }
    }
}
