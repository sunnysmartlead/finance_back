using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterUphBomDto : EntityDto<long>
    {
        public List<ProcessHoursEnterUphBomItemDto> ProcessHoursEnterUphBomItemsList { get; set; }

        public bool IsCOB { get; set; }
    }
}