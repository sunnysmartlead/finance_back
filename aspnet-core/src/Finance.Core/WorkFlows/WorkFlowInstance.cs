using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 工作流实例
    /// </summary>
    public class WorkflowInstance : FullAuditedEntity<long>
    {
        /// <summary>
        /// 工作流Id
        /// </summary>
        public virtual string WorkFlowId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// 工作流状态
        /// </summary>
        public virtual WorkflowState WorkflowState { get; set; }
    }

    public enum WorkflowState : byte
    {
        /// <summary>
        /// 执行中
        /// </summary>
        Running,

        /// <summary>
        /// 已结束
        /// </summary>
        Ended
    }

    /// <summary>
    /// 工作流节点实例
    /// </summary>
    public class NodeInstance : FullAuditedEntity<long>
    {
        /// <summary>
        /// 工作流Id
        /// </summary>
        public virtual string WorkFlowId { get; set; }

        /// <summary>
        /// 工作流节点Id
        /// </summary>
        public virtual string NodeId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 数据类型，字典表主键
        /// </summary>
        public virtual string FinanceDictionaryId { get; set; }

        /// <summary>
        /// 激活数字（必须接受到此输入，json long 数字串。节点才激活，实例的NodeInstanceStatus改为Current）
        /// </summary>
        public virtual string Activation { get; set; }

        /// <summary>
        /// 操作角色
        /// </summary>
        public virtual string RoleId { get; set; }

        /// <summary>
        /// 已操作用户的Id
        /// </summary>
        public virtual string OperatedUserIds { get; set; }

        /// <summary>
        /// 工作流实例Id
        /// </summary>
        public virtual long WorkFlowInstanceId { get; set; }

        /// <summary>
        /// 节点实例状态
        /// </summary>
        public virtual NodeInstanceStatus NodeInstanceStatus { get; set; }

        /// <summary>
        /// 节点实例结果（字典明细表主键）
        /// </summary>
        public virtual string FinanceDictionaryDetailId { get; set; }

        /// <summary>
        /// 数据，目前为记录随机调整节点而设置
        /// </summary>
        public virtual string Data { get; set; }

        /// <summary>
        /// 激活后允许出现的目标节点的选项，为空表示全部（为处理多线退回）
        /// </summary>
        public virtual string FinanceDictionaryDetailIds { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public virtual NodeType NodeType { get; set; }

        /// <summary>
        /// 流程流程标识符
        /// </summary>
        public virtual string ProcessIdentifier { get; set; }

        /// <summary>
        /// 是否退回
        /// </summary>
        public virtual bool IsBack { get; set; }

        /// <summary>
        /// 审批评论
        /// </summary>
        public virtual string Comment { get; set; }

    }

    /// <summary>
    /// 节点实例状态
    /// </summary>
    public enum NodeInstanceStatus : byte
    {
        /// <summary>
        /// 未经过
        /// </summary>
        [Description("未经过")]
        NotPassed,

        /// <summary>
        /// 当前
        /// </summary>
        [Description("当前")]
        Current,

        /// <summary>
        /// 已经过
        /// </summary>
        [Description("已经过")]
        Passed,

        /// <summary>
        /// 已重置
        /// </summary>
        [Description("已重置")]
        Reset,

        /// <summary>
        /// 已结束（表示已经被强行结束）
        /// </summary>
        [Description("已重置")]
        Over,
    }

    /// <summary>
    /// 节点实例结果
    /// </summary>
    public enum NodeInstanceResult : byte
    {
        /// <summary>
        /// 空
        /// </summary>
        Null,

        /// <summary>
        /// 完成
        /// </summary>
        Done,

        /// <summary>
        /// 是
        /// </summary>
        True,

        /// <summary>
        /// 否
        /// </summary>
        False,

        /// <summary>
        /// 退回
        /// </summary>
        Back
    }

    /// <summary>
    /// 工作流线实例
    /// </summary>
    public class LineInstance : FullAuditedEntity<long>
    {
        /// <summary>
        /// 工作流Id
        /// </summary>
        public virtual string WorkFlowId { get; set; }

        /// <summary>
        /// 工作流线Id
        /// </summary>
        public virtual string LineId { get; set; }

        /// <summary>
        /// 源实例节点Id
        /// </summary>
        public virtual string SoureNodeId { get; set; }

        /// <summary>
        /// 目标实例节点Id
        /// </summary>
        public virtual string TargetNodeId { get; set; }

        /// <summary>
        /// 目标节点顺序
        /// </summary>
        public virtual byte Index { get; set; }

        /// <summary>
        /// 激活数字（必须接受到此输入，线才激活，实例的IsCurrent改为True）
        /// </summary>
        public virtual string FinanceDictionaryDetailId { get; set; }

        /// <summary>
        /// 工作流实例Id
        /// </summary>
        public virtual long WorkFlowInstanceId { get; set; }

        /// <summary>
        /// 节点实例状态
        /// </summary>
        public virtual NodeInstanceStatus NodeInstanceStatus { get; set; }

        /// <summary>
        /// 节点是否已激活
        /// </summary>
        public virtual bool IsCurrent { get; set; }

        /// <summary>
        /// 激活后允许出现的目标节点的选项，为空表示全部（为处理多线退回）
        /// </summary>
        public virtual string FinanceDictionaryDetailIds { get; set; }

        /// <summary>
        /// 线的类型，默认普通线
        /// </summary>
        public virtual LineType LineType { get; set; }
    }

    /// <summary>
    /// 实例历史
    /// </summary>
    public class InstanceHistory : FullAuditedEntity<long>
    {
        /// <summary>
        /// 工作流Id
        /// </summary>
        public virtual string WorkFlowId { get; set; }

        /// <summary>
        /// 工作流实例Id
        /// </summary>
        public virtual long WorkFlowInstanceId { get; set; }

        /// <summary>
        /// 工作流节点Id
        /// </summary>
        public virtual string NodeId { get; set; }

        /// <summary>
        /// 工作流节点实例Id
        /// </summary>
        public virtual long NodeInstanceId { get; set; }

        /// <summary>
        /// 节点实例结果（字典明细表主键）填入的流转意见
        /// </summary>
        public virtual string FinanceDictionaryDetailId { get; set; }

        /// <summary>
        /// 审批评论
        /// </summary>
        public virtual string Comment { get; set; }


        /// <summary>
        /// 流转到的下一个用户Id
        /// </summary>
        public virtual long NextUserId { get; set; }
    }
}
