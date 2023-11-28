using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// NRE报价清单
    /// </summary>
    public class NreQuotationList : FullAuditedEntity<long>
    {
        /// <summary>
        /// 对外报价单的ID
        /// </summary>
        public long ExternalQuotationId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>       
        public virtual string ProductName { get; set; }
        /// <summary>
        /// 走量
        /// </summary>       
        public virtual decimal TravelVolume { get; set; }
        /// <summary>
        /// 手板件费
        /// </summary>
        public virtual decimal HandmadePartsFee { get; set; }
        /// <summary>
        /// 模具费
        /// </summary>
        public virtual decimal MyPropMoldCosterty { get; set; }
        /// <summary>
        /// 工装治具费
        /// </summary>
        public virtual decimal CostOfToolingAndFixtures { get; set; }
        /// <summary>
        /// 实验费
        /// </summary>
        public virtual decimal ExperimentalFees { get; set; }
        /// <summary>
        /// 研发费	
        /// </summary>
        public virtual decimal RDExpenses { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
