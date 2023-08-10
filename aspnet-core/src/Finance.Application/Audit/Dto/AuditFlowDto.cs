using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit.Dto
{
    /// <summary>
    /// 审批流程主表Dto
    /// </summary>
    public class AuditFlowDto
    {
        /// <summary>
        /// 核报价项目名称
        /// </summary>
        [Required]
        public string QuoteProjectName { get; set; }
        /// <summary>
        /// 核报价项目代码
        /// </summary>
        public string QuoteProjectNumber { get; set; }
        /// <summary>
        /// 核报价流程版本
        /// </summary>
        public int QuoteVersion { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }

    /// <summary>
    /// 审批流程有效Dto
    /// </summary>
    public class AuditFlowValidDto
    {
        /// <summary>
        /// 审批流程Id
        /// </summary>
        [Required]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }
    }
}
