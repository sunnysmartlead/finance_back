﻿using Abp.Application.Services.Dto;
using Finance.Processes;
using System;
using System.Collections.Generic;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationStandardTechnologyDto: EntityDto<long>
    {
        /// <summary>
        /// 维护人
        /// </summary>
        public string LastModifierUserName { get; set; }
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public string Name { get; set; }


        public List<FoundationReliableProcessHoursResponseDto> List { get; set; }
        //工时工序导入专用
        public List<ProcessHoursEnterDto> ProcessHoursEnterDtoList { get; set; }

        

    }
}