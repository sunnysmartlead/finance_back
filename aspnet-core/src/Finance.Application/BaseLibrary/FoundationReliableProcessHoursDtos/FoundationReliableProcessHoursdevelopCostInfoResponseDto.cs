using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationReliableProcessHoursdevelopCostInfoResponseDto : EntityDto<long>
    {
        public decimal? SoftwareHardPrice { get; set; }
        public decimal? PictureDevelopment { get; set; }
        public string DrawingSoftware { get; set; }
        public string TraceabilitySoftware { get; set; }
        public decimal? TotalHardwarePrice { get; set; }
        public decimal? Development { get; set; }

        public List<FoundationTechnologyFrockDto> HardwareInfo { get; set; }    




    }
}