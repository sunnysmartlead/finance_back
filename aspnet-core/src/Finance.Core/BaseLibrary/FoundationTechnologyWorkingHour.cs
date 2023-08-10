using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库标准工艺--工时库明细
    /// </summary>
    [Table("FTWorkingHour")]
	public class FoundationTechnologyWorkingHour : FullAuditedEntity<long>
    {

        /// <summary>
        /// 
        /// </summary>
        [Column("foundation_reliable_hours_id")]
        public long? FoundationReliableHoursId { get; set; }
        /// <summary>
        /// 标准人工工时
        /// </summary>
        [Column("labor_hour")]
		[StringLength(255, ErrorMessage = "标准人工工时长度不能超出255字符")]
		public string LaborHour { get; set; }

		/// <summary>
		/// 标准机器工时
		/// </summary>
		[Column("machine_hour")]
		[StringLength(255, ErrorMessage = "标准机器工时长度不能超出255字符")]
		public string MachineHour { get; set; }

		/// <summary>
		/// 人员数量
		/// </summary>
		[Column("number_personnel")]
		[StringLength(255, ErrorMessage = "人员数量长度不能超出255字符")]
		public string NumberPersonnel { get; set; }

		/// <summary>
		/// 年
		/// </summary>
		[Column("year")]
		[StringLength(255, ErrorMessage = "年长度不能超出255字符")]
		public string Year { get; set; }


	}
}
