using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnterDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public System.Nullable<System.Decimal> AuditFlowId { get; set; }
        public string DevelopTotalPrice { get; set; }
        public System.Nullable<System.Decimal> DeviceTotalPrice { get; set; }
        public string FixtureName { get; set; }
        public string FixtureNumber { get; set; }
        public System.Nullable<System.Decimal> FixturePrice { get; set; }
        public string FrockName { get; set; }
        public System.Nullable<System.Decimal> FrockNumber { get; set; }
        public System.Nullable<System.Decimal> FrockPrice { get; set; }
        public System.Nullable<System.Decimal> HardwareDeviceTotalPrice { get; set; }
        public System.Nullable<System.Decimal> HardwareTotalPrice { get; set; }
        public string OpenDrawingSoftware { get; set; }
        public string ProcessName { get; set; }
        public string ProcessNumber { get; set; }
        public System.Nullable<System.Decimal> ProductId { get; set; }
        public System.Nullable<System.Decimal> SoftwarePrice { get; set; }
        public string TestLineName { get; set; }
        public System.Nullable<System.Decimal> TestLineNumber { get; set; }
        public System.Nullable<System.Decimal> TestLinePrice { get; set; }
    }
}