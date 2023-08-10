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
    /// 样品阶段
    /// </summary>
    [Table("Pe_Sample")]
    public class Sample : FullAuditedEntity<long>
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
        /// 样品阶段名称（从字典明细表取值，FinanceDictionaryId是【SampleName】）
        /// </summary>
        public virtual string Name { get; set; }


        /// <summary>
        /// 需求量
        /// </summary>
        public virtual decimal Pcs { get; set; }
    }
}
