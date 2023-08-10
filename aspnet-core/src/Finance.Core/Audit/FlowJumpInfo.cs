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
    /// 流程跳转信息表
    /// </summary>
    public class FlowJumpInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 上一个流程
        /// </summary>
        [Column("PREVIOUSPROCESSIDENTIFIER")]
        public string PreviousProcessIdentifier { get; set; }
        /// <summary>
        /// 触发条件
        /// </summary>
        [Column("CONDITION")]
        public OPINIONTYPE Condition { get; set; }
        /// <summary>
        /// 下一个流程
        /// </summary>
        [Column("NEXTPROCESSIDENTIFIER")]
        public string NextProcessIdentifier { get; set; }
    }
}
