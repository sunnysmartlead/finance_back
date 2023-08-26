using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterListDto : EntityDto<long>
    {

        public bool IsDeleted { get; set; }
        public bool IsCOB { get; set; }
       
        public long AuditFlowId { get; set; }
        public long SolutionId { get; set; }
        //设备

        public List<ProcessHoursEnterListItemDto> ListItemDtos { get; set; }


        public List<ProcessHoursEnterUphListDto> ProcessHoursEnterUphList { get; set; } = new List<ProcessHoursEnterUphListDto>();
        public List<ProcessHoursEnterLineDtoList> ProcessHoursEnterLineList { get; set; } = new List<ProcessHoursEnterLineDtoList>();
    }
}