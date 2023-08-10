using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.FinanceMaintain
{
    /// <summary>
    /// 财务维护 毛利率方案
    /// </summary>
    public class GrossMarginForm : FullAuditedEntity<long>
    {
        /// <summary>
        /// 毛利率方案名称
        /// </summary>
        public virtual string GrossMarginName { get; set; }
        /// <summary>
        /// 毛利率  规则 每个毛利率用  |  隔开
        /// </summary>
        public virtual string GrossMarginPrice { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public virtual bool IsDefaultn { get; set; }
    }
}
