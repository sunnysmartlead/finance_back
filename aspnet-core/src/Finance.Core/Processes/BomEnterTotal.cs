using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    [Table("BETotal")]
	public class BomEnterTotal : FullAuditedEntity<long>
    {

		/// <summary>
		/// 
		/// </summary>
		[Column("audit_flow_id")]
		public long? AuditFlowId { get; set; }

		/// <summary>
		/// 分类
		/// </summary>
		[Column("classification")]
		[StringLength(255, ErrorMessage = "分类长度不能超出255字符")]
		public string Classification { get; set; }

		/// <summary>
		/// 
		/// </summary>
			[Column("solution_id")]
    	public long SolutionId { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[Column("remark")]
		[StringLength(255, ErrorMessage = "备注长度不能超出255字符")]
		public string Remark { get; set; }

		/// <summary>
		/// 总价
		/// </summary>
		[Column("total_cost")]
		public decimal? TotalCost { get; set; }

		/// <summary>
		/// 年
		/// </summary>
		[Column("year")]
		[StringLength(255, ErrorMessage = "年长度不能超出255字符")]
		public string Year { get; set; }

        /// <summary>
        ///  0 保存  1 提交
        /// </summary>
        [Column("status")]
        public long status { get; set; }

        /// <summary>
        /// 年份id
        /// </summary>
        [Column("model_count_year_Id")]

        public long ModelCountYearId { get; set; }
    }
}
