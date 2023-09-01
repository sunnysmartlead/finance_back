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
    public class ProcessHoursEnterToolInfoDto : EntityDto<long>
    {





   


        public List<ProcessHoursEnterFixtureDto> ZhiJuArr { get; set; }

        /// <summary>
        /// 检具名称
        /// </summary>
        public string FixtureName { get; set; }

        /// <summary>
        /// 检具数量
        /// </summary>
        public decimal FixtureNumber { get; set; }

        /// <summary>
        /// 检具单价
        /// </summary>
        public decimal FixturePrice { get; set; }

        /// <summary>
        /// 工装名称
        /// </summary>
        public string FrockName { get; set; }

        /// <summary>
        /// 工装数量
        /// </summary>
        public decimal FrockNumber { get; set; }

        /// <summary>
        /// 工装单价
        /// </summary>
        public decimal FrockPrice { get; set; }


        /// <summary>
		/// 测试线名称
		/// </summary>
        public string TestLineName { get; set; }

        /// <summary>
        /// 测试线数量
        /// </summary>
        public decimal TestLineNumber { get; set; }

        /// <summary>
        /// 测试线单价
        /// </summary>
        public decimal TestLinePrice { get; set; }

        public string DevelopTotalPrice { get; set; }

        public decimal SoftwarePrice { get; set; }

    }
}