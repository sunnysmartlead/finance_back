using Finance.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Dto
{
    /// <summary>
    /// 获取字典列表方法的参数输入
    /// </summary>
    public class GetFinanceDictionaryListInput : PagedInputDto
    {
        /// <summary>
        /// 字典名，即字典Id  获取字段列表的依据（包含输入值的都会被查询到，此值为空，则查询全部）
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// 字典显示名（包含输入值的都会被查询到，此值为空，则查询全部）
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// 备注（包含输入值的都会被查询到，此值为空，则查询全部）
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
