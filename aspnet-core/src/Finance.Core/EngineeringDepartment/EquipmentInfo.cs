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
    /// 设备部分表
    /// </summary>
    [Table("EQUIPMENTINFO")]
    public class EquipmentInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 关联>工时工序静态字段表WORKINGHOURSINFO的主键ID
        /// </summary>
        [Required]
        [Column("WORKHOURSID")]
        public long WorkHoursId { get; set; }
        /// <summary>
        /// 表示所属工时工序表部分
        /// </summary>
        [Column("PART")]
        public Part Part { get; set; }
        /// <summary>
        /// 设备名
        /// </summary>
        [Column("EQUIPMENTNAME")]
        public string EquipmentName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Column("STATUS")]
        public string Status { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [Column("NUMBER")]
        public int Number { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        [Column("UNITPRICE", TypeName = "decimal(18,4)")]
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// 表示所属工时工序表部分
    /// </summary>
    public enum Part : byte
    {
        /// <summary>
        ///设备部分
        /// </summary>
        Equipment,

        /// <summary>
        /// 追溯部分
        /// </summary>
        Trace,
        /// <summary>
        /// 治具部分
        /// </summary>
        Fixture
        
    }

}
