using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.Timelinesss
{
    public class GetTimelinessDto
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }
    }

    public class SetTimelinessDto
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public virtual List<NameValue> Data { get; set; }
    }

    public class TimelinessDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public virtual List<NameValue> Data { get; set; }
    }

    public class NameValue
    {
        public virtual string Name { get; set; }

        public virtual string Value { get; set; }

    }
}
