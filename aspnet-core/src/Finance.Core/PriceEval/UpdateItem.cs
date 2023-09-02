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
    /// 修改项
    /// </summary>
    [Table("Pe_UpdateItem")]
    public class UpdateItem : FullAuditedEntity<long>
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 模组数量Id（即零件Id）
        /// </summary>
        [Required]
        public virtual long ProductId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public virtual long SolutionId { get; set; }

        /// <summary>
        /// 修改项类型
        /// </summary>
        public virtual UpdateItemType UpdateItemType { get; set; }

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
        /// 修改项Json
        /// </summary>
        public virtual string MaterialJson { get; set; }

        /// <summary>
        /// 上传佐证材料
        /// </summary>
        [Required]
        public virtual long File { get; set; }
    }

    /// <summary>
    /// 修改项类型
    /// </summary>
    public enum UpdateItemType : byte 
    {
        /// <summary>
        /// 物料成本
        /// </summary>
        Material,

        /// <summary>
        /// 损耗成本
        /// </summary>
        LossCostItem,

        /// <summary>
        /// 制造成本
        /// </summary>
        ManufacturingCost,

        /// <summary>
        /// 质量成本
        /// </summary>
        QualityCost,

        /// <summary>
        /// 其他成本
        /// </summary>
        OtherCost,
    }

}
