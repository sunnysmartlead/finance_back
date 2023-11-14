using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    /// <summary>
    /// 用户待办
    /// </summary>
    public class UserTask : EntityDto<long>
    {
        internal virtual bool IsReset { get; set; }

        internal virtual string RoleIds { get; set; }
        /// <summary>
        /// 工作流实例Id
        /// </summary>
        public virtual long WorkFlowInstanceId { get; set; }

        /// <summary>
        /// 流程类型名称
        /// </summary>
        public virtual string WorkFlowName { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public virtual string NodeName { get; set; }

        /// <summary>
        /// 流程创建时间
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public virtual string TaskUser { get; set; }

        /// <summary>
        /// 操作用户Id列表
        /// </summary>
        public virtual List<int> TaskUserIds { get; set; }

        /// <summary>
        /// 工作流状态
        /// </summary>
        public virtual WorkflowState WorkflowState { get; set; }

        /// <summary>
        /// 流程流程标识符
        /// </summary>
        public virtual string ProcessIdentifier { get; set; }


        /// <summary>
        /// 是否为回退
        /// </summary>
        public virtual bool IsBack { get; set; }

        /// <summary>
        /// 审批评论
        /// </summary>
        public virtual string Comment { get; set; }
    }
}
