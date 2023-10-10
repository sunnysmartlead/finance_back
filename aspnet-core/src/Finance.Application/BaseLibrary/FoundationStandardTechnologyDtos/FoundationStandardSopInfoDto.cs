using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationStandardSopInfoDto : EntityDto<long>
    {





   


        public List<FoundationEnteritemDto> Issues { get; set; } = new List<FoundationEnteritemDto>();

        /// <summary>
        /// 年
        /// </summary>
        public string Year { get; set; }
        public decimal ModelCountYearId { get; set; }




    }
}