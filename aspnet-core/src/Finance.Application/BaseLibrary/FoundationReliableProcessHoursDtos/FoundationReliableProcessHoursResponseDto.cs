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

        public FoundationReliableProcessHoursDeviceResponseDto DeviceInfo { get; set; } = new FoundationReliableProcessHoursDeviceResponseDto();
        public FoundationReliableProcessHoursdevelopCostInfoResponseDto DevelopCostInfo { get; set; } = new FoundationReliableProcessHoursdevelopCostInfoResponseDto();
        public FoundationReliableProcessHoursFixtureResponseDto toolInfo { get; set; } = new FoundationReliableProcessHoursFixtureResponseDto(); 

        public List<FoundationWorkingHourItemDto> sopInfo { get; set; } =new List<FoundationWorkingHourItemDto>(); 

        public static implicit operator FoundationReliableProcessHoursResponseDto(List<FoundationReliableProcessHoursResponseDto> v)
        {
            throw new NotImplementedException();
        }
    }
}