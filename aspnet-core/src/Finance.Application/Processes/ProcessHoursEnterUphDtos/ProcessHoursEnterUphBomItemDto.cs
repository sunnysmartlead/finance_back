using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterUphBomItemDto : EntityDto<long>
    {

        public decimal? ComPuh { get; set; }
        public string Year { get; set; }
    }
}