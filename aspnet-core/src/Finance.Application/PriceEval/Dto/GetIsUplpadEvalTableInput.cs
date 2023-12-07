using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class GetIsUplpadEvalTableInput
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual decimal Year { get; set; }

        /// <summary>
        /// 年份上下
        /// </summary>
        public virtual YearType UpDown { get; set; }
    }
}
