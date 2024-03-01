using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationDeviceItemDto : EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 设备单价
        /// </summary>
        public decimal DevicePrice { get; set; }      
        public decimal? DeviceSort { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public string DeviceStatus { get; set; }
        /// <summary>
        /// 设备供应商
        /// </summary>
        public string DeviceProvider { get; set; }
        public long? ProcessHoursEnterId { get; set; }
    }
}