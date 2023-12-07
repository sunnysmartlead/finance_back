using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class GetWorkflowOveredInput : PagedResultRequestDto
    {
        /// <summary>
        /// 筛选流程名
        /// </summary>
        public virtual string Filter { get; set; }

        /// <summary>
        /// 流程ID
        /// </summary>
        public virtual long? AuditFlowId { get; set; }

    }
}
