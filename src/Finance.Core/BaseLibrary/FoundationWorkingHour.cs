using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--工时库
    /// </summary>
    [Table("FoundationWorkingHour")]
	public class FoundationWorkingHour : FullAuditedEntity<long>
    {

		/// <summary>
		/// 工时名称
		/// </summary>
		[Column("process_name")]
		[StringLength(255, ErrorMessage = "工时名称长度不能超出255字符")]
		public string ProcessName { get; set; }

		/// <summary>
		/// 工时编号
		/// </summary>
		[Column("process_number")]
		[StringLength(255, ErrorMessage = "工时编号长度不能超出255字符")]
		public string ProcessNumber { get; set; }


	}
}
