using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 报价审核表 中的  内部核价信息
    /// </summary>
    public class InternalInformation : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 模组数量主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// BOM成本
        /// </summary>
        public decimal BOMCost { get; set; }
        /// <summary>
        /// 制造成本
        /// </summary>
        public decimal YieldCost { get; set; }
        /// <summary>
        /// 损耗成本
        /// </summary>
        public decimal LossCost { get; set; }
        /// <summary>
        /// 物流成本
        /// </summary>
        public decimal logisticsCost { get; set; }
        /// <summary>
        /// MOQ分摊成本
        /// </summary>
        public decimal MOQCost { get; set; }
        /// <summary>
        /// 其他（质量+财务成本）
        /// </summary>
        public decimal ElseCost { get; set; }
        /// <summary>
        /// 总成本
        /// </summary>
        public decimal AllCost { get; set; }
        /// <summary>
        /// 总成本
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }
    }
}
