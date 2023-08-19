using AutoMapper;
using Finance.Ext;
using Finance.Nre;
using Finance.NrePricing.Dto;
using Finance.NrePricing.Model;
using Finance.Processes;
using Finance.ProductDevelopment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class NrePricingMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {

            configuration.CreateMap<HandPieceCostModel, HandPieceCost>();

            configuration.CreateMap<HandPieceCost, HandPieceCostModel>();


            configuration.CreateMap<RestsCostModel, RestsCost>()
                  .ForMember(u => u.ConstName, p => p.MapFrom(o => o.ConstName)); ;
            configuration.CreateMap<MouldInventoryModel, MouldInventory>();
            configuration.CreateMap<MouldInventory, MouldInventoryModel>();
            configuration.CreateMap<TravelExpenseModel, TravelExpense>();

            configuration.CreateMap<StructureBomInfo, MouldInventoryModel>()
                .ForMember(u => u.ModelName, p => p.MapFrom(o => o.TypeName))
                .ForMember(u => u.Id, options => options.Ignore())
                .ForMember(u => u.StructuralId, p => p.MapFrom(o => o.Id));
            configuration.CreateMap<LaboratoryFeeModel, LaboratoryFee>();
            configuration.CreateMap<EnvironmentalExperimentFeeModel, EnvironmentalExperimentFee>();
            configuration.CreateMap<QADepartmentQCModel, EnvironmentalExperimentFee>();

            configuration.CreateMap<QADepartmentQCModel, QADepartmentQC>()
                 .ForMember(u => u.Cost, p => p.MapFrom(o => o.Count*o.UnitPrice));

            configuration.CreateMap<EnvironmentalExperimentFee, EnvironmentalExperimentFeeModel>();

            configuration.CreateMap<QADepartmentQC, QADepartmentQCModel>()
                 .ForMember(u => u.Cost, p => p.MapFrom(o => o.Count * o.UnitPrice));
            ;
            configuration.CreateMap<InitialSalesDepartmentDto, InitialResourcesManagement>();

            configuration.CreateMap<LaboratoryFee, LaboratoryFeeModel>();
            //.ForMember(u => u.IsThirdPartyName, p => p.MapFrom(o => o.IsThirdParty ? YesNO.Yes.GetDescription() : YesNO.No.GetDescription()));
            configuration.CreateMap<EnvironmentalExperimentFee, LaboratoryFeeModel>();
               //.ForMember(u => u.IsThirdPartyName, p => p.MapFrom(o => o.IsThirdParty ? YesNO.Yes.GetDescription() : YesNO.No.GetDescription()))
               //.ForMember(u => u.TestItem, p => p.MapFrom(o => o.ProjectName));

            configuration.CreateMap<TravelExpense, TravelExpenseModel>();
            configuration.CreateMap<RestsCost, RestsCostModel>()
                 .ForMember(u => u.ConstName, p => p.MapFrom(o => o.ConstName));

            configuration.CreateMap<InitialResourcesManagement, ReturnSalesDepartmentDto>();

            configuration.CreateMap<QADepartmentTestExcelModel, EnvironmentalExperimentFeeModel>()
                 .ForMember(u => u.IsThirdParty, p => p.MapFrom(o => o.IsThirdParty== YesNO.Yes.GetDescription()));

            configuration.CreateMap<LaboratoryFeeExcelModel, LaboratoryFeeModel>()
                .ForMember(u => u.IsThirdParty, p => p.MapFrom(o => o.IsThirdParty == YesNO.Yes.GetDescription()));
                //.ForMember(u => u.IsThirdPartyName, p => p.MapFrom(o => o.IsThirdParty));

            configuration.CreateMap<HandPieceCostModifyDto, HandPieceCostModify>();
            configuration.CreateMap<HandPieceCostModify, HandPieceCostModifyDto>();

            configuration.CreateMap<MouldInventoryModifyDto, MouldInventoryModify>();
            configuration.CreateMap<MouldInventoryModify, MouldInventoryModifyDto>();

            configuration.CreateMap<ToolingCostsModifyDto, ToolingCostsModify>();
            configuration.CreateMap<ToolingCostsModify, ToolingCostsModifyDto>();

            configuration.CreateMap<FixtureCostsModifyDto, FixtureCostsModify>();
            configuration.CreateMap<FixtureCostsModify, FixtureCostsModifyDto>();

            configuration.CreateMap<InspectionToolCostModifyDto, InspectionToolCostModify>();
            configuration.CreateMap<InspectionToolCostModify, InspectionToolCostModifyDto>();

            configuration.CreateMap<ProductionEquipmentCostsModifyDto, ProductionEquipmentCostsModify>();
            configuration.CreateMap<ProductionEquipmentCostsModify, ProductionEquipmentCostsModifyDto>();

            configuration.CreateMap<ExperimentalExpensesModifyDto, ExperimentalExpensesModify>();
            configuration.CreateMap<ExperimentalExpensesModify, ExperimentalExpensesModifyDto>();

            configuration.CreateMap<TestingSoftwareCostsModifyDto, TestingSoftwareCostsModify>();
            configuration.CreateMap<TestingSoftwareCostsModify, TestingSoftwareCostsModifyDto>();

            configuration.CreateMap<TravelExpenseModifyDto, TravelExpenseModify>();
            configuration.CreateMap<TravelExpenseModify, TravelExpenseModifyDto>();

            configuration.CreateMap<RestsCostModifyDto, RestsCostModify>();
            configuration.CreateMap<RestsCostModify, RestsCostModifyDto>();

            configuration.CreateMap<PricingFormDto, ModifyItemPricingFormDto>();
            configuration.CreateMap<ModifyItemPricingFormDto, PricingFormDto>();

            configuration.CreateMap<UphAndValue, ProcessHoursEnterLine>();
            configuration.CreateMap<ProcessHoursEnterLine, UphAndValue>();
        }
        /// <summary>
        /// YesNO枚举
        /// </summary>
        public enum YesNO
        {
            /// <summary>
            /// 否
            /// </summary>
            [Description("否")]
            No,
            /// <summary>
            /// 是
            /// </summary>
            [Description("是")]
            Yes         
        }
    }
}
