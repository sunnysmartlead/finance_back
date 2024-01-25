using AutoMapper;
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
using Spire.Pdf.Exporting.XPS.Schema;
using NPOI.POIFS.Crypt.Dsig;
using Finance.Ext;

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

            configuration.CreateMap<LXDataListDto, DataList>()
                  .ForMember(u => u.Data, p => p.MapFrom(o => o.Data.ListToStr()));//Data ,分割
            configuration.CreateMap<DataList, LXDataListDto>()
               .ForMember(u => u.ListNameDisplayName, p => p.MapFrom(o => o.ListName.ToString()))//供应商优先级 ;
              .ForMember(u => u.Data, p => p.MapFrom(o => o.Data.StrToList()));// Data ,合并

            configuration.CreateMap<ManagerApprovalDto, LXRequirementEntDto>();
            configuration.CreateMap<LXRequirementEntDto, ManagerApprovalDto>();
        } 
    }
}
