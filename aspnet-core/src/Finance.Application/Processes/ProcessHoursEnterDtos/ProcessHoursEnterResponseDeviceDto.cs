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
        //设备总价
        public decimal DeviceTotalCost { get; set; }
      
        //设备信息
        public List<ProcessHoursEnterDeviceDto> DeviceArr { get; set; } 

   
    }
}