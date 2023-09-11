using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment
{
    public class FollowLineTangent : FullAuditedEntity<long>
    {

        /// <summary>
        /// 年份
        /// </summary>

        [Column("YEAR")]
        public int Year { get; set; }
        /// <summary>
        /// 人工工时
        /// </summary>

        [Column("LABORHOUR")]
        public double LaborHour { get; set; }
        /// <summary>
        /// 机器工时
        /// </summary>

        [Column("BOARDNAME")]
        public double MachineHour { get; set; }
        /// <summary>
        /// 人均跟线数量
        /// </summary>

        [Column("PERFOLLOWUPQUANTITY")]
        public double PerFollowUpQuantity { get; set; }

    }
}
