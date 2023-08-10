using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationReliableProcessHoursFixtureResponseDto : EntityDto<long>
    {
        public decimal? HardwareDeviceTotalPrice { get; set; }
        public decimal? SoftwarePrice { get; set; }
        public decimal? TestLineNumber { get; set; }
        public string TestLineName { get; set; }
        public decimal TestLinePrice { get; set; }
        public string FrockName { get; set; }
        public decimal FrockPrice { get; set; }
        public decimal? FrockNumber { get; set; }
        public string FixtureName { get; set; }
        public decimal FixtureNumber { get; set; }
        public decimal FixturePrice { get; set; }

        public List<FoundationTechnologyFixtureDto> zhiJuArr { get; set; }        




    }
}