﻿using Abp.Application.Services.Dto;
using System;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationFixtureItemDto: EntityDto<long>
    {
        
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public string FixtureBusiness { get; set; }
        /// <summary>
        ///  治具名称
        /// </summary>
        public string FixtureName { get; set; }
        /// <summary>
        /// 治具单价
        /// </summary>
        public System.Nullable<System.Decimal> FixturePrice { get; set; }
        /// <summary>
        /// 治具状态
        /// </summary>
        public string FixtureState { get; set; }
        /// <summary>
        /// 治具供应商
        /// </summary>
        public string FixtureProvider { get; set; }
        public System.Nullable<System.Decimal> FoundationFixtureId { get; set; }
    }
}