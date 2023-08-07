using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Infrastructure
{
    /// <summary>
    /// 财务字典表
    /// </summary>
    [Table("Fd_FinanceDictionary")]
    public class FinanceDictionary : FullAuditedEntity<string>
    {
        /// <summary>
        /// 字典显示名
        /// </summary>
        [Required]
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }

    }

    /// <summary>
    /// 财务字典表明细
    /// </summary>
    [Table("Fd_FinanceDictionaryDetail")]
    public class FinanceDictionaryDetail : FullAuditedEntity<string>
    {
        /// <summary>
        /// 财务字典表主键
        /// </summary>
        [Required]
        public virtual string FinanceDictionaryId { get; set; }

        /// <summary>
        /// 排序依据
        /// </summary>
        public virtual long Order { get; set; }

        /// <summary>
        /// 字典明细显示名
        /// </summary>
        [Required]
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
