using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class ResetList : CreationAuditedEntityDto<long>
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 工作流节点名称
        /// </summary>
        public virtual string NodeName { get; set; }

        /// <summary>
        /// 工作流节点状态
        /// </summary>
        public virtual NodeInstanceStatus NodeStatus { get; set; }

        /// <summary>
        /// 重置用户
        /// </summary>
        public virtual string ResetUser { get; set; }

        /// <summary>
        /// 目标用户
        /// </summary>
        public virtual string TargetUser { get; set; }

    }
}
