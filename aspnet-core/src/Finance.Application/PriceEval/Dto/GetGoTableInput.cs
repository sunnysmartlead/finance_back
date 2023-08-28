using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 获取推移图输入
    /// </summary>
    public class GetGoTableInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        [Required]
        public virtual int InputCount { get; set; }
    }
}
