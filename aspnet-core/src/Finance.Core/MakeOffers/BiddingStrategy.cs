using Abp.Domain.Entities.Auditing;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 报价审核表 中的 报价策略
    /// </summary>
    public class BiddingStrategy : FullAuditedEntity<long>
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
        /// 产品
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossMargin { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 含佣金的毛利率
        /// </summary>
        public decimal GrossMarginCommission { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }

    }
}
