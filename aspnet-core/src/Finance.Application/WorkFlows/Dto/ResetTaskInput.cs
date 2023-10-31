using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class ResetTaskInput
    {
        /// <summary>
        /// 工作流节点实例Id
        /// </summary>
        public virtual long NodeInstanceId { get; set; }

        /// <summary>
        /// 目标用户Id
        /// </summary>
        public virtual long TargetUserId { get; set; }
    }
}
