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
    /// 分摊数量
    /// </summary>
    [Table("Pe_ShareCount")]
    public class ShareCount : FullAuditedEntity<long>
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
        /// 模组Id
        /// </summary>
        public virtual long ProductId { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }


        /// <summary>
        /// 分摊数量
        /// </summary>
        public virtual decimal Count { get; set; }

    }
}
