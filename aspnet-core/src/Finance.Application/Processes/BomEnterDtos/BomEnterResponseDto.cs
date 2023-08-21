using Abp.Application.Services.Dto;
using Finance.BaseLibrary;
using System;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class BomEnterResponseDto : EntityDto<long>
    {
        public string ProcessName { get; set; }
        public string ProcessNumber { get; set; }



        public BomEnterDeviceInfoResponseDto DeviceInfo { get; set; }
        public BomEnterDevelopCostInfoResponseDto DevelopCostInfo { get; set; }
        public BomEnterToolInfoResponseDto ToolInfo { get; set; }

        public List<ProcessHoursEnteritem> SopInfo { get; set; }

        public List<ProcessHoursEnterUphDto> ListProcessHoursEnterUph { get; set; }
        public List<ProcessHoursEnterLineDto> ListProcessHoursEnterLineDto { get; set; }


    }
}