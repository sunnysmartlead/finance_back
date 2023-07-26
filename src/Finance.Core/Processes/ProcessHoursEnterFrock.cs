using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("PHEnterFrock")]
	public class ProcessHoursEnterFrock : FullAuditedEntity<long>
    {

		/// <summary>
		/// 硬件设备名称
		/// </summary>
		[Column("hardware_device_name")]
		[StringLength(255, ErrorMessage = "硬件设备名称长度不能超出255字符")]
		public string HardwareDeviceName { get; set; }

		/// <summary>
		/// 硬件设备数量
		/// </summary>
		[Column("hardware_device_number")]
		public decimal? HardwareDeviceNumber { get; set; }

		/// <summary>
		/// 硬件设备单价
		/// </summary>
		[Column("hardware_device_price")]
		public decimal? HardwareDevicePrice { get; set; }

		/// <summary>
		/// 工时工序导入id
		/// </summary>
		[Column("process_hours_enter_id")]
		public decimal? ProcessHoursEnterId { get; set; }

	}
}
