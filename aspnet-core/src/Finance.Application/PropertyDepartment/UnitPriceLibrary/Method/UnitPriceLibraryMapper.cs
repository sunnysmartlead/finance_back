using Abp.AutoMapper;
using Abp.Extensions;
using Abp.Modules;
using AutoMapper;
using Finance.EngineeringDepartment;
using Finance.Ext;
using Finance.FinanceMaintain;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary
{

    internal class UnitPriceLibraryMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {

            configuration.CreateMap<UInitPriceFormModel, UInitPriceForm>()
                  .ForMember(u => u.SupplierPriority, p => p.MapFrom(o => o.SupplierPriority.ParseEnum(true, EnumDescriptionFunc<SupplierPriority>())))//供应商优先级
                 .ForMember(u => u.FreezeOrNot, p => p.MapFrom(o => o.FreezeOrNot.ParseEnum(true, EnumDescriptionFunc<FreezeOrNot>())))//是否冻结
                 .ForMember(u => u.QuotationType, p => p.MapFrom(o => o.QuotationType.ParseEnum(true, EnumDescriptionFunc<QuotationType>())))//报价类型
                 .ForMember(u => u.MaterialControlStatus, p => p.MapFrom(o => o.MaterialControlStatus.ParseEnum(true, EnumDescriptionFunc<MaterialControlStatus>())))//物料管制状态
                 .ForMember(u => u.UInitPriceFormYearOrValueModes, p => p.MapFrom(o => Serialize(o.UInitPriceFormYearOrValueModes)));


            configuration.CreateMap<UInitPriceForm, UInitPriceFormModel>()
                  .ForMember(u => u.SupplierPriority, p => p.MapFrom(o => o.SupplierPriority.GetDescription()))//供应商优先级
                 .ForMember(u => u.FreezeOrNot, p => p.MapFrom(o => o.FreezeOrNot.GetDescription()))//是否冻结
                 .ForMember(u => u.QuotationType, p => p.MapFrom(o => o.QuotationType.GetDescription()))//报价类型
                 .ForMember(u => u.MaterialControlStatus, p => p.MapFrom(o => o.MaterialControlStatus.GetDescription()))//物料管制状态
                 .ForMember(u => u.UInitPriceFormYearOrValueModes, p => p.MapFrom(o => JsonExchangeRateUInitPriceFormYearOrValueMode(o.UInitPriceFormYearOrValueModes)));


            configuration.CreateMap<BacktrackGrossMarginDto, GrossMarginForm>()
                 .ForMember(u => u.GrossMarginPrice, p => p.MapFrom(o => o.GrossMarginPrice.StringGrossMargin(StringGrossMargin)));
            configuration.CreateMap<GrossMarginForm, GrossMarginDto>()
                 .ForMember(u => u.GrossMarginPrice, p => p.MapFrom(o => o.GrossMarginPrice.SplitGrossMargin(SplitGrossMargin)));
            configuration.CreateMap<ExchangeRateDto, ExchangeRate>()
                 .ForMember(u => u.ExchangeRateValue, p => p.MapFrom(o => JsonConvert.SerializeObject(o.ExchangeRateValue)));
            configuration.CreateMap<ExchangeRate, ExchangeRateDto>()
                .ForMember(u => u.ExchangeRateValue, p => p.MapFrom(o => o.ExchangeRateValue.JsonExchangeRateValue(JsonExchangeRateValue)));

            configuration.CreateMap<SharedMaterialWarehouseMode, SharedMaterialWarehouse>()
                 .ForMember(u => u.ModuleThroughputs, p => p.MapFrom(o => Serialize(o.ModuleThroughputs)));
            configuration.CreateMap<SharedMaterialWarehouse, SharedMaterialWarehouseMode>()
                 .ForMember(u => u.ModuleThroughputs, p => p.MapFrom(o => JsonExchangeRateYearOrValueModeCanNull(o.ModuleThroughputs)));


            configuration.CreateMap<LossRateModel, LossRateInfo>();

            configuration.CreateMap<QualityCostRatioDto, QualityCostRatio>();

            configuration.CreateMap<QualityCostRatioYearDto, QualityCostRatioYear>();
        }
        /// <summary>
        /// 返回枚举参数 的文字描述 的委托
        /// </summary>
        public static Func<T, string> EnumDescriptionFunc<T>() where T : Enum
        {
            return value => ((DescriptionAttribute)typeof(T).GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description;
        }
        /// <summary>
        /// 将list 拆分 string
        /// </summary>
        public static Func<List<decimal>, string> StringGrossMargin = p =>
            {

                if (p.Count > 0)
                {
                    return string.Join("|", p.ToArray());
                }
                else
                {
                    return "";
                }
            };
        /// <summary>
        /// 将字符串拆分成一个一个毛利率  string  to 将list
        /// </summary>
        public static Func<string, List<decimal>> SplitGrossMargin = p =>
        {
            if (!string.IsNullOrEmpty(p))
            {
                string[] price = p.Split('|');
                List<decimal> result = new();
                foreach (var item in price)
                {
                    if (!string.IsNullOrEmpty(item)) result.Add(decimal.Parse(item));
                }
                return result;
            }
            else
            {
                return null;
            }
        };
        /// <summary>
        ///  将json 转成   List YearOrValueMode>
        /// </summary>
        public static Func<string, List<YearOrValueMode>> JsonExchangeRateValue = p =>
        {
            return JsonConvert.DeserializeObject<List<YearOrValueMode>>(p);
        };

        /// <summary>
        ///  将json 转成   List YearOrValueMode>
        /// </summary>
        public static Func<string, List<UInitPriceFormYearOrValueMode>> JsonExchangeRateUInitPriceFormYearOrValueMode = p =>
        {
            return JsonConvert.DeserializeObject<List<UInitPriceFormYearOrValueMode>>(p);
        };
        /// <summary>
        ///  将json 转成   List YearOrValueModeCanNull>
        /// </summary>
        public static Func<string, List<YearOrValueModeCanNull>> JsonExchangeRateYearOrValueModeCanNull = p =>
        {
            return JsonConvert.DeserializeObject<List<YearOrValueModeCanNull>>(p);
        };
        /// <summary>
        ///  序列化
        /// </summary>
        public static Func<object, string> Serialize = p =>
        {
            return JsonConvert.SerializeObject(p);
        };
    }

}
