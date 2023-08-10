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
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public string FixtureGaugeBusiness { get; set; }
        public string FixtureGaugeName { get; set; }
        public string FixtureGaugePrice { get; set; }
        public string FixtureGaugeState { get; set; }
        public string ProcessName { get; set; }
        public string ProcessNumber { get; set; }
        public List<FoundationFixtureItemDto> FixtureList { get; set; }
        /// <summary>
        /// 维护人
        /// </summary>
        public string LastModifierUserName { get; set; }
    }
}