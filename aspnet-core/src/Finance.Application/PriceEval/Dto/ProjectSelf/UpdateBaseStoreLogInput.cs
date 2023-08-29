using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.ProjectSelf
{
    /// <summary>
    /// 修改日志
    /// </summary>
    public class UpdateBaseStoreLogInput:EntityDto<long>
    {
        /// <summary>
        /// 文本说明
        /// </summary>
        public virtual string Text { get; set; }
    }
}
