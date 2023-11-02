using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    /// <summary>
    /// 获取任务重置列表
    /// </summary>
    public class GetResetListInput : PagedResultRequestDto
    {
        /// <summary>
        /// 工作流节点名称
        /// </summary>
        public virtual string NodeName { get; set; }

        /// <summary>
        /// 重置用户
        /// </summary>
        public virtual string ResetUser { get; set; }

        /// <summary>
        /// 目标用户
        /// </summary>
        public virtual string TargetUser { get; set; }
    }
}
