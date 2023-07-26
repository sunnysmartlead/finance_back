using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("PHEnteritem")]
	public class ProcessHoursEnteritem : FullAuditedEntity<long>
    {

		/// <summary>
		/// 标准人工工时
		/// </summary>
		[Column("labor_hour")]
		public decimal? LaborHour { get; set; }

		/// <summary>
		/// 标准机器工时
		/// </summary>
		[Column("machine_hour")]
		public decimal? MachineHour { get; set; }

		/// <summary>
		/// 人员数量
		/// </summary>
		[Column("personnel_number")]
		public decimal? PersonnelNumber { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("process_hours_enter_id")]
		public decimal? ProcessHoursEnterId { get; set; }

		/// <summary>
		/// 年
		/// </summary>
		[Column("year")]
		[StringLength(255, ErrorMessage = "年长度不能超出255字符")]
		public string Year { get; set; }


	}
}
