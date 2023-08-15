using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
	/// <summary>
	/// 物流信息录入
	/// </summary>
    [Table("LogisticsCost")]
	public class Logisticscost : FullAuditedEntity<long>
    {

        /// <summary>
        /// 流程id
        /// </summary>
        [Column("audit_flow_id")]
		public long? AuditFlowId { get; set; }

		/// <summary>
		/// 分类
		/// </summary>
		[Column("classification")]
		[StringLength(255, ErrorMessage = "分类长度不能超出255字符")]
		public string Classification { get; set; }


		/// <summary>
		/// 运费/月
		/// </summary>
		[Column("freight_price")]
		public decimal? FreightPrice { get; set; }

		/// <summary>
		/// 月需求量
		/// </summary>
		[Column("monthly_demand_price")]
		public decimal? MonthlyDemandPrice { get; set; } 

		/// <summary>
		/// 单PCS包装价格/元
		/// </summary>
		[Column("packaging_price")]
		public decimal? PackagingPrice { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("product_id")]
		public long? ProductId { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[Column("remark")]
		[StringLength(255, ErrorMessage = "备注长度不能超出255字符")]
		public string Remark { get; set; }

		/// <summary>
		/// 单PCS运输费
		/// </summary>
		[Column("singly_demand_price")]
		public decimal? SinglyDemandPrice { get; set; }

		/// <summary>
		/// 仓储费用/月
		/// </summary>
		[Column("storage_price")]
		public decimal? StoragePrice { get; set; }

		/// <summary>
		/// 单PCS总物流成本
		/// </summary>
		[Column("transport_price")]
		public decimal? TransportPrice { get; set; }

		/// <summary>
		///  年份
		/// </summary>
		[Column("year")]
		[StringLength(255, ErrorMessage = " 年份长度不能超出255字符")]
		public string Year { get; set; }


	}
}
