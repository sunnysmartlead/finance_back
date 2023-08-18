using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("PHEnter")]
	public class ProcessHoursEnter : FullAuditedEntity<long>
    {

		/// <summary>
		/// 流程id
		/// </summary>
		[Column("audit_flow_id")]
		public long? AuditFlowId { get; set; }

		/// <summary>
		/// 工装治具总价
		/// </summary>
		[Column("develop_total_price")]
		[StringLength(255, ErrorMessage = "工装治具总价长度不能超出255字符")]
		public string DevelopTotalPrice { get; set; }

		/// <summary>
		/// 设备总价
		/// </summary>
		[Column("device_total_price")]
		public decimal? DeviceTotalPrice { get; set; }

		/// <summary>
		/// 检具名称
		/// </summary>
		[Column("fixture_name")]
		[StringLength(255, ErrorMessage = "检具名称长度不能超出255字符")]
		public string FixtureName { get; set; }

		/// <summary>
		/// 检具数量
		/// </summary>
		[Column("fixture_number")]
		public decimal FixtureNumber { get; set; }

		/// <summary>
		/// 检具单价
		/// </summary>
		[Column("fixture_price")]
		public decimal FixturePrice { get; set; }

		/// <summary>
		/// 工装名称
		/// </summary>
		[Column("frock_name")]
		[StringLength(255, ErrorMessage = "工装名称长度不能超出255字符")]
		public string FrockName { get; set; }

		/// <summary>
		/// 工装数量
		/// </summary>
		[Column("frock_number")]
		public decimal FrockNumber { get; set; }

		/// <summary>
		/// 工装单价
		/// </summary>
		[Column("frock_price")]
		public decimal FrockPrice { get; set; }

		/// <summary>
		/// 软硬件总价
		/// </summary>
		[Column("hardware_device_total_price")]
		public decimal HardwareDeviceTotalPrice { get; set; }

		/// <summary>
		/// 硬件总价
		/// </summary>
		[Column("hardware_total_price")]
		public decimal HardwareTotalPrice { get; set; }

		/// <summary>
		/// 开图软件
		/// </summary>
		[Column("open_drawing_software")]
		[StringLength(255, ErrorMessage = "开图软件长度不能超出255字符")]
		public string OpenDrawingSoftware { get; set; }

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
		/// 零件id
		/// </summary>
		[Column("product_id")]
		public long? ProductId { get; set; }

		/// <summary>
		/// 软件单价
		/// </summary>
		[Column("software_price")]
		public decimal? SoftwarePrice { get; set; }

		/// <summary>
		/// 测试线名称
		/// </summary>
		[Column("test_line_name")]
		[StringLength(255, ErrorMessage = "测试线名称长度不能超出255字符")]
		public string TestLineName { get; set; }

		/// <summary>
		/// 测试线数量
		/// </summary>
		[Column("test_line_number")]
		public decimal TestLineNumber { get; set; }

		/// <summary>
		/// 测试线单价
		/// </summary>
		[Column("test_line_price")]
		public decimal TestLinePrice { get; set; }


	}
}
