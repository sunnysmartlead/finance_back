using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class InstanceHistoryListDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// 操作用户名
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// 工作流节点名称
        /// </summary>
        public virtual string NodeName { get; set; }

        /// <summary>
        /// 流转意见
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// 审批评论
        /// </summary>
        public virtual string Comment { get; set; }
    }
}
