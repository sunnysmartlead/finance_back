using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--工序库
    /// </summary>
    [Table("FProcesses")]
	public class FoundationProcesses : FullAuditedEntity<long>
    {

		/// <summary>
		/// 工序编号
		/// </summary>
		[Column("process_number")]
		[StringLength(255, ErrorMessage = "工序编号")]
		public string processNumber { get; set; }

		/// <summary>
		/// 工序名称
		/// </summary>
		[Column("process_name")]
        [StringLength(255, ErrorMessage = "工序名称")]
        public string processName { get; set; }

	


	}
}
