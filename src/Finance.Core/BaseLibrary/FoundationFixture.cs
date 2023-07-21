using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--治具检具库界面
    /// </summary>
    [Table("FoundationFixture")]
	public class FoundationFixture : FullAuditedEntity<long>
    {

		/// <summary>
		/// 检具供应商
		/// </summary>
		[Column("fixture_gauge_business")]
		[StringLength(255, ErrorMessage = "检具供应商长度不能超出255字符")]
		public string FixtureGaugeBusiness { get; set; }

		/// <summary>
		/// 检具名称
		/// </summary>
		[Column("fixture_gauge_name")]
		[StringLength(255, ErrorMessage = "检具名称长度不能超出255字符")]
		public string FixtureGaugeName { get; set; }

		/// <summary>
		/// 检具单价
		/// </summary>
		[Column("fixture_gauge_price")]
		[StringLength(255, ErrorMessage = "检具单价长度不能超出255字符")]
		public string FixtureGaugePrice { get; set; }

		/// <summary>
		/// 检具状态
		/// </summary>
		[Column("fixture_gauge_state")]
		[StringLength(255, ErrorMessage = "检具状态长度不能超出255字符")]
		public string FixtureGaugeState { get; set; }


		/// <summary>
		/// 工序名称
		/// </summary>
		[Column("process_name")]
		[StringLength(255, ErrorMessage = "工序名称长度不能超出255字符")]
		public string ProcessName { get; set; }

		/// <summary>
		/// 工序编号
		/// </summary>
		[Column("process_number")]
		[StringLength(255, ErrorMessage = "工序编号长度不能超出255字符")]
		public string ProcessNumber { get; set; }


	}
}
