using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment
{
    /// <summary>
    /// 工时工序部分年份表
    /// </summary>
    [Table("YEARINFO")]
    public class YearInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 关联>工时工序静态字段表WORKINGHOURSINFO的主键ID
        /// </summary>
        [Column("WORKHOURSID")]
        public long WorkHoursId { get; set; }

        /// <summary>
        /// 流程表ID
        /// </summary>
        [Required]
        [Column("AUDITFLOWID")]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// MoudelCount表id
        /// </summary>
        [Required]
        [Column("PRODUCTID")]
        public long ProductId { get; set; }
        /// <summary>
        /// 年份记录区分
        /// </summary>
        [Required]
        [Column("PART")]
        public YearPart Part { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        [Column("YEAR")]
        public int Year { get; set; }
        /// <summary>
        /// 标准人工工时
        /// </summary>
        [Column("STANDARDLABORHOURS")]
        public double StandardLaborHours { get; set; }
        /// <summary>
        /// 标准机器工时
        /// </summary>
        [Column("STANDARDMACHINEHOURS")]
        public double StandardMachineHours { get; set; }
        /// <summary>
        /// 人员数量
        /// </summary>
        [Column("PERSONCOUNT")]
        public double PersonCount { get; set; }

    }

    /// <summary>
    /// 表示所属工时工序表部分
    /// </summary>
    public enum YearPart : byte
    {
        /// <summary>
        ///工时工序部分
        /// </summary>
        WorkingHour,

        /// <summary>
        /// 切线部分
        /// </summary>
        SwitchLine,
        

    }

}
