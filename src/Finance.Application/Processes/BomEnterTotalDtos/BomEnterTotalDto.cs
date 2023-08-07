using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class BomEnterTotalDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public System.Nullable<System.Decimal> AuditFlowId { get; set; }
        public string Classification { get; set; }
        public System.Nullable<System.Decimal> ProductId { get; set; }
        public string Remark { get; set; }
        public System.Nullable<System.Decimal> TotalCost { get; set; }
        public string Year { get; set; }
    }
}