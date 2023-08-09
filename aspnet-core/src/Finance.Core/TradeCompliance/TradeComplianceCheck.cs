using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TradeCompliance
{
    /// <summary>
    /// 贸易合规检查表
    /// </summary>
    public class TradeComplianceCheck : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程表ID
        /// </summary>
        [Required]
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// MoudelCount表id(零件号)
        /// </summary>
        [Required]
        [Column("PRODUCTID")]
        public long ProductId { get; set; }
        /// <summary>
        /// 产品名称（零件1，零件2...）
        /// </summary>
        [Required]
        [Column("PRODUCTNAME")]
        public string ProductName { get; set; }
        /// <summary>
        /// 产品小类
        /// </summary>         
        [Required]
        [Column("PRODUCTTYPE")]
        public string ProductType { get; set; }
        /// <summary>
        /// 最终出口国家
        /// </summary>
        [Required]
        [Column("COUNTRY")]
        public string Country { get; set; }
        /// <summary>
        /// 产品公允价
        /// </summary>
        [Required]
        [Column("PRODUCTFAIRVALUE")]
        public decimal ProductFairValue { get; set; }
        /// <summary>
        /// Eccn成分价值占比
        /// </summary>
        [Column("ECCNPRICEPERCENT", TypeName = "decimal(18,4)")]
        public decimal EccnPricePercent { get; set; }
        /// <summary>
        /// 待定成分价值占比
        /// </summary>
        [Column("PENDINGPRICEPERCENT", TypeName = "decimal(18,4)")]
        public decimal PendingPricePercent { get; set; }
        /// <summary>
        /// 合计价值占比
        /// </summary>
        [Column("AMOUNTPRICEPERCENT", TypeName = "decimal(18,4)")]
        public decimal AmountPricePercent { get; set; }
        /// <summary>
        /// 分析结论
        /// </summary>
        [Column("ANALYSISCONCLUSION")]
        public string AnalysisConclusion { get; set; }
    }
}
