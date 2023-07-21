using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--日志表
    /// </summary>
    [Table("FoundationLogs")]
	public class FoundationLogs : FullAuditedEntity<long>
    {

		/// <summary>
		/// 备注
		/// </summary>
		[Column("remark")]
		[StringLength(255, ErrorMessage = "备注长度不能超出255字符")]
		public string Remark { get; set; }

		/// <summary>
		/// 版本
		/// </summary>
		[Column("version")]
		[StringLength(255, ErrorMessage = "版本长度不能超出255字符")]
		public string Version { get; set; }


	}
}
