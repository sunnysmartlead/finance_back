using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class BomEnterDeviceInfoResponseDto : EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public long? AuditFlowId { get; set; }
        public string Classification { get; set; }
        public System.Nullable<System.Decimal> DirectDepreciation { get; set; }
        public System.Nullable<System.Decimal> DirectLaborPrice { get; set; }
        public System.Nullable<System.Decimal> DirectLineChangeCost { get; set; }
        public System.Nullable<System.Decimal> DirectManufacturingCosts { get; set; }
        public System.Nullable<System.Decimal> DirectSummary { get; set; }
        public System.Nullable<System.Decimal> IndirectDepreciation { get; set; }
        public System.Nullable<System.Decimal> IndirectLaborPrice { get; set; }
        public System.Nullable<System.Decimal> IndirectManufacturingCosts { get; set; }
        public System.Nullable<System.Decimal> IndirectSummary { get; set; }
        public long? ProductId { get; set; }
        public string Remark { get; set; }
        public System.Nullable<System.Decimal> TotalCost { get; set; }
        public string Year { get; set; }

        public List<BomEnterDto> ListBomEnter { get; set; }
        public List<BomEnterTotalDto> ListBomEnterTotal{ get; set; }


    }
}