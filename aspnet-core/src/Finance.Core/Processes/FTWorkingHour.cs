using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    [Table("FTWorkingHour")]
	public class FTWorkingHour : FullAuditedEntity<long>
    {

		/// <summary>
		/// 
		/// </summary>
		[Column("foundation_reliable_hours_id")]
		public decimal? FoundationReliableHoursId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("IsDeleted")]
		public decimal Isdeleted { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("labor_hour")]
		[StringLength(510, ErrorMessage = "LaborHour长度不能超出510字符")]
		public string LaborHour { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("machine_hour")]
		[StringLength(510, ErrorMessage = "MachineHour长度不能超出510字符")]
		public string MachineHour { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("number_personnel")]
		[StringLength(510, ErrorMessage = "NumberPersonnel长度不能超出510字符")]
		public string NumberPersonnel { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Column("year")]
		[StringLength(510, ErrorMessage = "Year长度不能超出510字符")]
		public string Year { get; set; }


	}
}
