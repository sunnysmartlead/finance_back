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
    /// 车型下的模组数量
    /// </summary>
    [Table("Pe_CarModelCount")]
    public class CarModelCount : FullAuditedEntity<long>
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
        /// 车型
        /// </summary>
        public virtual string CarModel { get; set; }

        /// <summary>
        /// 序号（正序排序，从1开始）
        /// </summary>
        [Required]
        public virtual long Order { get; set; }

        /// <summary>
        /// 客户零件号
        /// </summary>
        public virtual string PartNumber { get; set; }

        /// <summary>
        /// 子项目代码
        /// </summary>
        public virtual string Code { get; set; }


        /// <summary>
        /// 产品（产品名称从这里取）（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 产品大类（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string ProductType { get; set; }

        /// <summary>
        /// 像素
        /// </summary>
        [Required]
        public virtual string Pixel { get; set; }

        /// <summary>
        /// 我司角色
        /// </summary>
        public virtual string OurRole { get; set; }

        /// <summary>
        /// 市场份额（%）
        /// </summary>
        [Required]
        public virtual decimal MarketShare { get; set; }

        /// <summary>
        /// 模组搭载率
        /// </summary>
        [Required]
        public virtual decimal ModuleCarryingRate { get; set; }

        /// <summary>
        /// 单车产品数量
        /// </summary>
        [Required]
        public virtual int SingleCarProductsQuantity { get; set; }
    }

    /// <summary>
    /// 车型下的模组数量年份
    /// </summary>
    [Table("Pe_CarModelCountYear")]
    public class CarModelCountYear : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 主表 车型下的模组数量（CarModelCount） 的Id
        /// </summary>
        [Required]
        public virtual long CarModelCountId { get; set; }

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
        public virtual decimal Quantity { get; set; }

    }
}
