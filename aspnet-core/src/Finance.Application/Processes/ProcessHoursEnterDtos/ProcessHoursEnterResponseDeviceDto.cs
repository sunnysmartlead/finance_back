using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterResponseDeviceDto : EntityDto<long>
    {

        public decimal DeviceTotalCost { get; set; }
      

        public List<ProcessHoursEnterDeviceDto> DeviceArr { get; set; } 

   
    }
}