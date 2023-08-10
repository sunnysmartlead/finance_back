using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.FinanceParameter
{
    /// <summary>
    /// 质量成本比例录入
    /// </summary>
    [Table("QUALITYCOSTINFO")]
    public class QualityRatioEntryInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 类别
        /// </summary>
        [Column("CATEGORY")]
        public string Category { get; set; }
        /// <summary>
        /// 是否首款
        /// </summary>
        [Column("ISFIRST")]
        public bool IsFirst { get; set; }
        
    }
    public class QualityRatioYearInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 主表质量成本比例录入QualityCostProportionEntryInfo的id
        /// </summary>
        [Required]
        [Column("QUALITYCOSTID")]
        public  long QualityCostId { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        [Column("YEAR")]
        public int Year { get; set; }
        /// <summary>
        /// 比例(数值中共有18位数，其中整数占14位，小数占4位。)
        /// </summary>
        [Column("RATE",TypeName = "decimal(18,4)")]
        public decimal Rate { get; set; }
    }
}
