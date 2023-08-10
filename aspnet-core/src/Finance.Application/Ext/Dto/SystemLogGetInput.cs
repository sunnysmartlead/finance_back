using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext.Dto
{
    public class SystemLogGetInput : PagedResultRequestDto
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreationTime { get; set; }

        /// <summary>
        /// 请求Id
        /// </summary>
        public virtual string RequestId { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        public virtual bool? IsException { get; set; }
    }
}
