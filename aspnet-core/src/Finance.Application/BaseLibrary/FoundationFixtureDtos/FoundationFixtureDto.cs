using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationFixtureDto: EntityDto<long>
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
        public string FixtureGaugeBusiness { get; set; }
        /// <summary>
        /// 检具名称
        /// </summary>
        public string FixtureGaugeName { get; set; }
        /// <summary>
        /// 检具单价
        /// </summary>
        public decimal FixtureGaugePrice { get; set; }
        /// <summary>
        /// 检具状态
        /// </summary>
        public string FixtureGaugeState { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 工序编号
        /// </summary>
        public string ProcessNumber { get; set; }
        public List<FoundationFixtureItemDto> FixtureList { get; set; } = new List<FoundationFixtureItemDto>();
        /// <summary>
        /// 维护人
        /// </summary>
        public string LastModifierUserName { get; set; }
    }
}