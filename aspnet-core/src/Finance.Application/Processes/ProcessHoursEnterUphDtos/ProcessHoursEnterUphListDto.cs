using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterUphListDto: EntityDto<long>
    {

        public System.Nullable<System.Decimal> Cobuph { get; set; }
        public System.Nullable<System.Decimal> Smtuph { get; set; }
        public System.Nullable<System.Decimal> Zcuph { get; set; }
        public string Year { get; set; }
    }
}