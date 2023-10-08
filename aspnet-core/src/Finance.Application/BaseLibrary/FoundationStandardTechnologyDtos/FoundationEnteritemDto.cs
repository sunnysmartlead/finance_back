using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationEnteritemDto : EntityDto<long>
    {
        public string LaborHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MachineHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NumberPersonnel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Year { get; set; }
    }
}