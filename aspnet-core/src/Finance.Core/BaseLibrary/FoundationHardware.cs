using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 基础库--硬件库
    /// </summary>
    [Table("FH")]
	public class FoundationHardware : FullAuditedEntity<long>
    {

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

		/// <summary>
		/// 软件名称
		/// </summary>
		[Column("software_name")]
		[StringLength(255, ErrorMessage = "软件名称长度不能超出255字符")]
		public string SoftwareName { get; set; }

		/// <summary>
		/// 软件单价
		/// </summary>
		[Column("software_price")]
		[StringLength(255, ErrorMessage = "软件单价长度不能超出255字符")]
		public decimal? SoftwarePrice { get; set; }

		/// <summary>
		/// 软件状态
		/// </summary>
		[Column("software_state")]
		[StringLength(255, ErrorMessage = "软件状态长度不能超出255字符")]
		public string SoftwareState { get; set; }

        /// <summary>
        /// 追溯软件
        /// </summary>
        [Column("traceability_software")]
        public string TraceabilitySoftware { get; set; }

        /// <summary>
        /// 追溯软件费用
        /// </summary>
        [Column("traceability_software_cost")]
        public decimal TraceabilitySoftwareCost { get; set; }

        /// <summary>
        /// 软件供应商
        /// </summary>
        [Column("software_supplier")]
        public string SoftwareSupplier { get; set; }

    }
}
