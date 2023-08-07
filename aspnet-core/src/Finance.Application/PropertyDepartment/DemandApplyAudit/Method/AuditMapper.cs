

using AutoMapper;
using Finance.DemandApplyAudit;
using Finance.Entering;
using Finance.Entering.Model;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Finance.PropertyDepartment.DemandApplyAudit.Method
{
    /// <summary>
    /// 营销部审核 映射
    /// </summary>
    public class AuditMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<PricingTeamDto, PricingTeam>();
            configuration.CreateMap<DesignSolutionDto, DesignSolution>();
            configuration.CreateMap<SolutionTable, SolutionTableDto>()
                .ForMember(u=>u.StructEngineerId , options => options.MapFrom(input => ternary(input.StructEngineerId)))
              .ForMember(u => u.ElecEngineerId, options => options.MapFrom(input => ternary(input.ElecEngineerId)));

            configuration.CreateMap<PricingTeam, PricingTeamDto>();
            configuration.CreateMap<DesignSolution, DesignSolutionDto>();
            configuration.CreateMap<SolutionTableDto, SolutionTable>();

            configuration.CreateMap<DesignSolutionModel, DesignSolutionDto>();
        }        
        /// <summary>
        /// 三目运算
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static long? ternary(long prop)
        {
            return prop == 0 ? null : prop;
        }
    }
}
