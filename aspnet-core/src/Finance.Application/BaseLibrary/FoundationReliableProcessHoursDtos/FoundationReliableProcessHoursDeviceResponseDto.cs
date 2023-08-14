using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationReliableProcessHoursDeviceResponseDto : EntityDto<long>
    {
        public System.Nullable<System.Decimal> DeviceTotalPrice { get; set; }

        public List<FoundationTechnologyDeviceDto> DeviceArr { get; set; }    




    }
}