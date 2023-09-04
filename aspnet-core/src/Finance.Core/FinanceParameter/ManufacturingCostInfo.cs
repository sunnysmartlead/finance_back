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
    /// 制造成本里计算字段参数维护
    /// </summary>
    [Table("MANUFACTURINGCOSTINFO")]
    public class ManufacturingCostInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 月工作天数
        /// </summary>
        [Column("MONTHLYWORKINGDAYS")]
        public double MonthlyWorkingDays { get; set; }
        /// <summary>
        /// 人员平均工资
        /// </summary>
        [Column("AVERAGEWAGE", TypeName = "decimal(18,4)")]
        public decimal AverageWage { get; set; }
        /// <summary>
        /// 每班正常工作时间
        /// </summary>
        [Column("WORKINGHOURS")]
        public double WorkingHours { get; set; }
        /// <summary>
        /// 稼动率
        /// </summary>
        [Column("RATEOFMOBILIZATION", TypeName = "decimal(18,4)")]
        public decimal RateOfMobilization { get; set; }
        /// <summary>
        /// 折旧年限
        /// </summary>
        [Column("USEFULLIFEOFFIXEDASSETS")]
        public double UsefulLifeOfFixedAssets { get; set; }
        /// <summary>
        /// 每日班次
        /// </summary>
        [Column("DAILYSHIFT")]
        public double DailyShift { get; set; }
        /// <summary>
        /// 增值税率
        /// </summary>
        [Column("VATRATE", TypeName = "decimal(18,4)")]
        public decimal VatRate { get; set; }
        /// <summary>
        /// 人均跟线数量
        /// </summary>
        [Column("TRACELINEOFPERSON")]
        public double TraceLineOfPerson { get; set; }
        /// <summary>
        /// GP12工序成本(元/pcs)
        /// </summary>
        [Column("PROCESSCOST", TypeName = "decimal(18,4)")]
        public decimal ProcessCost { get; set; }
        /// <summary>
        /// 产能利用率
        /// </summary>
        [Column("CAPACITYUTILIZATIONRATE", TypeName = "decimal(18,4)")]
        public decimal CapacityUtilizationRate { get; set; }
        /// <summary>
        /// 年
        /// </summary>
        [Column("YEAR")]
        public int Year { get; set; }
    }
}
