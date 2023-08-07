using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("PHEnterDevice")]
    public class ProcessHoursEnterDevice : FullAuditedEntity<long>
    {

        /// <summary>
        /// 设备名称
        /// </summary>
        [Column("device_name")]
        [StringLength(255, ErrorMessage = "设备名称长度不能超出255字符")]
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备数量
        /// </summary>
        [Column("device_number")]
        public decimal? DeviceNumber { get; set; }

        /// <summary>
        /// 设备单价
        /// </summary>
        [Column("device_price")]
        public decimal? DevicePrice { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        [Column("device_status")]
        [StringLength(255, ErrorMessage = "设备状态长度不能超出255字符")]
        public string DeviceStatus { get; set; }

        /// <summary>
        /// 工时工序导入id
        /// </summary>
        [Column("process_hours_enter_id")]
        public long? ProcessHoursEnterId { get; set; }


    }
}
