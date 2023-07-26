using Abp.Application.Services.Dto;
using System;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationWorkingHourDto: EntityDto<long>
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
    }
}