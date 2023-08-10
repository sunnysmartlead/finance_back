using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary
{
    /// <summary>
    /// 毛利率方案 交互类
    /// </summary>
    public class BacktrackGrossMarginDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public virtual long  Id { get; set; }   
        /// <summary>
        /// 毛利率方案名称
        /// </summary>
        public virtual string GrossMarginName { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public virtual List<decimal> GrossMarginPrice { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public virtual bool IsDefaultn { get; set; }

    }
}
