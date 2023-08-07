using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit
{
    /// <summary>
    /// 在发生拒绝的情况下，需要清除哪些流程记录
    /// </summary>
    public class FlowClearInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 当前处于的流程标识符
        /// </summary>
        [Column("CURRENTPROCESSIDENTIFIER")]
        public string CurrentProcessIdentifier { get; set; }
        /// <summary>
        /// 需要清除的流程标识符
        /// </summary>
        [Column("CLEARPROCESSIDENTIFIER")]
        public string ClearProcessIdentifier { get; set; }
    }
}
