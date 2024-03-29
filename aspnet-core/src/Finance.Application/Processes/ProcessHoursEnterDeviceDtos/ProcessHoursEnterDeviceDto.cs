﻿using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 设备列表
    /// </summary>
    public class ProcessHoursEnterDeviceDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public System.Nullable<System.Decimal> DeviceNumber { get; set; }
        /// <summary>
        /// 设备单价
        /// </summary>
        public System.Nullable<System.Decimal> DevicePrice { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public string DeviceStatus { get; set; }
        /// <summary>
        /// 工时工序Id
        /// </summary>
        public System.Nullable<System.Decimal> ProcessHoursEnterId { get; set; }
    }
}