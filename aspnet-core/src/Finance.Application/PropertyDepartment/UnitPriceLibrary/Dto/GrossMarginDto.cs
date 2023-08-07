using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Dto
{
    /// <summary>
    /// 毛利率 交互类
    /// </summary>
    public class GrossMarginDto 
    {
        /// <summary>
        /// 毛利率id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 毛利率方案名称
        /// </summary>
        public string GrossMarginName { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public List<decimal> GrossMarginPrice { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public virtual bool IsDefaultn { get; set; }

    }
}
