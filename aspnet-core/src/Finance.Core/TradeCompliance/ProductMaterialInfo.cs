using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;

namespace Finance.TradeCompliance
{
    /// <summary>
    /// 产品组成物料信息表
    /// </summary>
    public class ProductMaterialInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程表ID
        /// </summary>
        [Required]
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// TradeComplianceCheck表id(贸易合规表ID)
        /// </summary>
        [Required]
        [Column("TRADECOMPLIANCECHECKID")]
        public long TradeComplianceCheckId { get; set; }
        /// <summary>
        /// 物料在对应BOM中的ID（结构是s+Id,电子是e+Id）
        /// </summary>
        [Column("MATERIALIDINBOM")]
        public string MaterialIdInBom { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [Column("MATERIALCODE")]
        public string MaterialCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [Column("MATERIALNAME")]
        public string MaterialName { get; set; }
        /// <summary>
        /// 材料名称
        /// </summary>
        [Column("MATERIALDETAILNAME")]
        public string MaterialDetailName { get; set; }
        /// <summary>
        /// 物料数量
        /// </summary>
        [Column("COUNT")]
        public double Count { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        [Column("UNITPRICE")]
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 金额(单价*数量)
        /// </summary>
        [Column("AMOUNT")]
        public decimal Amount { get; set; }
        /// <summary>
        /// 物料管制状态分类
        /// </summary>
        public string ControlStateType { get; set; }
    }
}
