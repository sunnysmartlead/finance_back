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
    /// 工装治具部分表
    /// </summary>
    [Table("TOOLINGFIXTUREINFO")]
    public class ToolingFixtureInfo : FullAuditedEntity<long>
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
        /// 治具名称
        /// </summary>
        [Column("TOOLINGNAME")]
        public string ToolingName { get; set; }
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
        /// <summary>
        /// 工装治具部分工装名称
        /// </summary>
        [Column("TOOLINGNAME")]
        public string WorkName { get; set; }
        /// <summary>
        /// 工装治具部分数量
        /// </summary>
        [Column("TOOLINGCOUNT")]
        public int ToolingCount { get; set; }
        /// <summary>
        /// 工装治具部分工装单价
        /// </summary>
        [Column("UNITPRICEOFTOOLING", TypeName = "decimal(18,4)")]
        public decimal UnitPriceOfTooling { get; set; }
        /// <summary>
        /// 工装治具部分测试线名称
        /// </summary>
        [Column("TESTLINENAME")]
        public string TestLineName { get; set; }
        /// <summary>
        /// 工装治具部分测试线数量
        /// </summary>
        [Column("TESTLINECOUNT")]
        public int TestLineCount { get; set; }
        /// <summary>
        /// 工装治具部分测试线线束单价
        /// </summary>
        [Column("UNITPRICEOFTESTLINE", TypeName = "decimal(18,4)")]
        public decimal UnitPriceOfTestLine { get; set; }
    }
}
