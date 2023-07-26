using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("PHEnterLine")]
	public class ProcessHoursEnterLine : FullAuditedEntity<long>
    {
		/// <summary>
		/// 流程id
		/// </summary>
		[Column("audit_flow_id")]
		public decimal? AuditFlowId { get; set; }

		/// <summary>
		/// 零件id
		/// </summary>
		[Column("product_id")]
		public decimal? ProductId { get; set; }

		/// <summary>
		/// UPH参数
		/// </summary>
		[Column("UPH")]
		[StringLength(255, ErrorMessage = "UPH参数长度不能超出255字符")]
		public string Uph { get; set; }

		/// <summary>
		/// 值
		/// </summary>
		[Column("value")]
		public decimal? Value { get; set; }

		/// <summary>
		/// 年份
		/// </summary>
		[Column("year")]
		[StringLength(255, ErrorMessage = "年份长度不能超出255字符")]
		public string Year { get; set; }


	}
}
