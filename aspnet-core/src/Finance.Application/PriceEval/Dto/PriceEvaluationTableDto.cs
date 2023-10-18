using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class ManufacturingCostDto2
    {
        /// <summary>
        /// 成本类型
        /// </summary>
        public virtual CostType CostType { get; set; }

        /// <summary>
        /// 成本项目
        /// </summary>
        public virtual string CostItem { get; set; }

        /// <summary>
        /// 直接制造成本：直接人工
        /// </summary>
        public virtual decimal? DirectLabor1 { get; set; }

        /// <summary>
        /// 直接制造成本：设备折旧
        /// </summary>
        public virtual decimal? EquipmentDepreciation1 { get; set; }

        /// <summary>
        /// 直接制造成本：换线成本
        /// </summary>
        public virtual decimal? LineChangeCost1 { get; set; }

        /// <summary>
        /// 直接制造成本：制造费用
        /// </summary>
        public virtual decimal? ManufacturingExpenses1 { get; set; }


        /// <summary>
        /// 小计
        /// </summary>
        public virtual decimal? Subtotal1 { get; set; }

        /// <summary>
        /// 间接制造成本：直接人工
        /// </summary>
        public virtual decimal? DirectLabor2 { get; set; }

        /// <summary>
        /// 间接制造成本：设备折旧
        /// </summary>
        public virtual decimal? EquipmentDepreciation2 { get; set; }

        /// <summary>
        /// 间接制造成本：制造费用
        /// </summary>
        public virtual decimal? ManufacturingExpenses2 { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        public virtual decimal? Subtotal2 { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public virtual decimal Subtotal { get; set; }
    }
    public class ExcelPriceEvaluationTableDto : PriceEvaluationTableDto
    {


        /// <summary>
        /// 制造成本
        /// </summary>
        public virtual List<ManufacturingCostDto2> ManufacturingCostDto { get; set; }

        /// <summary>
        /// 损耗-电子料
        /// </summary>
        public virtual string ShDzl { get; set; }

        /// <summary>
        /// 损耗-结构料
        /// </summary>
        public virtual string ShJgl { get; set; }

        /// <summary>
        /// 损耗-胶水
        /// </summary>
        public virtual string ShJs { get; set; }

        /// <summary>
        /// 损耗-外协加工
        /// </summary>
        public virtual string ShWxjg { get; set; }

        /// <summary>
        /// 损耗-包装材料
        /// </summary>
        public virtual string ShBzcl { get; set; }

        /// <summary>
        /// MOQ-电子料
        /// </summary>
        public virtual string MoqDzl { get; set; }

        /// <summary>
        /// MOQ-结构料
        /// </summary>
        public virtual string MoqJgl { get; set; }

        /// <summary>
        /// MOQ-胶水
        /// </summary>
        public virtual string MoqJs { get; set; }

        /// <summary>
        /// MOQ-外协加工
        /// </summary>
        public virtual string MoqWxjg { get; set; }

        /// <summary>
        /// MOQ-包装材料
        /// </summary>
        public virtual string MoqBzcl { get; set; }


        /// <summary>
        /// 合计金额（人民币）合计
        /// </summary>
        public virtual decimal TotalMoneyCynCount { get; set; }


        /// <summary>
        /// 损耗 合计
        /// </summary>
        public virtual decimal LossCount { get; set; }

        /// <summary>
        /// 损耗率 合计
        /// </summary>
        public virtual decimal LossRateCount { get; set; }


        /// <summary>
        /// 损耗成本合计
        /// </summary>
        public virtual decimal WastageCostCount { get; set; }

        /// <summary>
        /// MOQ分摊成本合计
        /// </summary>
        public virtual decimal MoqShareCountCount { get; set; }




        /// <summary>
        /// 夹具
        /// </summary>
        public virtual decimal? Fixture { get; set; }

        /// <summary>
        /// 物流费
        /// </summary>
        public virtual decimal LogisticsFee { get; set; }

        /// <summary>
        /// 产品类别
        /// </summary>
        public virtual string ProductCategory { get; set; }

        /// <summary>
        /// 成本比例
        /// </summary>
        public virtual decimal CostProportion { get; set; }

        /// <summary>
        /// 成本比例带%
        /// </summary>
        public virtual string CostProportionText { get; set; }

        /// <summary>
        /// 质量成本（MAX)
        /// </summary>
        public virtual decimal QualityCost { get; set; }

        /// <summary>
        /// 账期（天）
        /// </summary>
        public virtual int AccountingPeriod { get; set; }

        /// <summary>
        /// 资金成本率
        /// </summary>
        public virtual decimal? CapitalCostRate { get; set; }

        ///// <summary>
        ///// 财务成本
        ///// </summary>
        //public virtual decimal? FinancialCost { get; set; }

        /// <summary>
        /// 税务成本
        /// </summary>
        public virtual decimal? TaxCost { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public virtual decimal Total { get; set; }
    }
    /// <summary>
    /// 核价表 Dto
    /// 多表联查得到，非实体类
    /// </summary>
    public class PriceEvaluationTableDto
    {
        /// <summary>
        /// 电子大类料合计值
        /// </summary>
        public virtual decimal ElectronicSumValue { get; set; }

        /// <summary>
        /// 核价表的年份，为0表示全生命周期
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 核价表标题
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        public virtual int InputCount { get; set; }

        /// <summary>
        /// 需求量
        /// </summary>
        public virtual decimal RequiredCount { get; set; }

        /// <summary>
        /// 物料（SuperType，超级大种类，区分结构料、电子料、SMT外协等信息）
        /// </summary>
        public virtual List<Material> Material { get; set; }

        /// <summary>
        /// 电子料汇总信息
        /// </summary>
        public virtual List<ElectronicSum> ElectronicSum { get; set; }

        /// <summary>
        /// 结构料汇总信息
        /// </summary>
        public virtual List<StructuralSum> StructuralSum { get; set; }

        /// <summary>
        /// 胶水等辅材汇总信息
        /// </summary>
        public virtual List<GlueMaterialSum> GlueMaterialSum { get; set; }

        /// <summary>
        /// SMT外协汇总信息
        /// </summary>
        public virtual List<SMTOutSourceSum> SMTOutSourceSum { get; set; }

        /// <summary>
        /// 包材汇总信息
        /// </summary>
        public virtual List<PackingMaterialSum> PackingMaterialSum { get; set; }

        #region 成本项目：制造成本



        /// <summary>
        /// 制造成本
        /// </summary>
        public virtual List<ManufacturingCost> ManufacturingCost { get; set; }



        #endregion

        /// <summary>
        /// 损耗成本
        /// </summary>
        public virtual List<LossCost> LossCost { get; set; }

        /// <summary>
        /// 其他成本项目
        /// </summary>
        public virtual OtherCostItem OtherCostItem { get; set; }

        /// <summary>
        /// 其他成本项目2
        /// </summary>
        public virtual List<OtherCostItem2> OtherCostItem2 { get; set; }

        /// <summary>
        /// 成本合计
        /// </summary>
        public virtual decimal TotalCost { get; set; }

        /// <summary>
        /// 编制日期
        /// </summary>
        public virtual DateTime PreparedDate { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public virtual DateTime? AuditDate { get; set; }

        /// <summary>
        /// 批准日期
        /// </summary>
        public virtual DateTime? ApprovalDate { get; set; }
    }

    /// <summary>
    /// 其他成本项目
    /// </summary>
    public class OtherCostItem : EntityDto<int>
    {
        /// <summary>
        /// 夹具
        /// </summary>
        public virtual decimal? Fixture { get; set; }

        /// <summary>
        /// 物流费
        /// </summary>
        public virtual decimal LogisticsFee { get; set; }

        /// <summary>
        /// 产品类别
        /// </summary>
        public virtual string ProductCategory { get; set; }

        /// <summary>
        /// 成本比例
        /// </summary>
        public virtual decimal CostProportion { get; set; }

        /// <summary>
        /// 质量成本（MAX)
        /// </summary>
        public virtual decimal QualityCost { get; set; }

        /// <summary>
        /// 账期（天）
        /// </summary>
        public virtual int AccountingPeriod { get; set; }

        /// <summary>
        /// 资金成本率
        /// </summary>
        public virtual decimal? CapitalCostRate { get; set; }

        ///// <summary>
        ///// 财务成本
        ///// </summary>
        //public virtual decimal? FinancialCost { get; set; }

        /// <summary>
        /// 税务成本
        /// </summary>
        public virtual decimal? TaxCost { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public virtual decimal Total { get; set; }
    }

    /// <summary>
    /// 其他成本项目2
    /// </summary>
    public class OtherCostItem2 //: EntityDto<int>
    {
        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 每年的模组数量
        /// </summary>
        public virtual decimal Quantity { get; set; }

        /// <summary>
        /// 成本项目名称
        /// </summary>
        public virtual string ItemName { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public virtual decimal? Total { get; set; }


        /// <summary>
        /// 模具费分摊
        /// </summary>
        public virtual decimal? MoldCosts { get; set; }


        /// <summary>
        /// 治具费分摊
        /// </summary>
        public virtual decimal? FixtureCost { get; set; }

        /// <summary>
        /// 工装费分摊
        /// </summary>
        public virtual decimal? ToolCost { get; set; }

        /// <summary>
        /// 检具费用分摊
        /// </summary>
        public virtual decimal? InspectionCost { get; set; }

        /// <summary>
        /// 实验费分摊
        /// </summary>
        public virtual decimal? ExperimentCost { get; set; }

        /// <summary>
        /// 专用设备分摊
        /// </summary>
        public virtual decimal? SpecializedEquipmentCost { get; set; }

        /// <summary>
        /// 测试软件费分摊
        /// </summary>
        public virtual decimal? TestSoftwareCost { get; set; }

        /// <summary>
        /// 其他费用分摊
        /// </summary>
        public virtual decimal? OtherExpensesCost { get; set; }

        /// <summary>
        /// 差旅费分摊
        /// </summary>
        public virtual decimal? TravelCost { get; set; }

        /// <summary>
        /// 呆滞物料分摊
        /// </summary>
        public virtual decimal? SluggishCost { get; set; }

        /// <summary>
        /// 质保金分摊
        /// </summary>
        public virtual decimal? RetentionCost { get; set; }

        /// <summary>
        /// 线体成本分摊
        /// </summary>
        public virtual decimal? LineCost { get; set; }

        /// <summary>
        /// 其他成本
        /// </summary>
        public virtual decimal? OtherCost { get; set; }
    }


    /// <summary>
    /// 其他成本项目2核价看板列表项
    /// </summary>
    public class OtherCostItem2List
    {
        /// <summary>
        /// 费用大类
        /// </summary>
        public virtual string CostType { get; set; }

        /// <summary>
        /// 成本项目
        /// </summary>
        public virtual string ItemName { get; set; }

        /// <summary>
        /// 总费用
        /// </summary>
        public virtual decimal Total { get; set; }

        /// <summary>
        /// 分摊数量
        /// </summary>
        public virtual decimal Count { get; set; }

        /// <summary>
        /// 单颗成本
        /// </summary>
        public virtual decimal Cost { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Note { get; set; }

        /// <summary>
        /// 是否分摊
        /// </summary>
        public virtual bool IsShare { get; set; }

        /// <summary>
        /// 分摊年数
        /// </summary>
        public virtual decimal YearCount { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        internal virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        internal virtual YearType UpDown { get; set; }
    }


    /// <summary>
    /// 损耗成本
    /// </summary>
    public class LossCost : EntityDto<int>
    {
        /// <summary>
        /// 修改项Id
        /// </summary>
        public virtual string EditId { get; set; }

        /// <summary>
        /// 修改备注
        /// </summary>
        public virtual string EditNotes { get; set; }

        /// <summary>
        /// 成本项目名称（损耗率、电子料等等）
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 损耗成本
        /// </summary>
        public virtual decimal WastageCost { get; set; }

        /// <summary>
        /// MOQ分摊成本
        /// </summary>
        public virtual decimal MoqShareCount { get; set; }
    }

    /// <summary>
    /// 物料
    /// </summary>
    public class Material : EntityDto<string>
    {
        /// <summary>
        /// 数量（模组年数量）
        /// </summary>
        internal virtual decimal Quantity { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        internal virtual decimal Year { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// 超级大种类
        /// </summary>
        public virtual string SuperType { get; set; }

        /// <summary>
        /// 物料大类
        /// </summary>

        public virtual string CategoryName { get; set; }

        /// <summary>
        /// 物料种类
        /// </summary>

        public virtual string TypeName { get; set; }


        /// <summary>
        /// SAP（物理编码）
        /// </summary>
        public virtual string Sap { get; set; }

        /// <summary>
        /// 材料名称
        /// </summary>
        public virtual string MaterialName { get; set; }

        /// <summary>
        /// 是否客供
        /// </summary>
        public virtual bool IsCustomerSupply { get; set; }

        /// <summary>
        /// 是否客供
        /// </summary>
        public virtual string IsCustomerSupplyStr { get; set; }

        /// <summary>
        /// 装配数量
        /// </summary>
        public virtual double AssemblyCount { get; set; }

        /// <summary>
        /// 材料单价（原币）
        /// </summary>
        public virtual decimal MaterialPrice { get; set; }

        /// <summary>
        /// 系统单价原币 (存json)（计算过年将率后的单价）
        /// </summary>
        internal string SystemiginalCurrency { get; set; }

        /// <summary>
        /// 币别文本
        /// </summary>
        public virtual string CurrencyText { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public virtual decimal ExchangeRate { get; set; }

        /// <summary>
        /// SOP年汇率（即首年）
        /// </summary>
        internal virtual decimal SopExchangeRate { get; set; }

        /// <summary>
        /// 汇率的值  存json字符串
        /// </summary>
        internal virtual string ExchangeRateValue { get; set; }

        /// <summary>
        /// 汇率表Id
        /// </summary>
        internal virtual long ExchangeRateId { get; set; }

        /// <summary>
        /// 本位币(存json)
        /// </summary>
        internal virtual string StandardMoney { get; set; }

        /// <summary>
        /// 材料单价（人民币）
        /// </summary>
        public virtual decimal MaterialPriceCyn { get; set; }

        /// <summary>
        /// 合计金额（人民币）
        /// </summary>
        public virtual decimal TotalMoneyCyn { get; set; }

        /// <summary>
        /// 合计金额（人民币）不含客供
        /// </summary>
        public virtual decimal TotalMoneyCynNoCustomerSupply { get; set; }

        /// <summary>
        /// 损耗
        /// </summary>
        public virtual decimal Loss { get; set; }

        /// <summary>
        /// 损耗率
        /// </summary>
        internal virtual decimal LossRate { get; set; }

        /// <summary>
        /// 材料成本（含损耗）
        /// </summary>
        public virtual decimal MaterialCost { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        public virtual decimal InputCount { get; set; }

        /// <summary>
        /// 采购量
        /// </summary>
        public virtual decimal PurchaseCount { get; set; }

        /// <summary>
        /// MOQ分摊成本
        /// </summary>
        public virtual decimal MoqShareCount { get; set; }

        /// <summary>
        /// MOQ
        /// </summary>
        public virtual decimal Moq { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        public virtual int AvailableInventory { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remarks { get; set; }
    }

    /// <summary>
    /// 制造成本
    /// </summary>
    public class ManufacturingCost : EntityDto<int>
    {
        /// <summary>
        /// 修改项Id
        /// </summary>
        public virtual string EditId { get; set; }

        /// <summary>
        /// 修改备注
        /// </summary>
        public virtual string EditNotes { get; set; }

        /// <summary>
        /// 成本类型
        /// </summary>
        public virtual CostType CostType { get; set; }

        /// <summary>
        /// 成本项目
        /// </summary>
        public virtual string CostItem { get; set; }

        /// <summary>
        /// 梯度K/Y（模组数量）
        /// </summary>
        public virtual decimal GradientKy { get; set; }

        /// <summary>
        /// 月需求量
        /// </summary>
        internal decimal MonthlyDemand { get; set; }

        /// <summary>
        /// 直接制造成本
        /// </summary>
        public virtual ManufacturingCostDirect ManufacturingCostDirect { get; set; }

        /// <summary>
        /// 间接制造成本
        /// </summary>
        public virtual ManufacturingCostIndirect ManufacturingCostIndirect { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public virtual decimal Subtotal { get; set; }
    }

    /// <summary>
    /// 直接制造成本
    /// </summary>
    public class ManufacturingCostDirect : EntityDto<int>
    {
        /// <summary>
        /// 直接制造成本：直接人工
        /// </summary>
        public virtual decimal DirectLabor { get; set; }

        /// <summary>
        /// 直接制造成本：直接人工（未除以月需求量）
        /// </summary>
        internal virtual decimal DirectLaborNo { get; set; }

        /// <summary>
        /// 月需求量
        /// </summary>
        internal decimal MonthlyDemand { get; set; }

        /// <summary>
        /// 直接制造成本：设备折旧
        /// </summary>
        public virtual decimal EquipmentDepreciation { get; set; }

        /// <summary>
        /// 直接制造成本：设备折旧（未除以月需求量）
        /// </summary>
        internal virtual decimal EquipmentDepreciationNo { get; set; }

        /// <summary>
        /// 直接制造成本：换线成本
        /// </summary>
        public virtual decimal LineChangeCost { get; set; }

        /// <summary>
        /// 直接制造成本：制造费用
        /// </summary>
        public virtual decimal ManufacturingExpenses { get; set; }


        /// <summary>
        /// 小计
        /// </summary>
        public virtual decimal Subtotal { get; set; }
    }

    /// <summary>
    /// 间接制造成本
    /// </summary>
    public class ManufacturingCostIndirect : EntityDto<int>
    {
        /// <summary>
        /// 月需求量
        /// </summary>
        internal decimal MonthlyDemand { get; set; }

        /// <summary>
        /// 间接制造成本：直接人工
        /// </summary>
        public virtual decimal DirectLabor { get; set; }

        /// <summary>
        /// 间接制造成本：设备折旧
        /// </summary>
        public virtual decimal EquipmentDepreciation { get; set; }

        /// <summary>
        /// 间接制造成本：制造费用
        /// </summary>
        public virtual decimal ManufacturingExpenses { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        public virtual decimal Subtotal { get; set; }
    }
}
