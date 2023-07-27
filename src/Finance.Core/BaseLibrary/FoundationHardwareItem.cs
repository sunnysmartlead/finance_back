using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--硬件库明细
    /// </summary>
    [Table("FHItem")]
	public class FoundationHardwareItem : FullAuditedEntity<long>
    {

		/// <summary>
		/// 硬件id
		/// </summary>
		[Column("foundation_hardware_id")]
		public long? FoundationHardwareId { get; set; }

		/// <summary>
		/// 供应商
		/// </summary>
		[Column("hardware_business")]
		[StringLength(255, ErrorMessage = "供应商长度不能超出255字符")]
		public string HardwareBusiness { get; set; }

		/// <summary>
		/// 硬件名称
		/// </summary>
		[Column("hardware_name")]
		[StringLength(255, ErrorMessage = "硬件名称长度不能超出255字符")]
		public string HardwareName { get; set; }

		/// <summary>
		/// 硬件价格
		/// </summary>
		[Column("hardware_price")]
		[StringLength(255, ErrorMessage = "硬件价格长度不能超出255字符")]
		public string HardwarePrice { get; set; }

		/// <summary>
		/// 硬件状态
		/// </summary>
		[Column("hardware_state")]
		[StringLength(255, ErrorMessage = "硬件状态长度不能超出255字符")]
		public string HardwareState { get; set; }

	}
}
