using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit.Dto
{
    /// <summary>
    /// 审批流程流转明细表Dto
    /// </summary>
    public class AuditFlowDetailDto
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        [Required]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 流程Identifier
        /// </summary>
        [Required]
        public string ProcessIdentifier { get; set; }
        /// <summary>
        /// 用户ID(用户表主键)
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 意见
        /// </summary>
        [Required]
        public OPINIONTYPE Opinion { get; set; }
        /// <summary>
        /// 意见说明
        /// </summary>
        public string OpinionDescription { get; set; }
    }
}
