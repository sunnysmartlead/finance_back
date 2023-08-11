using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows
{
    /// <summary>
    /// 工作流
    /// </summary>
    public class Workflow : Entity<string>
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public virtual int Version { get; set; }
    }

    /// <summary>
    /// 工作流节点
    /// </summary>
    public class Node : Entity<string>
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
        /// 数据类型，字典表主键（如果为开始或结束节点，即其不需要流转数据输入，则为空 string.Empty 空字符串）
        /// </summary>
        public virtual string FinanceDictionaryId { get; set; }

        /// <summary>
        /// 激活数字（必须接受到此输入。节点才激活，实例的NodeInstanceStatus改为Current），为空表示无条件激活
        /// </summary>
        public virtual string Activation { get; set; }

        /// <summary>
        /// 操作角色
        /// </summary>
        public virtual string RoleId { get; set; }

        public virtual NodeType NodeType { get; set; }
    }

    public enum NodeType : byte
    {
        Ordinary,
        Start,
        End
    }

    /// <summary>
    /// 工作流的线
    /// </summary>
    public class Line : Entity<string>
    {
        ///// <summary>
        ///// 不被映射到数据库的Id
        ///// </summary>
        //[NotMapped]
        //public override string Id => $"{WorkFlowId}-{SoureNodeId}-{TargetNodeId}";

        /// <summary>
        /// 工作流Id
        /// </summary>
        public virtual string WorkFlowId { get; set; }

        /// <summary>
        /// 源节点Id
        /// </summary>
        public virtual string SoureNodeId { get; set; }

        /// <summary>
        /// 目标节点Id
        /// </summary>
        public virtual string TargetNodeId { get; set; }

        /// <summary>
        /// 目标节点顺序
        /// </summary>
        public virtual byte Index { get; set; }

        /// <summary>
        /// 激活数字（必须接受到此输入，线才激活，实例的IsCurrent改为True），为空表示无条件激活
        /// </summary>
        public virtual string FinanceDictionaryDetailId { get; set; }

        /// <summary>
        /// 激活后允许出现的目标节点的选项，为空表示全部（为处理多线退回）
        /// </summary>
        public virtual string FinanceDictionaryDetailIds { get; set; }

        /// <summary>
        /// 线的类型，默认普通线
        /// </summary>
        public virtual LineType LineType { get; set; }
    }

    public enum LineType : byte
    {
        /// <summary>
        /// 普通线
        /// </summary>
        Ordinary = 0,

        /// <summary>
        /// 退回线
        /// </summary>
        Reset,
    }

}
