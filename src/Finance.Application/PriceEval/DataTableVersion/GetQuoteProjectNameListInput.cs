using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.DataTableVersion
{
    /// <summary>
    /// 获取核报价项目名称列表 参数输入
    /// </summary>
    public class GetQuoteProjectNameListInput : PagedResultRequestDto
    {
        /// <summary>
        /// 核报价项目名称  筛选用  包含此字段输入的 核报价项目名称 都会返回  如果为空，则返回所有
        /// </summary>
        public string QuoteProjectName { get; set; }
    }
}
