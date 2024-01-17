using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 节点开始/更新时间记录
    /// </summary>
    public class NodeTime : Entity<long>
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public virtual long WorkFlowInstanceId { get; internal set; }

        /// <summary>
        /// 节点Id
        /// </summary>
        public virtual long NodeInstance { get; internal set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public virtual DateTime? StartTime { get; internal set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime? UpdateTime { get; internal set; }
    }
}
