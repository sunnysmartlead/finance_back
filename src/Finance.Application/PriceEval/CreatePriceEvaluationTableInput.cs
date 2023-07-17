using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 生成核价表方法 参数输入
    /// </summary>
    public class CreatePriceEvaluationTableInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }
    }
}
