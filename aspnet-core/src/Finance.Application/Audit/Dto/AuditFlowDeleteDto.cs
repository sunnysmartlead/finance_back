using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit.Dto
{
    /// <summary>
    /// 流程删除记录表传入Dto
    /// </summary>
    public class AuditFlowDeleteDto
    {
        /// <summary>
        /// 对应流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 删除理由
        /// </summary>
        public string DeleteReason { get; set; }
    }
}
