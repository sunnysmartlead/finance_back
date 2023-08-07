using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Finance.Infrastructure.Dto
{
    /// <summary>
    /// 编辑字典明细
    /// </summary>
    public class EditFinanceDictionaryDetailInput : EntityDto<string>
    {
        /// <summary>
        /// 排序依据（以此字段倒序排序）
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
