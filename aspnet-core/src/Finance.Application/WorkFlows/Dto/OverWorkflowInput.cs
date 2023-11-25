using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class OverWorkflowInput
    {

        /// <summary>
        /// 流程Id
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 删除理由
        /// </summary>
        public string DeleteReason { get; set; }
    }
}
