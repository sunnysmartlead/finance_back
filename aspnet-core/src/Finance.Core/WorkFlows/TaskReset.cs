using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 任务重置表
    /// </summary>
    public class TaskReset : Entity<long>
    {
        /// <summary>
        /// 工作流节点实例Id
        /// </summary>
        public virtual string NodeInstanceId { get; set; }

        /// <summary>
        /// 重置用户Id
        /// </summary>
        public virtual long ResetUserId { get; set; }

        /// <summary>
        /// 目标用户Id
        /// </summary>
        public virtual long TargetUserId { get; set; }

    }
}
