using Finance.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Dto
{
    /// <summary>
    /// 获取字典明细列表方法的参数输入
    /// </summary>
    public class GetFinanceDictionaryDetailListInput : PagedInputDto
    {
        /// <summary>
        /// 字典的Id，明细所属的字典
        /// </summary>
        [Required]
        public virtual string FinanceDictionaryId { get; set; }

        /// <summary>
        /// 字典明细显示名（包含输入值的都会被查询到，此值为空，则查询全部）
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// 备注（包含输入值的都会被查询到，此值为空，则查询全部）
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
