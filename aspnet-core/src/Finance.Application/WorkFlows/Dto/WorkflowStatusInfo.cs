using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class WorkflowStatusInfo
    {
        /// <summary>
        /// 流程实例
        /// </summary>
        public virtual List<NodeInstance> NodeInstance { get; set; }

    }
}
