using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProductionControl
{
    /// <summary>
    /// 物流成本
    /// </summary>
    [Table("PRODUCTIONCONTROLINFO")]
    public class ProductionControlInfo : FullAuditedEntity<long>
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
        /// 年份
        /// </summary>
        [Column("YEAR")]
        public string Year { get; set; }
        /// <summary>
        /// 单pcs包装价格/元
        /// </summary>
        [Column("PERPACKAGINGPRICE")]
        public decimal PerPackagingPrice { get; set; }
        /// <summary>
        /// 运费/月
        /// </summary>
        [Column("FREIGHT")]
        public decimal Freight { get; set; }
        /// <summary>
        /// 仓储费用/元
        /// </summary>
        [Column("STORAGEEXPENSES")]
        public decimal StorageExpenses { get; set; }
        /// <summary>
        /// 月底需求量
        /// </summary>
        [Column("MONTHENDDEMAND")]
        public decimal MonthEndDemand { get; set; }
        /// <summary>
        /// 单PCS运输费
        /// </summary>
        [Column("PERFREIGHT")]
        public decimal PerFreight { get; set; }
        /// <summary>
        /// 单PCS总物流成本
        /// </summary>
        [Column("PERTOTALLOGISTICSCOST")]
        public decimal PerTotalLogisticsCost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARKS")]
        public string Remarks { get; set; }
    }
}
