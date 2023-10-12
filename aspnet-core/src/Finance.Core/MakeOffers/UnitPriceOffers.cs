using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 报价分析看板中的 产品单价表 实体类
    /// </summary>
    public class UnitPriceOffers : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 模组id 关联ModelCount
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
        /// 毛利率 存json  模型为 GrossMarginModel
        /// </summary>
        public string GrossMarginList { get; set; }
        /// <summary>
        /// 梯度
        /// </summary>
        public string gradient { get; set; }
        /// <summary>
        /// 梯度(K/Y)
        /// </summary>
        public virtual string GradientValue { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }
    }
}
