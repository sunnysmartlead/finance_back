using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("BE")]
	public class BomEnter : FullAuditedEntity<long>
    {

		/// <summary>
		/// 
		/// </summary>
		[Column("audit_flow_id")]
		public decimal? AuditFlowId { get; set; }

		/// <summary>
		/// 分类
		/// </summary>
		[Column("classification")]
		[StringLength(255, ErrorMessage = "分类长度不能超出255字符")]
		public string Classification { get; set; }

		/// <summary>
		/// 直接制造成本-设备折损
		/// </summary>
		[Column("direct_depreciation")]
		public decimal? DirectDepreciation { get; set; }

		/// <summary>
		/// 直接制造成本-直接人工
		/// </summary>
		[Column("direct_labor_price")]
		public decimal? DirectLaborPrice { get; set; }

		/// <summary>
		/// 直接制造成本-换线成本
		/// </summary>
		[Column("direct_line_change_cost")]
		public decimal? DirectLineChangeCost { get; set; }

		/// <summary>
		/// 直接制造成本-制造成本
		/// </summary>
		[Column("direct_manufacturing_costs")]
		public decimal? DirectManufacturingCosts { get; set; }

		/// <summary>
		/// 直接制造成本-小计
		/// </summary>
		[Column("direct_summary")]
		public decimal? DirectSummary { get; set; }

		/// <summary>
		/// 间接制造成本-设备折损
		/// </summary>
		[Column("indirect_depreciation")]
		public decimal? IndirectDepreciation { get; set; }

		/// <summary>
		/// 间接制造成本-直接人工
		/// </summary>
		[Column("indirect_labor_price")]
		public decimal? IndirectLaborPrice { get; set; }

		/// <summary>
		/// 间接制造成本-制造成本
		/// </summary>
		[Column("indirect_manufacturing_costs")]
		public decimal? IndirectManufacturingCosts { get; set; }

		/// <summary>
		/// 间接制造成本-小计
		/// </summary>
		[Column("indirect_summary")]
		public decimal? IndirectSummary { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("product_id")]
		public decimal? ProductId { get; set; }

		/// <summary>
		/// 备注·
		/// </summary>
		[Column("remark")]
		[StringLength(255, ErrorMessage = "备注·长度不能超出255字符")]
		public string Remark { get; set; }

		/// <summary>
		/// 合集成本
		/// </summary>
		[Column("total_cost")]
		public decimal? TotalCost { get; set; }

		/// <summary>
		/// 年份
		/// </summary>
		[Column("year")]
		[StringLength(255, ErrorMessage = "年份长度不能超出255字符")]
		public string Year { get; set; }


	}
}
