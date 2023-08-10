using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Dto
{
    /// <summary>
    /// 添加字典明细参数输入
    /// </summary>
    public class AddFinanceDictionaryDetailInput
    {
        /// <summary>
        /// 财务字典表主键（即字典的Id、字典的Name，现在Id和Name是一码事）（不允许包含【-】）
        /// </summary>
        [Required]
        [RegularExpression(FinanceConsts.FinanceDictionaryNameRegular)]
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
