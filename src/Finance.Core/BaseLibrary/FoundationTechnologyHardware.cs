using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--工时工序追溯部分(硬件及软件开发费用)
    /// </summary>
    [Table("FoundationTechnologyHardware")]
	public class FoundationTechnologyHardware : FullAuditedEntity<long>
    {

		/// <summary>
		/// 
		/// </summary>
		[Column("fixture_sort")]
		[StringLength(255, ErrorMessage = "FixtureSort长度不能超出255字符")]
		public string FixtureSort { get; set; }

		/// <summary>
		/// 工序工时id
		/// </summary>
		[Column("foundation_reliable_hours_id")]
		public decimal? FoundationReliableHoursId { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[Column("hardware_name")]
		[StringLength(255, ErrorMessage = "名称长度不能超出255字符")]
		public string HardwareName { get; set; }

		/// <summary>
		/// 数量
		/// </summary>
		[Column("hardware_number")]
		public decimal? HardwareNumber { get; set; }

		/// <summary>
		/// 单价
		/// </summary>
		[Column("hardware_price")]
		public decimal? HardwarePrice { get; set; }
	}
}
