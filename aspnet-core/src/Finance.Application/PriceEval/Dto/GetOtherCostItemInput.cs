using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 获取 其他成本项目 参数输入
    /// </summary>
    public class GetOtherCostItemInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

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
    }

    /// <summary>
    /// 获取 其他成本项目 参数输入
    /// </summary>
    public class GetOtherCostItem2Input
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

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
    }

    /// <summary>
    /// 获取 其他成本项目 参数输入
    /// </summary>
    public class GetOtherCostItem2ListInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        [Required]
        public virtual long SolutionId { get; set; }

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
    }
}
