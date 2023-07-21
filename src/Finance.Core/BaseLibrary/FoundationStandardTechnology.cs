using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{

    /// <summary>
    /// 基础库--标准工艺库
    /// </summary>
    [Table("FoundationStandardTechnology")]
	public class FoundationStandardTechnology : FullAuditedEntity<long>
    {

		/// <summary>
		/// 标准工艺名称
		/// </summary>
		[Column("name")]
		[StringLength(255, ErrorMessage = "标准工艺名称长度不能超出255字符")]
		public string Name { get; set; }


	}
}
