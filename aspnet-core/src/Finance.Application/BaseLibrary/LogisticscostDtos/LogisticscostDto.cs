using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        /// <summary>
        /// 运输月
        /// </summary>
        public System.Nullable<System.Decimal> FreightPrice { get; set; }
        /// <summary>
        /// 运输月
        /// </summary>
        public System.Nullable<System.Decimal> MonthlyDemandPrice { get; set; }

        /// <summary>
        ///     单PCS包装价格/元
        /// </summary>
        public System.Nullable<System.Decimal> PackagingPrice { get; set; }
        public System.Nullable<System.Int64> SolutionId { get; set; }
        public string Remark { get; set; }
        /// <summary>
        ///  单PCS运输费
        /// </summary>
        public System.Nullable<System.Decimal> SinglyDemandPrice { get; set; }
        public System.Nullable<System.Decimal> StoragePrice { get; set; }

        /// <summary>
        /// 单PCS总物流成本
        /// </summary>
        public System.Nullable<System.Decimal> TransportPrice { get; set; }
        public string Year { get; set; }

        /// <summary>
        /// 需求量
        /// </summary>
        public decimal YearMountCount { get; set; }
        public long ModelCountYearId { get; set; }

        /// <summary>
        /// 月度
        /// </summary>
        public long Moon { get; set; }


        /// <summary>
        ///  0 保存  1 提交
        /// </summary>
        public long Status { get; set; }
        public List<LogisticscostResponseDto> LogisticscostList { get; set; }
    }
}