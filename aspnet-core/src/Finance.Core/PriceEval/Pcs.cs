using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 走量（PCS）
    /// </summary>
    [Table("Pe_Pcs")]
    public class Pcs : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 核价表主键
        /// </summary>
        [Required]
        public virtual long PriceEvaluationId { get; set; }

        /// <summary>
        /// 走量类型
        /// </summary>
        [Required]
        public virtual PcsType PcsType { get; set; }

        /// <summary>
        /// 车厂
        /// </summary>
        [Required]
        public virtual string CarFactory { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        [Required]
        public virtual string CarModel { get; set; }

        ///// <summary>
        ///// 系数
        ///// </summary>
        //public virtual decimal K { get; set; }

        /// <summary>
        /// 梯度（这个属性在最新的需求中已经被取消了，相应依赖它的代码也需要更改）
        /// </summary>
        [Required]
        public virtual decimal Kv { get; set; }
    }

    /// <summary>
    /// 终端走量年份
    /// </summary>
    [Table("Pe_PcsYear")]
    public class PcsYear : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 主表 终端走量（PCS） 的Id
        /// </summary>
        [Required]
        public virtual long PcsId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        [Required]
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        public virtual int Quantity { get; set; }

    }



    /// <summary>
    /// 走量类型
    /// </summary>
    public enum PcsType : byte
    {
        /// <summary>
        /// 客户输入
        /// </summary>
        Input,

        /// <summary>
        /// 内部评估
        /// </summary>
        Eval
    }

    /// <summary>
    /// 年份上下
    /// </summary>
    public enum YearType : byte
    {
        /// <summary>
        /// 全年度
        /// </summary>
        Year,

        /// <summary>
        /// 上半年
        /// </summary>
        FirstHalf,

        /// <summary>
        /// 下半年
        /// </summary>
        SecondHalf
    }
}
