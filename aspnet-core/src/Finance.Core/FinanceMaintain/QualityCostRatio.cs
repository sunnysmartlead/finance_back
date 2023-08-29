using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.FinanceMaintain
{
    /// <summary>
    /// 质量成本比例
    /// </summary>
    public class QualityCostRatio : FullAuditedEntity<long>
    {
        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 是否首款产品  true/是   false/否
        /// </summary>
        public bool IsItTheFirstProduct { get; set; }
    }
    /// <summary>
    /// 质量成本比例年份
    /// </summary>
    public class QualityCostRatioYear : FullAuditedEntity<long>
    {
        /// <summary>
        /// 质量成本比例ID
        /// </summary>
        public long QualityCostRatioId { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public decimal Value { get; set; }
    }
}
