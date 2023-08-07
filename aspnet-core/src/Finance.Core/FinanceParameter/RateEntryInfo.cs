using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.FinanceParameter
{
    /// <summary>
    /// 财务费率录入表
    /// </summary>
    [Table("RATEENTRYINFO")]
    public class RateEntryInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 直接制造率
        /// </summary>
        [Column("DIRECTMANUFACTURINGRATE", TypeName = "decimal(18,4)")]
        public decimal DirectManufacturingRate { get; set; }
        /// <summary>
        /// 间接人工率
        /// </summary>
        [Column("INDIRECTLABORRATE", TypeName = "decimal(18,4)")]
        public decimal IndirectLaborRate { get; set; }
        /// <summary>
        /// 间接折旧率
        /// </summary>
        [Column("INDIRECTDEPRECIATIONRATE", TypeName = "decimal(18,4)")]
        public decimal IndirectDepreciationRate { get; set; }
        /// <summary>
        /// 间接制造率
        /// </summary>
        [Column("INDIRECTMANUFACTURINGRATE", TypeName = "decimal(18,4)")]
        public decimal IndirectManufacturingRate { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        [Column("YEAR")]
        public int Year { get; set; }

    }
}
