using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("PHEnterFixture")]
	public class ProcessHoursEnterFixture : FullAuditedEntity<long>
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
		/// 工时工序导入id
		/// </summary>
		[Column("process_hours_enter_id")]
		public long? ProcessHoursEnterId { get; set; }


	}
}
