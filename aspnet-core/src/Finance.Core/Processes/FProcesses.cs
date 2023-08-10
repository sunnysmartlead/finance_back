using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    [Table("FProcesses")]
	public class FProcesses : FullAuditedEntity<long>
    {
		/// <summary>
		/// 
		/// </summary>
		[Column("process_name")]
		[StringLength(510, ErrorMessage = "ProcessName长度不能超出510字符")]
		public string ProcessName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("process_number")]
		[StringLength(510, ErrorMessage = "ProcessNumber长度不能超出510字符")]
		public string ProcessNumber { get; set; }


	}
}
