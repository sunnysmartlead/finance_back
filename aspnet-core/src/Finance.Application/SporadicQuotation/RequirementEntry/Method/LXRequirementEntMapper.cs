﻿using AutoMapper;
using Finance.EngineeringDepartment;
using Finance.FinanceMaintain;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Finance.PropertyDepartment.UnitPriceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.SporadicQuotation.RequirementEntry.Dto;
using Finance.LXRequirementEntry;

namespace Finance.SporadicQuotation.RequirementEntry.Method
{
    /// <summary>
    /// 注册映射关系
    /// </summary>
    internal class LXRequirementEntMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        { 
            configuration.CreateMap<LXRequirementEntDto, RequirementEnt>();
            configuration.CreateMap<RequirementEnt, LXRequirementEntDto>();

            configuration.CreateMap<LXDataListDto, DataList>();
            configuration.CreateMap<DataList, LXDataListDto>()
               .ForMember(u => u.ListNameDisplayName, p => p.MapFrom(o=>o.ListName.ToString()));//供应商优先级 ;
        }
    }
}
