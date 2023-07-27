using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库---治具检具库界面明细
    /// </summary>
    [Table("FFiItem")]
	public class FoundationFixtureItem : FullAuditedEntity<long>
    {

		/// <summary>
		/// 治具供应商
		/// </summary>
		[Column("fixture_business")]
		[StringLength(255, ErrorMessage = "治具供应商长度不能超出255字符")]
		public string FixtureBusiness { get; set; }

		/// <summary>
		/// 治具名称
		/// </summary>
		[Column("fixture_name")]
		[StringLength(255, ErrorMessage = "治具名称长度不能超出255字符")]
		public string FixtureName { get; set; }

		/// <summary>
		/// 治具单价
		/// </summary>
		[Column("fixture_price")]
		public decimal? FixturePrice { get; set; }

		/// <summary>
		/// 治具状态
		/// </summary>
		[Column("fixture_state")]
		[StringLength(255, ErrorMessage = "治具状态长度不能超出255字符")]
		public string FixtureState { get; set; }

		/// <summary>
		/// 治具检具库id
		/// </summary>
		[Column("foundation_fixture_id")]
		public long? FoundationFixtureId { get; set; }
	}
}
