﻿using Abp.Application.Services.Dto;
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

        /// 软硬件总价

        public decimal HardwareDeviceTotalPrice { get; set; }


        public List<ProcessHoursEnterFrockDto> HardwareInfo { get; set; }

        //硬件总价
        public decimal HardwareTotalPrice { get; set; }

        /// <summary>
        /// 软件单价
        /// </summary>

        public decimal SoftwarePrice { get; set; }

    }


    }