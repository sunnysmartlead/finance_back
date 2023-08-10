using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// 营销部 返回 交互类
    /// </summary>
    public class ReturnSalesDepartmentDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 表单名称  如{手板件费用,模具费,生产设备费}
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// 核价金额
        /// </summary>
        public decimal PricingMoney { get; set; }
        /// <summary>
        /// 报价系数
        /// </summary>
        public decimal OfferCoefficient { get; set; }
        /// <summary>
        /// 报价金额
        /// </summary>
        public decimal OfferMoney { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
