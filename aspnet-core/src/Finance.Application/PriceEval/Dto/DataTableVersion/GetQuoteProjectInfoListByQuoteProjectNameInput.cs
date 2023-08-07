using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.DataTableVersion
{
    /// <summary>
    /// GetQuoteProjectInfoListByQuoteProjectName 方法的参数输入
    /// </summary>
    public class GetQuoteProjectInfoListByQuoteProjectNameInput : PagedResultRequestDto
    {
        /// <summary>
        /// 核报价项目名称
        /// </summary>
        public virtual string QuoteProjectName { get; set; }
    }
}
