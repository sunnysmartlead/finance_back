using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("BPMaintenance")]
	public class BaseProcessMaintenance : FullAuditedEntity<long>
    {
		/// <summary>
		/// 
		/// </summary>
		[Column("process_name")]
		[StringLength(255, ErrorMessage = "ProcessName长度不能超出255字符")]
		public string ProcessName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("process_number")]
		[StringLength(255, ErrorMessage = "ProcessNumber长度不能超出255字符")]
		public string ProcessNumber { get; set; }


	}
}
