using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 基础库--实验库环境
    /// </summary>
    [Table("FoundationReliable")]
	public class Foundationreliable : FullAuditedEntity<long>
    {

		/// <summary>
		/// 分类
		/// </summary>
		[Column("classification")]
		[StringLength(255, ErrorMessage = "分类长度不能超出255字符")]
		public string Classification { get; set; }


		/// <summary>
		/// 名称
		/// </summary>
		[Column("name")]
		[StringLength(255, ErrorMessage = "名称长度不能超出255字符")]
		public string Name { get; set; }

		/// <summary>
		/// 单价
		/// </summary>
		[Column("price")]
		[StringLength(255, ErrorMessage = "单价长度不能超出255字符")]
		public double? Price { get; set; }

		/// <summary>
		/// 单位
		/// </summary>
		[Column("unit")]
		[StringLength(255, ErrorMessage = "单位长度不能超出255字符")]
		public string Unit { get; set; }

        /// <summary>
        /// 实验室
        /// </summary>
        [Column("laboratory")]
        [StringLength(255, ErrorMessage = "单位长度不能超出255字符")]
        public string Laboratory { get; set; }

    }
}
