using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterDto: EntityDto<long>
    {

        public string ProcessName { get; set; }
        public string ProcessNumber { get; set; }
        public long? AuditFlowId { get; set; }
        public long? SolutionId { get; set; }
        //设备

        public ProcessHoursEnterResponseDeviceDto DeviceInfo { get; set; } = new ProcessHoursEnterResponseDeviceDto();
        //追溯部分(硬件及软件开发费用)
        public ProcessHoursEnterDevelopCostInfoDeviceDto DevelopCostInfo { get; set; } = new ProcessHoursEnterDevelopCostInfoDeviceDto();
        //工装治具部分
        public ProcessHoursEnterToolInfoDto ToolInfo { get; set; } = new ProcessHoursEnterToolInfoDto();

        public List<ProcessHoursEnterSopInfoDto> SopInfo { get; set; } = new List<ProcessHoursEnterSopInfoDto>();
        public List<ProcessHoursEnteritemDto> SopInfoAll { get; set; } = new List<ProcessHoursEnteritemDto>();
    }
}