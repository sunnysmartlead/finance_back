using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowTest
{
    public class ReulstData2<T>
    {
        public virtual T Result { get; set; }

        public virtual string TargetUrl { get; set; }

        public virtual bool Success { get; set; }

        public virtual string Error { get; set; }

        public virtual bool UnAuthorizedRequest { get; set; }

        public virtual bool __abp { get; set; }

    }
    public class ReulstData<T>
    {
        public virtual PagedResult<T> Result { get; set; }

        public virtual string TargetUrl { get; set; }

        public virtual bool Success { get; set; }

        public virtual string Error { get; set; }

        public virtual bool UnAuthorizedRequest { get; set; }

        public virtual bool __abp { get; set; }

    }

    public class PagedResult<T>
    {
        public virtual List<T> Items { get; set; }
        public virtual long TotalCount { get; set; }
    }

    /// <summary>
    /// 用户待办
    /// </summary>
    public class UserTask
    {
        public virtual long Id { get; set; }

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
    /// 财务字典dto
    /// </summary>
    public class FinanceDictionaryListDto
    {
        public virtual string Id { get; set; }

        /// <summary>
        /// 字典显示名
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
