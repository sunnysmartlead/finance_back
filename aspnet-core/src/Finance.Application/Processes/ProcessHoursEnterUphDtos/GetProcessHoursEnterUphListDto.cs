using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class GetProcessHoursEnterUphListDto : EntityDto<long>
    {

        public System.Nullable<System.Decimal> AuditFlowId { get; set; }
        public System.Nullable<System.Decimal> SolutionId { get; set; }
        public List<ProcessHoursEnterUphListDto> ProcessHoursEnterUphListDtos { get; set; }
    }
}