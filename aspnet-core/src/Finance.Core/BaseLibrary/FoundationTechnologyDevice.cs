using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 基础库--设备信息
    /// </summary>
    [Table("FTDevice")]
	public class FoundationTechnologyDevice : FullAuditedEntity<long>
    {

		/// <summary>
		/// 设备名称
		/// </summary>
		[Column("device_name")]
		[StringLength(255, ErrorMessage = "设备名称长度不能超出255字符")]
		public string DeviceName { get; set; }

		/// <summary>
		/// 设备编号
		/// </summary>
		[Column("device_number")]
		[StringLength(255, ErrorMessage = "设备编号长度不能超出255字符")]
		public string DeviceNumber { get; set; }

		/// <summary>
		/// 设备单价
		/// </summary>
		[Column("device_price")]
		[StringLength(255, ErrorMessage = "设备单价长度不能超出255字符")]
		public string DevicePrice { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("device_sort")]
		[StringLength(255, ErrorMessage = "DeviceSort长度不能超出255字符")]
		public string DeviceSort { get; set; }

		/// <summary>
		/// 设备状态1
		/// </summary>
		[Column("device_status")]
		[StringLength(255, ErrorMessage = "设备状态1长度不能超出255字符")]
		public string DeviceStatus { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("foundation_reliable_hours_id")]
		public long? FoundationReliableHoursId { get; set; }

	}
}
