using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class GetWorkflowInstanceInput : PagedResultRequestDto
    {
        /// <summary>
        /// 流程状态
        /// </summary>
        public virtual WorkflowState WorkflowState { get; set; }
    }
}
