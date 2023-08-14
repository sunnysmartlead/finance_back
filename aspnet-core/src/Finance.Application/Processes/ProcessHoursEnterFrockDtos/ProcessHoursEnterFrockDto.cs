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
        public string HardwareDeviceName { get; set; }
        public System.Nullable<System.Decimal> HardwareDeviceNumber { get; set; }
        public System.Nullable<System.Decimal> HardwareDevicePrice { get; set; }
        public System.Nullable<System.Decimal> ProcessHoursEnterId { get; set; }
    }
}