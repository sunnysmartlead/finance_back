using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment
{
    /// <summary>
    /// 损耗率表
    /// </summary>
    [Table("LOSSRATEINFO")]
    public class LossRateInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程表ID
        /// </summary>
        [Required]
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// MoudelCount表id
        /// </summary>
        [Required]
        [Column("PRODUCTID")]
        public long ProductId { get; set; }
        /// <summary>
        /// 产品名称（零件1，零件2...）
        /// </summary>
        [Column("PRODUCT")]
        public string Product { get; set; }
        /// <summary>
        /// 产品大类
        /// </summary>
        [Required]
        [Column("SUPERTYPE")]
        public string SuperType { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [Column("IDNUMBER")]
        public int IdNumber { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        [Column("CATEGORYNAME")]
        public string CategoryName { get; set; }
    }

    /// <summary>
    /// 损耗率年份
    /// </summary>
    [Table("LOSSRATEYEARINFO")]
    public class LossRateYearInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程表ID
        /// </summary>
        [Required]
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }

        /// <summary>
        /// 主表损耗率LossRateInfo的id
        /// </summary>
        [Required]
        [Column("LOSSRATEINFOID")]
        public virtual long LossRateInfoId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        [Column("YEAR")]
        public virtual int Year { get; set; }

        /// <summary>
        /// 损耗率
        /// </summary>
        [Required]
        [Column("RATE", TypeName = "decimal(18,4)")]
        public virtual decimal Rate { get; set; }
    }
    
}
