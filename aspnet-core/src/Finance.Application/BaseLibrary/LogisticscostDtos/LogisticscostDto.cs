using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class LogisticscostDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public System.Nullable<System.Int64> AuditFlowId { get; set; }
        public string Classification { get; set; }
        public System.Nullable<System.Decimal> FreightPrice { get; set; }
        public System.Nullable<System.Decimal> MonthlyDemandPrice { get; set; }
        public System.Nullable<System.Decimal> PackagingPrice { get; set; }
        public System.Nullable<System.Int64> SolutionId { get; set; }
        public string Remark { get; set; }
        public System.Nullable<System.Decimal> SinglyDemandPrice { get; set; }
        public System.Nullable<System.Decimal> StoragePrice { get; set; }
        public System.Nullable<System.Decimal> TransportPrice { get; set; }
        public string Year { get; set; }
        public decimal YearMountCount { get; set; }
        public List<LogisticscostResponseDto> LogisticscostList { get; set; }
    }
}