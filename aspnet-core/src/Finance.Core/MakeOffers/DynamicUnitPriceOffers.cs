using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 报价分析看板中的 动态单价表 实体类
    /// </summary>
    public class DynamicUnitPriceOffers : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 目标单价(内部)
        /// </summary>
        public decimal InteriorTargetUnitPrice { get; set; }
        /// <summary>
        /// 目标价(内部)整套毛利率
        /// </summary>
        public decimal AllInteriorGrossMargin { get; set; }
        /// <summary>
        /// 目标价(内部)增加客供料毛利率
        /// </summary>
        public decimal AllInteriorClientGrossMargin { get; set; }
        /// <summary>
        /// 目标价(内部)剔除NRE分摊费用毛利率
        /// </summary>
        public decimal AllInteriorNreGrossMargin { get; set; }
        /// <summary>
        /// 目标单价(客户)
        /// </summary>
        public decimal ClientTargetUnitPrice { get; set; }
        /// <summary>
        /// 目标价(客户)整套毛利率
        /// </summary>
        public decimal AllClientGrossMargin { get; set; }
        /// <summary>
        /// (客户)增加客供料毛利率
        /// </summary>
        public decimal AllClientClientGrossMargin { get; set; }
        /// <summary>
        /// (客户)剔除NRE分摊费用毛利率
        /// </summary>
        public decimal AllClientNreGrossMargin { get; set; }
        /// <summary>
        /// 模组数量主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 单车产品数量
        /// </summary>
        public long ProductNumber { get; set; }
      
        /// <summary>
        /// 目标毛利率(内部)
        /// </summary>
        public decimal InteriorTargetGrossMargin { get; set; }
      
        /// <summary>
        /// 目标毛利率(客户)
        /// </summary>
        public decimal ClientTargetGrossMargin { get; set; }
        /// <summary>
        /// 本次报价-单价
        /// </summary>
        public decimal OfferUnitPrice { get; set; }
        /// <summary>
        /// 本次报价-毛利率
        /// </summary>
        public decimal OffeGrossMargin { get; set; }
        /// <summary>
        /// (本次报价增加客供料毛利率
        /// </summary>
        public decimal ClientGrossMargin { get; set; }
        /// <summary>
        /// 本次报价剔除NRE分摊费用毛利率
        /// </summary>
        public decimal NreGrossMargin { get; set; }
        
        /// <summary>
        /// 车型
        /// </summary>
        public virtual string CarModel { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }  
    }
}
