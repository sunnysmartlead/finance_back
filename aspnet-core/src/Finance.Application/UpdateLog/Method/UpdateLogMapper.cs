using AutoMapper;
using Finance.Nre;
using Finance.NrePricing.Dto;
using Finance.NrePricing.Model;
using Finance.ProductDevelopment;
using Finance.UpdateLog.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.UpdateLog.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateLogMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {

            configuration.CreateMap<Versions, VersionsDto>();
            configuration.CreateMap<VersionsDto, Versions>();

            configuration.CreateMap<UpdateLogInfoDto, UpdateLogInfo>();
            configuration.CreateMap<UpdateLogInfo, UpdateLogInfoDto>();


            configuration.CreateMap<Versions, VersionsUpdateLogInfoDto>();
            configuration.CreateMap<VersionsUpdateLogInfoDto, Versions>();
        }
    }
}
