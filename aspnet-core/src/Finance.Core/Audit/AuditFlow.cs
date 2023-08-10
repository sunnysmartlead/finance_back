using Abp.Domain.Entities;
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
    /// 审批流程主表
    /// </summary>
    public class AuditFlow : FullAuditedEntity<long>
    {
        /// <summary>
        /// 核报价项目名称
        /// </summary>
        [Column("QUOTEPROJECTNAME")]
        public string QuoteProjectName { get; set; }
        /// <summary>
        /// 核报价项目代码
        /// </summary>
        [Column("QUOTEPROJECTNUMBER")]
        public string QuoteProjectNumber { get; set; }
        /// <summary>
        /// 核报价流程版本
        /// </summary>
        [Column("QUOTEVERSION")]
        public int QuoteVersion { get; set; }
        /// <summary>
        /// 核价是否有效(0:无效,1:有效)
        /// </summary>
        [Column("ISVALID")]
        public bool IsValid { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARKS")]
        public string Remarks { get; set; }

    }

    /// <summary>
    /// 已完成流程索引
    /// </summary>
    public class AuditFinishedProcess : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程Id
        /// </summary>
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 已完成流程索引
        /// </summary>
        [Column("FLOWPROCESSIDENTIFIER")]
        public string FlowProcessIdentifier { get; set; }
    }

    /// <summary>
    /// 当前流程索引
    /// </summary>
    public class AuditCurrentProcess : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程Id
        /// </summary>
        [Column("AUDITFLOWID")]
        public long AuditFlowId{ get; set; }
        /// <summary>
        /// 当前流程索引
        /// </summary>
        [Column("FLOWPROCESSIDENTIFIER")]
        public string FlowProcessIdentifier { get; set; }
    }
}
