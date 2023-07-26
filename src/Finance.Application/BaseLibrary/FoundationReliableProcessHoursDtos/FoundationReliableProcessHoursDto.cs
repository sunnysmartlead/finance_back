using Abp.Application.Services.Dto;
using System;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationReliableProcessHoursDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public System.Nullable<System.Decimal> Development { get; set; }
        public System.Nullable<System.Decimal> DevelopTotalPrice { get; set; }
        public System.Nullable<System.Decimal> DeviceTotalPrice { get; set; }
        public string DrawingSoftware { get; set; }
        public string FigureSoftware { get; set; }
        public string FrockName { get; set; }
        public System.Nullable<System.Decimal> FrockNumber { get; set; }
        public System.Nullable<System.Decimal> HardwareDeviceTotalPrice { get; set; }
        public System.Nullable<System.Decimal> PictureDevelopment { get; set; }
        public string ProcessName { get; set; }
        public string ProcessNumber { get; set; }
        public System.Nullable<System.Decimal> SoftwareHardPrice { get; set; }
        public System.Nullable<System.Decimal> SoftwarePrice { get; set; }
        public System.Nullable<System.Decimal> StandardTechnologyId { get; set; }
        public string TestLineName { get; set; }
        public System.Nullable<System.Decimal> TestLineNumber { get; set; }
        public System.Nullable<System.Decimal> TestLinePrice { get; set; }
        public System.Nullable<System.Decimal> TotalHardwarePrice { get; set; }
        public string TraceabilitySoftware { get; set; }
    }
}