using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    public class NodeTime : Entity<long>
    {
        /// <summary>
        /// 节点Id
        /// </summary>
        public virtual long NodeInstance { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public virtual DateTime StartTime { get; set; }

        public virtual DateTime UpdateTime { get; set; }
    }
}
