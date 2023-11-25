using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class WorkflowDeleteDto
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// 删除理由
        /// </summary>
        public virtual string Comment { get; set; }
    }
}
