using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit
{
    /// <summary>
    /// 流程跳转权限表
    /// </summary>
    public class AuditFlowRight : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 流程标识符
        /// </summary>
        public string ProcessIdentifier { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 权限类型
        /// </summary>
        public RIGHTTYPE RightType { get; set; }
        /// <summary>
        /// 是否回退
        /// </summary>
        public bool IsRetype { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
