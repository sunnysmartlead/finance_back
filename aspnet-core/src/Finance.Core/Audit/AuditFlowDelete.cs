using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit
{
    /// <summary>
    /// 流程删除记录表
    /// </summary>
    public class AuditFlowDelete : FullAuditedEntity<long>
    {
        /// <summary>
        /// 对应流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string AuditFlowName { get; set; }
        /// <summary>
        /// 流程版本
        /// </summary>
        public long AuditFlowVersion { get; set; }
        /// <summary>
        /// 流程发起人
        /// </summary>
        public long AuditFlowUserId { get; set; }
        /// <summary>
        /// 流程删除人
        /// </summary>
        public long AuditFlowDeleterId { get; set; }
        /// <summary>
        /// 删除原因
        /// </summary>
        public string DeleteReason { get; set; }
    }
}
