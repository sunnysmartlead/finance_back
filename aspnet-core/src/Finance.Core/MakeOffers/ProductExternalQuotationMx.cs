using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 产品报价清单实体类
    /// </summary>
    public class ProductExternalQuotationMx : FullAuditedEntity<long>
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
        /// 年份
        /// </summary>     
        public virtual string Year { get; set; }       
        /// <summary>
        /// 单价
        /// </summary>     
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
