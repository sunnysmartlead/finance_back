using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterLineDtoList: EntityDto<long>
    {
        public System.Nullable<System.Decimal> Xtsl { get; set; }
        public System.Nullable<System.Decimal> Gxftl { get; set; }
        public string Year { get; set; }
        public long ModelCountYearId { get; set; }
    }
}