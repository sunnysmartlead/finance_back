using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationReliableProcessHoursResponseDto : EntityDto<long>
    {

        public string ProcessName { get; set; }
        public string ProcessNumber { get; set; }

        public FoundationReliableProcessHoursDeviceResponseDto DeviceInfo { get; set; }
        public FoundationReliableProcessHoursdevelopCostInfoResponseDto DevelopCostInfo { get; set; }
        public FoundationReliableProcessHoursFixtureResponseDto toolInfo { get; set; }

        public List<FoundationWorkingHourItemDto> sopInfo { get; set; }

        public static implicit operator FoundationReliableProcessHoursResponseDto(List<FoundationReliableProcessHoursResponseDto> v)
        {
            throw new NotImplementedException();
        }
    }
}