using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Dto
{
    /// <summary>
    /// 财务维护 汇率表 交互类
    /// </summary>
    public class ExchangeRateDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public virtual long Id { get; set; }
        /// <summary>
        /// 货币种类(比如 CNY)
        /// </summary>
        public virtual string ExchangeRateKind { get; set; }
        /// <summary>
        /// 汇率的值 
        /// </summary>
        public virtual List<YearOrValueMode> ExchangeRateValue { get; set; }
    }
}
