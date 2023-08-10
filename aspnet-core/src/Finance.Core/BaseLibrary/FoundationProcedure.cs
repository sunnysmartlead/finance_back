using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--工装库
    /// </summary>
    [Table("FProcedure")]
	public class FoundationProcedure : FullAuditedEntity<long>
    {

		/// <summary>
		/// 工装名称
		/// </summary>
		[Column("installation_name")]
		[StringLength(255, ErrorMessage = "工装名称长度不能超出255字符")]
		public string InstallationName { get; set; }

		/// <summary>
		/// 工装单价
		/// </summary>
		[Column("installation_price")]
		public decimal? InstallationPrice { get; set; }

		/// <summary>
		/// 工装供应商
		/// </summary>
		[Column("installation_supplier")]
		[StringLength(255, ErrorMessage = "工装供应商长度不能超出255字符")]
		public string InstallationSupplier { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("name")]
		[StringLength(255, ErrorMessage = "Name长度不能超出255字符")]
		public string Name { get; set; }

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
		[StringLength(255, ErrorMessage = "工序单价长度不能超出255字符")]
		public string ProcessNumber { get; set; }

		/// <summary>
		/// 测试线名称
		/// </summary>
		[Column("test_name")]
		[StringLength(255, ErrorMessage = "测试线名称长度不能超出255字符")]
		public string TestName { get; set; }

		/// <summary>
		/// 测试线单价
		/// </summary>
		[Column("test_price")]
		public decimal? TestPrice { get; set; }


	}
}
