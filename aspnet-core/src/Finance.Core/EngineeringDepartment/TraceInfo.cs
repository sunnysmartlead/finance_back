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
    /// 追溯部分表(硬件及软件开发费用)
    /// </summary>
    [Table("TRACEINFO")]
    public class TraceInfo : FullAuditedEntity<long>
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
        [Required]
        [Column("PRODUCT")]
        public string Product { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [Column("IDNUMBER")]
        public int IdNumber { get; set; }
        /// <summary>
        /// 工序
        /// </summary>
        [Column("PROCEDURE")]
        public string Procedure { get; set; }
        /// <summary>
        /// 硬件设备
        /// </summary>
        [Column("HARDWAREEQUIPMENT")]
        public string HardwareEquipment { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [Column("NUMBER")]
        public int Number { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        [Column("UNITPRICE", TypeName = "decimal(18,4)")]
        public decimal UnitPrice { get; set; }
    }
}
