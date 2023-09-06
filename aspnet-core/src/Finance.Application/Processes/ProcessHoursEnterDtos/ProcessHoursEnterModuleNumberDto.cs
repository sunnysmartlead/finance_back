using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 项目走量
    /// </summary>
    public class ProcessHoursEnterModuleNumberDto : EntityDto<long>
    {


        public long? AuditFlowId { get; set; }
        public long? SolutionId { get; set; }


    }
}