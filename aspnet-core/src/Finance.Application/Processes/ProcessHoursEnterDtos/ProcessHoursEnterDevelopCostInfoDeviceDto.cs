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
    public class ProcessHoursEnterDevelopCostInfoDeviceDto : EntityDto<long>
    {
        /// <summary>
        /// 开图软件
        /// </summary>
        public string OpenDrawingSoftware { get; set; }

        /// <summary>
        /// 软硬件总价
        /// </summary>
        public decimal HardwareDeviceTotalPrice { get; set; }

        /// <summary>
        /// 硬件设备列表
        /// </summary>
        public List<ProcessHoursEnterFrockDto> HardwareInfo { get; set; }

        /// <summary>
        /// 软硬件总价
        /// </summary>
        public decimal HardwareTotalPrice { get; set; }

        /// <summary>
        /// 开发费(开图)
        /// </summary>
        public decimal SoftwarePrice { get; set; }
        /// <summary>
        /// 硬件总价
        /// </summary>
        public decimal TotalHardwarePrice { get; set; }
        /// <summary>
        /// 无用字段
        /// </summary>
        public decimal PictureDevelopment { get; set; }
        /// <summary>
        /// 追溯软件费用
        /// </summary>
        public decimal Development { get; set; }
        /// <summary>
        /// 追溯软件
        /// </summary>
        public string TraceabilitySoftware { get; set; }
        /// <summary>
        /// 追溯软件费用
        /// </summary>
        public decimal TraceabilitySoftwareCost { get; set; }
    }


    }