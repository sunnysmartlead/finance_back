using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Dto
{
    /// <summary>
    /// 编辑字典
    /// </summary>
    public class EditFinanceDictionaryInput : EntityDto<string>
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
}
