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
    /// 工时工序静态字段表
    /// </summary>
    [Table("WORKINGHOURSINFO")]
    public class WorkingHoursInfo : FullAuditedEntity<long>
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
        /// 设备部分设备总价
        /// </summary>
        [Column("EQUIPMENTTOTALPRICE", TypeName = "decimal(18,4)")]
        public decimal EquipmentTotalPrice { get; set; }
        /// <summary>
        /// 追溯部分(硬件及软件开发费用)硬件总价
        /// </summary>
        [Column("HARDWARETOTALPRICE", TypeName = "decimal(18,4)")]
        public decimal HardwareTotalPrice { get; set; }
        /// <summary>
        /// 追溯部分(硬件及软件开发费用)追溯软件
        /// </summary>
        [Column("TRACEABILITYSOFTWARE")]
        public string TraceabilitySoftware { get; set; }
        /// <summary>
        /// 追溯部分(硬件及软件开发费用)开发费(追溯)
        /// </summary>
        [Column("TRACEABILITYDEVELOPMENTFEE", TypeName = "decimal(18,4)")]
        public decimal TraceabilityDevelopmentFee { get; set; }
        /// <summary>
        /// 追溯部分(硬件及软件开发费用)开图软件
        /// </summary>
        [Column("MAPPINGSOFTWARE")]
        public string MappingSoftware { get; set; }
        /// <summary>
        /// 追溯部分(硬件及软件开发费用)开发费（开图）
        /// </summary>
        [Column("MAPPINGDEVELOPMENTFEE", TypeName = "decimal(18,4)")]
        public decimal MappingDevelopmentFee { get; set; }
        /// <summary>
        /// 追溯部分(硬件及软件开发费用)软硬件总价
        /// </summary>
        [Column("SOFTWAREANDHARDWARETOTALPRICE", TypeName = "decimal(18,4)")]
        public decimal SoftwareAndHardwareTotalPrice { get; set; }
        /// <summary>
        /// 工装名称
        /// </summary>
        [Column("TOOLINGNAME")]
        public string ToolingName { get; set; }
        /// <summary>
        /// 工装数量
        /// </summary>
        [Column("TOOLINGNUM")]
        public int ToolingNum { get; set; }
        /// <summary>
        /// 工装单价
        /// </summary>
        [Column("TOOLINGPRICE", TypeName = "decimal(18,4)")]
        public decimal ToolingPrice { get; set; }
        /// <summary>
        /// 测试线名称
        /// </summary>
        [Column("TESTNAME")]
        public string TestName { get; set; }
        /// <summary>
        /// 测试线数量
        /// </summary>
        [Column("TESTNUM")]
        public int TestNum { get; set; }
        /// <summary>
        /// 测试线单价
        /// </summary>
        [Column("TESTPRICE", TypeName = "decimal(18,4)")]
        public decimal TestPrice { get; set; }
        /// <summary>
        /// 工装治具部分工装治具总价
        /// </summary>
        [Column("TOTALPRICEOFTOOLINGANDFIXTURES", TypeName = "decimal(18,4)")]
        public decimal TotalPriceOfToolingAndFixtures { get; set; }
    }
}
