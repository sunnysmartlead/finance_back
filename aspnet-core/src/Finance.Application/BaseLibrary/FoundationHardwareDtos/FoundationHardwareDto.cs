using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationHardwareDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public string ProcessName { get; set; }
        public string ProcessNumber { get; set; }
        public string SoftwareName { get; set; }
        public decimal SoftwarePrice { get; set; }
        public string SoftwareState { get; set; }
        public string SoftwareBusiness { get; set; }
        public string LastModifierUserName { get; set; }
        /// <summary>
        /// 追溯软件
        /// </summary>
        public string TraceabilitySoftware { get; set; }

        /// <summary>
        /// 追溯软件费用
        /// </summary>
        public decimal TraceabilitySoftwareCost { get; set; }
        public List<FoundationHardwareItemDto> ListHardware { get; set; } = new List<FoundationHardwareItemDto>();
    }
}