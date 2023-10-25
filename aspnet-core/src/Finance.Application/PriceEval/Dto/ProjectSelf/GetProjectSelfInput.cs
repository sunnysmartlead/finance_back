using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace Finance.PriceEval.Dto.ProjectSelf
{
    public class GetProjectSelfInput: PagedResultRequestDto
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public virtual string Filter { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        public virtual string Code { get; set; }
    }
}
