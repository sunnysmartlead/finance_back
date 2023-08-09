using Finance.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Dto
{
    /// <summary>
    /// 查询汇率 交互类
    /// </summary>
    public class ExchangeRateInputDto : PagedInputDto
    {
        /// <summary>
        /// 查询过滤关键字 货币种类(比如 CNY)
        /// </summary>
        public virtual string ExchangeRateKind { get; set; }
    }
}
