using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterFrockDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        /// <summary>
        /// 工时工序设备名称
        /// </summary>
        public string HardwareDeviceName { get; set; }
        /// <summary>
        /// 硬件设备数量
        /// </summary>
        public System.Nullable<System.Decimal> HardwareDeviceNumber { get; set; }
        /// <summary>
        /// 硬件单价
        /// </summary>
        public System.Nullable<System.Decimal> HardwareDevicePrice { get; set; }
        /// <summary>
        /// 工时工序id
        /// </summary>
        public System.Nullable<System.Decimal> ProcessHoursEnterId { get; set; }
    }
}