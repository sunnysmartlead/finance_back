using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库---设备明细
    /// </summary>
    [Table("FDeviceItem")]
	public class FoundationDeviceItem : FullAuditedEntity<long>
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
		/// 排序
		/// </summary>
		[Column("device_sort")]
		public decimal? DeviceSort { get; set; }

		/// <summary>
		/// 设备状态
		/// </summary>
		[Column("device_status")]
		[StringLength(255, ErrorMessage = "设备状态长度不能超出255字符")]
		public string DeviceStatus { get; set; }

		/// <summary>
		/// 设备库id
		/// </summary>
		[Column("process_hours_enter_id")]
		public long? ProcessHoursEnterId { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [Column("DeviceProvider")]
        [StringLength(255, ErrorMessage = "供应商长度不能超出255字符")]
        public string DeviceProvider { get; set; }


    }
}
