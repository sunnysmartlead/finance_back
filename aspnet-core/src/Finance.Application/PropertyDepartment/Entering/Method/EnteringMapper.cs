

using AutoMapper;
using Finance.Entering;
using Finance.Entering.Model;
using Finance.ProductDevelopment;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Finance.PropertyDepartment.Entering.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class EnteringMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ElectronicBomInfo, ElectronicDto>()
                 .ForMember(u => u.ElectronicId, options => options.MapFrom(input => input.Id))
                  .ForMember(u => u.Id, options => options.Ignore());
            configuration.CreateMap<ElectronicDto, EnteringElectronic>()
                 .ForMember(u => u.MaterialsUseCount, p => p.MapFrom(input => ListToJson(input.MaterialsUseCount)))
                 .ForMember(u => u.SystemiginalCurrency, p => p.MapFrom(input => ListToJson(input.SystemiginalCurrency)))
                 .ForMember(u => u.InTheRate, p => p.MapFrom(input => ListToJson(input.InTheRate)))
                 .ForMember(u => u.StandardMoney, p => p.MapFrom(input => ListToJson(input.StandardMoney)))
                 .ForMember(u => u.RebateMoney, p => p.MapFrom(input => ListToJson(input.RebateMoney)));
            configuration.CreateMap<StructuralMaterialModel, StructureElectronic>()
                 .ForMember(u => u.StandardMoney, p => p.MapFrom(input => ListToJson(input.StandardMoney)))
                 .ForMember(u => u.MaterialsUseCount, p => p.MapFrom(input => ListToJson(input.MaterialsUseCount)))
                 .ForMember(u => u.InTheRate, p => p.MapFrom(input => ListToJson(input.InTheRate)))
                 .ForMember(u => u.SystemiginalCurrency, p => p.MapFrom(input => ListToJson(input.SystemiginalCurrency)))
                 .ForMember(u => u.RebateMoney, p => p.MapFrom(input => ListToJson(input.RebateMoney)));

            configuration.CreateMap<StructureBomInfo, ConstructionModel>()
                 .ForMember(u => u.StructureId, p => p.MapFrom(input => input.Id))
                 .ForMember(u => u.Id, options => options.Ignore());

            //从数据库实体类 映射到交互类
            configuration.CreateMap<EnteringElectronic, ElectronicDto>()
                 .ForMember(u => u.MaterialsUseCount, p => p.MapFrom(input => JsonToListKV(input.MaterialsUseCount)))
                 .ForMember(u => u.SystemiginalCurrency, p => p.MapFrom(input => JsonToListKV(input.SystemiginalCurrency)))
                 .ForMember(u => u.InTheRate, p => p.MapFrom(input => JsonToListKV(input.InTheRate)))
                 .ForMember(u => u.StandardMoney, p => p.MapFrom(input => JsonToListKV(input.StandardMoney)))
                 .ForMember(u => u.RebateMoney, p => p.MapFrom(input => JsonToKvMode(input.RebateMoney)));


            configuration.CreateMap<ElectronicBomInfoBak, ElectronicBomInfo>();


            configuration.CreateMap<EnteringElectronic, EnteringElectronicCopy>()
                  .ForMember(u => u.IsEntering, p => p.MapFrom(input => false))
                  .ForMember(u => u.IsSubmit, p => p.MapFrom(input => false));
            configuration.CreateMap<StructureElectronic, StructureElectronicCopy>()
                  .ForMember(u => u.IsEntering, p => p.MapFrom(input => false))
                  .ForMember(u => u.IsSubmit, p => p.MapFrom(input => false));

            configuration.CreateMap<EnteringElectronicCopy, ElectronicDtoCopy>()
                 .ForMember(u => u.MaterialsUseCount, p => p.MapFrom(input => JsonToListKV(input.MaterialsUseCount)))
                 .ForMember(u => u.SystemiginalCurrency, p => p.MapFrom(input => JsonToListKV(input.SystemiginalCurrency)))
                 .ForMember(u => u.InTheRate, p => p.MapFrom(input => JsonToListKV(input.InTheRate)))
                 .ForMember(u => u.StandardMoney, p => p.MapFrom(input => JsonToListKV(input.StandardMoney)))
                 .ForMember(u => u.RebateMoney, p => p.MapFrom(input => JsonToKvMode(input.RebateMoney)));

            configuration.CreateMap<StructureBomInfo, ConstructionModelCopy>()
                 .ForMember(u => u.StructureId, p => p.MapFrom(input => input.Id))
                 .ForMember(u => u.Id, options => options.Ignore());
        }
        /// <summary>
        /// 将json 转成   List YearOrValueMode>
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static List<YearOrValueMode> JsonToList(string price)
        {
            return JsonConvert.DeserializeObject<List<YearOrValueMode>>(price);
        }
        /// <summary>
        /// 将json 转成   List YearOrValueMode>
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static List<KvMode> JsonToKvMode(string price)
        {
            return JsonConvert.DeserializeObject<List<KvMode>>(price);
        }
        /// <summary>
        /// 将json 转成   List YearOrValueKvMode>
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static List<YearOrValueKvMode> JsonToListKV(string price)
        {
            return JsonConvert.DeserializeObject<List<YearOrValueKvMode>>(price);
        }
        /// <summary>
        /// 将list 序列化成json
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static string ListToJson<T>(List<T> price) where T : class
        {
            return JsonConvert.SerializeObject(price);
        }
    }
}
