using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 查询已经生成的核价表 参数输入
    /// </summary>
    public class GetPriceEvaluationTableResultInput
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

        /// <summary>
        /// 是否为全生命周期核价表
        /// </summary>
        public virtual bool IsAll { get; set; }
    }
}
