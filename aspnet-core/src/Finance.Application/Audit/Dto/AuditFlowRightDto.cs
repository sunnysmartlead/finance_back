using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit.Dto
{
    /// <summary>
    /// 流程权限请求输入Dto
    /// </summary>
    public class ProcessRightInputDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 对应流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
    }

    /// <summary>
    /// 返回用户所属流程中具有权限的节点信息Dto
    /// </summary>
    public class AuditFlowRightDto
    {
        /// <summary>
        /// 返回的节点信息
        /// </summary>
        public List<AuditFlowRight> AuditFlowRightList { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; } = "执行成功";
    }

    /// <summary>
    /// 流程信息
    /// </summary>
    public class AuditFlowRightDetailDto : Entity<long>
    {
        internal virtual bool IsReset { get; set; }

        /// <summary>
        /// 流程标识符
        /// </summary>
        public string ProcessIdentifier { get; set; }
        /// <summary>
        /// 流程界面名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 权限类型
        /// </summary>
        public RIGHTTYPE Right { get; set; }
        /// <summary>
        /// 是否回退
        /// </summary>
        public bool IsRetype { get; set; }
        /// <summary>
        /// 流转说明
        /// </summary>
        public string JumpDescription { get; set; }

        /// <summary>
        /// 要求完成时间
        /// </summary>
        public DateTime? RequiredTime { get; set; }

        /// <summary>
        /// 实际完成时间
        /// </summary>
        public DateTime? Time { get; set; }

        /// <summary>
        /// 完成状态
        /// </summary>
        public FlowStatus FlowStatus { get; set; }

        /// <summary>
        /// 操作用户Id列表
        /// </summary>
        public virtual List<int> TaskUserIds { get; set; }
    }

    /// <summary>
    /// 返回用户所有具有权限的流程和界面
    /// </summary>
    public class AuditFlowRightInfoDto
    {
        /// <summary>
        /// 对应流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 对应流程标题
        /// </summary>
        public string AuditFlowTitle { get; set; }
        /// <summary>
        /// 流程类型
        /// </summary>
        public string AuditFlowType { get; set; }
        /// <summary>
        /// 返回的节点信息
        /// </summary>
        public List<AuditFlowRightDetailDto> AuditFlowRightDetailList { get; set; }
    }

}
