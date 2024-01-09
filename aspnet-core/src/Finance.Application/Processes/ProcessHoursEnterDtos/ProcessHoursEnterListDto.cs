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
        //是否是COB成本
        public bool IsCOB { get; set; }
      
        public long AuditFlowId { get; set; }
        public long SolutionId { get; set; }
        //工时工时列表
        public List<ProcessHoursEnterListItemDto> ListItemDtos { get; set; }
        //uph列表
        public List<ProcessHoursEnterUphListDto> ProcessHoursEnterUphList { get; set; } = new List<ProcessHoursEnterUphListDto>();
        //线体数量列表
        public List<ProcessHoursEnterLineDtoList> ProcessHoursEnterLineList { get; set; } = new List<ProcessHoursEnterLineDtoList>();
    }
}