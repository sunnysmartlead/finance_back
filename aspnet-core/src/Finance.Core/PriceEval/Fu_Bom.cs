using Abp.Domain.Entities;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    public class Fu_Bom : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 本表的原Id
        /// </summary>
        public virtual string Tid { get; set; }

        /// <summary>
        /// 数量（模组年数量）
        /// </summary>
        public virtual decimal Quantity { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual decimal Year { get; set; }

        /// <summary>
        /// 年份上下
        /// </summary>
        public virtual YearType UpDown { get; set; }

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
        [ExcelIgnore]
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
        /// 币别文本
        /// </summary>
        public virtual string CurrencyText { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public virtual decimal ExchangeRate { get; set; }

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

        /// <summary>
        /// 修改意见
        /// </summary>
        public string ModificationComments { get; set; }
    }


    public class Fu_ManufacturingCost : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 本表的原Id
        /// </summary>
        public virtual int Tid { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份上下
        /// </summary>
        public virtual YearType UpDown { get; set; }

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


        #region 直接制造成本

        /// <summary>
        /// 本表的原Id
        /// </summary>
        public virtual int Did { get; set; }

        /// <summary>
        /// 年份上下
        /// </summary>
        public virtual YearType DUpDown { get; set; }

        /// <summary>
        /// 直接制造成本：直接人工
        /// </summary>
        public virtual decimal DDirectLabor { get; set; }

        /// <summary>
        /// 直接制造成本：设备折旧
        /// </summary>
        public virtual decimal DEquipmentDepreciation { get; set; }

        /// <summary>
        /// 直接制造成本：换线成本
        /// </summary>
        public virtual decimal DLineChangeCost { get; set; }

        /// <summary>
        /// 直接制造成本：制造费用
        /// </summary>
        public virtual decimal DManufacturingExpenses { get; set; }


        /// <summary>
        /// 小计
        /// </summary>
        public virtual decimal DSubtotal { get; set; }

        #endregion


        #region 间接制造成本

        /// <summary>
        /// 本表的原Id
        /// </summary>
        public virtual int Iid { get; set; }

        /// <summary>
        /// 年份上下
        /// </summary>
        public virtual YearType IUpDown { get; set; }

        /// <summary>
        /// 间接制造成本：直接人工
        /// </summary>
        public virtual decimal IDirectLabor { get; set; }

        /// <summary>
        /// 间接制造成本：设备折旧
        /// </summary>
        public virtual decimal IEquipmentDepreciation { get; set; }

        /// <summary>
        /// 间接制造成本：制造费用
        /// </summary>
        public virtual decimal IManufacturingExpenses { get; set; }

        /// <summary>
        /// 小计
        /// </summary>
        public virtual decimal ISubtotal { get; set; }

        #endregion


        /// <summary>
        /// 合计
        /// </summary>
        public virtual decimal Subtotal { get; set; }
    }

    public class Fu_LossCost : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份上下
        /// </summary>
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 本表的原Id
        /// </summary>
        public virtual int Tid { get; set; }

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
    public class Fu_OtherCostItem2 : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

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
    public class Fu_OtherCostItem : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份上下
        /// </summary>
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 本表的原Id
        /// </summary>
        public virtual int Tid { get; set; }

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
    public class Fu_QualityCostListDto : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 修改项Id
        /// </summary>
        public virtual string EditId { get; set; }

        /// <summary>
        /// 修改备注
        /// </summary>
        public virtual string EditNotes { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }

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
    }
    public class Fu_LogisticsCost : Entity<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 梯度Id
        /// </summary>
        [Required]
        public virtual long GradientId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }

        /// <summary>
        /// 修改项Id
        /// </summary>
        public virtual string EditId { get; set; }

        /// <summary>
        /// 修改备注
        /// </summary>
        public virtual string EditNotes { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 单pcs包装价格/元
        /// </summary>
        public decimal PerPackagingPrice { get; set; }

        /// <summary>
        /// 运费/月
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 仓储费用/元
        /// </summary>
        public decimal StorageExpenses { get; set; }

        /// <summary>
        /// 月底需求量
        /// </summary>
        public decimal MonthEndDemand { get; set; }

        /// <summary>
        /// 单PCS运输费
        /// </summary>
        public decimal PerFreight { get; set; }

        /// <summary>
        /// 单PCS总物流成本
        /// </summary>
        public decimal PerTotalLogisticsCost { get; set; }
    }


}
