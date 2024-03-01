using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationDeviceDto : EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 工序编号
        /// </summary>
        public string ProcessNumber { get; set; }
        /// <summary>
        /// 设备list集合 其中包括 设备名称  设备状态  设备单价  设备供应商
        /// </summary>
        public List<FoundationDeviceItemDto> DeviceList { get; set; } = new List<FoundationDeviceItemDto>();

        /// <summary>
        /// 维护人
        /// </summary>
        public string LastModifierUserName { get; set; }
    }
}