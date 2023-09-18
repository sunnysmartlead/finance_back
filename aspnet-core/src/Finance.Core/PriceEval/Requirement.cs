using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 要求（此表涉及到列转行）
    /// </summary>
    [Table("Pe_Requirement")]
    public class Requirement : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 核价表主键
        /// </summary>
        [Required]
        public virtual long PriceEvaluationId { get; set; }

        /// <summary>
        /// （%）
        /// </summary>
        [Required]
        public virtual decimal AnnualDeclineRate { get; set; }


        /// <summary>
        /// 年度返利要求（金额）
        /// </summary>
        [Required]
        public virtual decimal AnnualRebateRequirements { get; set; }

        /// <summary>
        /// 一次性折让率（%）
        /// </summary>
        [Required]
        public virtual decimal OneTimeDiscountRate { get; set; }

        /// <summary>
        /// 年度佣金比例（%）
        /// </summary>
        [Required]
        public virtual decimal CommissionRate { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份上下
        /// </summary>
        public virtual YearType UpDown { get; set; }
    }
}
