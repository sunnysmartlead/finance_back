using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--工序工时工装治置
    /// </summary>
    [Table("FoundationTechnologyFixture")]
	public class FoundationTechnologyFixture : FullAuditedEntity<long>
    {

		/// <summary>
		/// 名称
		/// </summary>
		[Column("fixture_name")]
		[StringLength(255, ErrorMessage = "名称长度不能超出255字符")]
		public string FixtureName { get; set; }

		/// <summary>
		/// 数量
		/// </summary>
		[Column("fixture_number")]
		public decimal? FixtureNumber { get; set; }

		/// <summary>
		/// 单价
		/// </summary>
		[Column("fixture_price")]
		public decimal? FixturePrice { get; set; }

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
	}
}
