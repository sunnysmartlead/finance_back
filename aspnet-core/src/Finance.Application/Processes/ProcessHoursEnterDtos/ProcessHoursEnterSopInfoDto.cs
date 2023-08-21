using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterSopInfoDto : EntityDto<long>
    {





   


        public List<ProcessHoursEnteritemDto> Issues { get; set; } = new List<ProcessHoursEnteritemDto>();

        /// <summary>
        /// 年
        /// </summary>
        public string Year { get; set; }




    }
}