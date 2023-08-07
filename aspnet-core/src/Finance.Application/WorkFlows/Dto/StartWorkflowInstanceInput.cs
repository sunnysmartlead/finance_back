using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class StartWorkflowInstanceInput
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public virtual string WorkflowId { set; get; }

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { set; get; }

        /// <summary>
        /// 流转意见
        /// </summary>
        public virtual string FinanceDictionaryDetailId { set; get; }
    }
}
