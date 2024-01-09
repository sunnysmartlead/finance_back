using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 治具
    /// </summary>
    public class ProcessHoursEnterFixtureDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string FixtureName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public System.Nullable<System.Decimal> FixtureNumber { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public System.Nullable<System.Decimal> FixturePrice { get; set; }
        /// <summary>
        /// 工时工序id
        /// </summary>
        public System.Nullable<System.Decimal> ProcessHoursEnterId { get; set; }
    }
}