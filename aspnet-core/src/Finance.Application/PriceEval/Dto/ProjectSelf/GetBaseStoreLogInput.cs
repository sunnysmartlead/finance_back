using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace Finance.PriceEval.Dto.ProjectSelf
{
    /// <summary>
    /// 获取基础库修改日志
    /// </summary>
    public class GetBaseStoreLogInput: PagedResultRequestDto
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Filter { get; set; }
    }
}
