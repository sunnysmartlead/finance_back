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
    /// 审批流程流转明细表
    /// </summary>
    public class AuditFlowDetail : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 流程Identifier
        /// </summary>
        [Column("PROCESSIDENTIFIER")]
        public string ProcessIdentifier { get; set; }
        /// <summary>
        /// 用户ID(用户表主键)
        /// </summary>
        [Column("USERID")]
        public long UserId { get; set; }
        /// <summary>
        /// 意见
        /// </summary>
        [Column("OPINION")]
        public OPINIONTYPE Opinion { get; set; }
        /// <summary>
        /// 意见说明
        /// </summary>
        [Column("OPINIONDESCRIPTION")]
        public string OpinionDescription { get; set; }
        /// <summary>
        /// 接收流程Identifier(如只有查看权限，则为空)
        /// </summary>
        [Column("RECEIVEPROCESSIDENTIFIER")]
        public string ReceiveProcessIdentifier { get; set; }
        /// <summary>
        /// 用户ID（接收人）(用户表主键)
        /// </summary>
        [Column("RECEIVERID")]
        public long ReceiverId { get; set; }
    }
}
