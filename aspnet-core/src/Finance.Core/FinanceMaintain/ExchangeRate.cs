using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.FinanceMaintain
{
    /// <summary>
    /// 汇率录入表
    /// </summary>
    public class ExchangeRate : FullAuditedEntity<long>
    {
        /// <summary>
        /// 货币种类(比如 CNY)
        /// </summary>
        public virtual string ExchangeRateKind { get; set; }
        /// <summary>
        /// 汇率的值  存json字符串
        /// </summary>
        public virtual string ExchangeRateValue { get; set; }
    }
}
