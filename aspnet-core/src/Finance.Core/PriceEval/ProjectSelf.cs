using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Finance.PriceEval
{
    /// <summary>
    /// 项目自建表
    /// </summary>
    [Table("ProjectSelf")]
    public class ProjectSelf : FullAuditedEntity<long>
    {
        /// <summary>
        /// 客户
        /// </summary>
        public virtual string Custom { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public virtual string CustomName { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 子项目代码
        /// </summary>
        public virtual string SubCode { get; set; }

        /// <summary>
        /// 子项目描述
        /// </summary>
        public virtual string SubDescription { get; set; }
    }

    /// <summary>
    /// 基础库日志
    /// </summary>
    [Table("BaseStoreLog")]
    public class BaseStoreLog : CreationAuditedEntity<long>
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public virtual OperationType OperationType { get; set; }

        /// <summary>
        /// 操作的记录数量
        /// </summary>
        public virtual long Count { get; set; }

        /// <summary>
        /// 文本说明
        /// </summary>
        public virtual string Text { get; set; }
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperationType : byte
    {
        /// <summary>
        /// 增
        /// </summary>
        Insert,

        /// <summary>
        /// 编辑
        /// </summary>
        Update,

        /// <summary>
        /// 删除
        /// </summary>
        Delete,

        /// <summary>
        /// 导入
        /// </summary>
        Import
    }
}
