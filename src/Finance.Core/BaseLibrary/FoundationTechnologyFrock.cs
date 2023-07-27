using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 基础库--工序工时硬件设备
    /// </summary>
    [Table("FTFrock")]
	public class FoundationTechnologyFrock : FullAuditedEntity<long>
    {

		/// <summary>
		/// 工时工序id
		/// </summary>
		[Column("foundation_reliable_hours_id")]
		public long? FoundationReliableHoursId { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[Column("hardware_device_name")]
		[StringLength(255, ErrorMessage = "名称长度不能超出255字符")]
		public string HardwareDeviceName { get; set; }

		/// <summary>
		/// 数量
		/// </summary>
		[Column("hardware_device_number")]
		public decimal? HardwareDeviceNumber { get; set; }

		/// <summary>
		/// 单价
		/// </summary>
		[Column("hardware_device_price")]
		public decimal? HardwareDevicePrice { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("process_name")]
		[StringLength(255, ErrorMessage = "ProcessName长度不能超出255字符")]
		public string ProcessName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("process_number")]
		[StringLength(255, ErrorMessage = "ProcessNumber长度不能超出255字符")]
		public string ProcessNumber { get; set; }

	}
}
