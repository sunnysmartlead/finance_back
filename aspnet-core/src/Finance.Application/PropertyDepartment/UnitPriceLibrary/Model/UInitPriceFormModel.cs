using Abp.Domain.Entities.Auditing;
using Finance.Ext;
using Finance.PropertyDepartment.Entering.Model;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary.Model
{
    /// <summary>
    /// 基础单价表 模型
    /// </summary>
    public class UInitPriceFormModel  
    {
        /// <summary>
        /// 价格主数据号
        /// </summary>      
        [ExcelColumnName("价格主数据号")]
        public virtual string PriceMasterDataNumber { get; set; }
        /// <summary>
        /// 采购组编码
        /// </summary>
        [ExcelColumnName("采购组编码")]
        public virtual string ProcurementGroupCode { get; set; }
        /// <summary>
        /// 采购组名称
        /// </summary>
        [ExcelColumnName("采购组名称")]
        public virtual string ProcurementGroupName { get; set; }
        /// <summary>
        /// 采购组织编码
        /// </summary>
        [ExcelColumnName("采购组织编码")]
        public virtual string PurchaseOrganizationCode { get; set; }
        /// <summary>
        /// 采购组织名称
        /// </summary>
        [ExcelColumnName("采购组织名称")]
        public virtual string PurchasingOrganizationName { get; set; }
        /// <summary>
        /// 工厂编码
        /// </summary>
        [ExcelColumnName("工厂编码")]
        public virtual string FactoryCode { get; set; }
        /// <summary>
        /// 工厂名称
        /// </summary>
        [ExcelColumnName("工厂名称")]
        public virtual string FactoryName { get; set; }
        /// <summary>
        /// 供应商编码
        /// </summary>
        [ExcelColumnName("供应商编码")]
        public virtual string SupplierCode { get; set; }
        /// <summary>
        /// 供应商优先级
        /// </summary>
        [ExcelColumnName("供应商优先级")]
        public virtual string SupplierPriority { get; set; }
        /// <summary>
        /// 供应商ERP编码
        /// </summary>
        [ExcelColumnName("供应商ERP编码")]
        public virtual string SupplierErpCode { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        [ExcelColumnName("供应商名称")]
        public virtual string SupplierName { get; set; }
        /// <summary>
        /// 一级物料簇编码
        /// </summary>
        [ExcelColumnName("一级物料簇编码")]
        public virtual string FirstLevelMaterialClusterCode { get; set; }
        /// <summary>
        /// 一级物料簇名称
        /// </summary>
        [ExcelColumnName("一级物料簇名称")]
        public virtual string FirstLevelMaterialClusterName { get; set; }
        /// <summary>
        /// 二级物料簇编码
        /// </summary>
        [ExcelColumnName("二级物料簇编码")]
        public virtual string SecondaryMaterialClusterCode { get; set; }
        /// <summary>
        /// 二级物料簇名称
        /// </summary>
        [ExcelColumnName("二级物料簇名称")]
        public virtual string SecondaryMaterialClusterName { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [ExcelColumnName("物料编码")]
        public virtual string MaterialCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [ExcelColumnName("物料名称")]
        public virtual string MaterialName { get; set; }
        /// <summary>
        /// 采购类别
        /// </summary>
        [ExcelColumnName("采购类别")]
        public virtual string PurchaseCategory { get; set; }
        /// <summary>
        /// 基本单位
        /// </summary>
        [ExcelColumnName("基本单位")]
        public virtual string BasicUnit { get; set; }
        /// <summary>
        /// 是否客户指定
        /// </summary>
        [ExcelColumnName("是否客户指定")]
        public virtual string IsItSpecifiedByTheCustomer { get; set; }
        /// <summary>
        /// 是否冻结
        /// </summary>
        [ExcelColumnName("是否冻结")]
        public virtual string FreezeOrNot { get; set; }
        /// <summary>
        /// 同步PO状态
        /// </summary>
        [ExcelColumnName("同步PO状态")]
        public virtual string SynchronizePOstatus { get; set; }
        /// <summary>
        /// 物料冻结
        /// </summary>
        [ExcelColumnName("物料冻结")]
        public virtual string MaterialFreezing { get; set; }
        /// <summary>
        /// 采购订单冻结
        /// </summary>
        [ExcelColumnName("采购订单冻结")]
        public virtual string PurchaseOrderFreeze { get; set; }
        /// <summary>
        /// 收获冻结
        /// </summary>
        [ExcelColumnName("收获冻结")]
        public virtual string HarvestFreeze { get; set; }
        /// <summary>
        /// 付款冻结
        /// </summary>
        [ExcelColumnName("付款冻结")]
        public virtual string PaymentFreeze { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [ExcelColumnName("创建人")]
        public virtual string Founder { get; set; }
        /// <summary>
        /// 生效日期
        /// </summary>
        [ExcelFormat("yyyy-MM-dd")]
        [ExcelColumnName("生效日期")]
        public virtual DateTime EffectiveDate { get; set; }
        /// <summary>
        /// 失效日期
        /// </summary>
        [ExcelFormat("yyyy-MM-dd")]
        [ExcelColumnName("失效日期")]
        public virtual DateTime ExpirationDate { get; set; }
        /// <summary>
        /// 价格基数
        /// </summary>         
        [ExcelColumnName("价格基数")]
        public virtual string PriceBase { get; set; }
        /// <summary>
        /// 定价单位
        /// </summary>         
        [ExcelColumnName("定价单位")]
        public virtual string PricingUnit { get; set; }
        /// <summary>
        /// 货币编码
        /// </summary>         
        [ExcelColumnName("货币编码")]
        public virtual string CurrencyCode { get; set; }
        /// <summary>
        /// 税率
        /// </summary>         
        [ExcelColumnName("税率")]
        public virtual string TaxRate { get; set; }
        /// <summary>
        /// 未税价
        /// </summary>         
        [ExcelColumnName("未税价")]
        public virtual string UntaxedPrice { get; set; }
        /// <summary>
        /// 含税价
        /// </summary>         
        [ExcelColumnName("含税价")]
        public virtual string PriceIncludingTax { get; set; }
        /// <summary>
        /// 成本基价
        /// </summary>         
        [ExcelColumnName("成本基价")]
        public virtual string CostBasePrice { get; set; }
        /// <summary>
        /// 返利未税价
        /// </summary>         
        [ExcelColumnName("返利未税价")]
        public virtual string RebateBeforeTaxPrice { get; set; }
        /// <summary>
        /// 返利含税价
        /// </summary>         
        [ExcelColumnName("返利含税价")]
        public virtual string RebatePriceIncludingTax { get; set; }
        /// <summary>
        /// 过量交货限度%
        /// </summary>         
        [ExcelColumnName("过量交货限度%")]
        public virtual string ExcessiveDeliveryLimit { get; set; }
        /// <summary>
        /// 交货不足限度%
        /// </summary>         
        [ExcelColumnName("交货不足限度%")]
        public virtual string InsufficientDeliveryLimit { get; set; }
        /// <summary>
        /// 计划天数
        /// </summary>         
        [ExcelColumnName("计划天数")]
        public virtual string PlannedDays { get; set; }
        /// <summary>
        /// 关税
        /// </summary>         
        [ExcelColumnName("关税")]
        public virtual string Tariff { get; set; }
        /// <summary>
        /// 原净价
        /// </summary>         
        [ExcelColumnName("原净价")]
        public virtual string OriginalNetPrice { get; set; }
        /// <summary>
        /// 价格变动类型
        /// </summary>         
        [ExcelColumnName("价格变动类型")]
        public virtual string TypeOfPriceChange { get; set; }
        /// <summary>
        /// 原有效日期
        /// </summary>         
        [ExcelFormat("yyyy-MM-dd")]
        [ExcelColumnName("原有效日期")]
        public virtual DateTime OriginalEffectiveDate { get; set; }
        /// <summary>
        /// 原失效日期
        /// </summary>      
        [ExcelColumnName("原失效日期")]
        public virtual string OriginalExpirationDate { get; set; }
        /// <summary>
        /// 返利模式
        /// </summary>      
        [ExcelColumnName("返利模式")]
        public virtual string RebateMode { get; set; }
        /// <summary>
        /// 返点信息
        /// </summary>      
        [ExcelColumnName("返点信息")]
        public virtual string RebateInformation { get; set; }
        /// <summary>
        /// 创建方式
        /// </summary>      
        [ExcelColumnName("创建方式")]
        public virtual string CreationMethod { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>      
        [ExcelColumnName("版本号")]
        public virtual string VersionNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>      
        [ExcelColumnName("备注")]
        public virtual string Remarks { get; set; }
        /// <summary>
        /// 报价类型
        /// </summary>      
        [ExcelColumnName("报价类型")]
        public virtual string QuotationType { get; set; }
        /// <summary>
        /// 阶梯/批量起始数量
        /// </summary>      
        [ExcelColumnName("阶梯/批量起始数量")]
        public virtual decimal BatchStartQuantity { get; set; }
        /// <summary>
        /// 阶梯/批量未税价（当期价格）
        /// </summary>      
        [ExcelColumnName("阶梯/批量未税价（当期价格）")]
        public virtual decimal BulkUntaxedPrice { get; set; }
        /// <summary>
        /// 阶梯/批量含税价
        /// </summary>      
        [ExcelColumnName("阶梯/批量含税价")]
        public virtual string BatchPriceIncludingTax { get; set; }
        /// <summary>
        /// 阶梯/批量返利未税价
        /// </summary>      
        [ExcelColumnName("阶梯/批量返利未税价")]
        public virtual string BatchRebateWithoutTaxPrice { get; set; }
        /// <summary>
        /// 阶梯/批量返利含税价
        /// </summary>      
        [ExcelColumnName("阶梯/批量返利含税价")]
        public virtual string BatchRebateIncludingTaxPrice { get; set; }
        /// <summary>
        /// 原产地信息
        /// </summary>      
        [ExcelColumnName("原产地信息")]
        public virtual string OriginInformation { get; set; }
        /// <summary>
        /// 物料管制状态
        /// </summary>
        [ExcelColumnName("物料管制状态")]
        public virtual string MaterialControlStatus { get; set; }
        /// <summary>
        /// MOQ
        /// </summary>
        [ExcelColumnName("MOQ")]
        public virtual decimal Moq { get; set; }
        /// <summary>
        /// 累积采购数量
        /// </summary>
        [ExcelColumnName("累积采购数量")]
        public virtual decimal AccumulatedProcurementQuantity { get; set; }
        /// <summary>
        /// 返点率and年将率and年未税价 集合
        /// </summary>
        public List<UInitPriceFormYearOrValueMode> UInitPriceFormYearOrValueModes { get; set; }

    }
    /// <summary>
    /// 返点率在Excel中的 列
    /// </summary>
    public class RebateRateColumn
    {
        /// <summary>
        /// BC列
        /// </summary>
        [ExcelColumnIndex("BC")]
        public decimal BC { get; set; }
        /// <summary>
        /// BD列
        /// </summary>
        [ExcelColumnIndex("BD")]
        public decimal BD { get; set; }
        /// <summary>
        /// BE列
        /// </summary>
        [ExcelColumnIndex("BE")]
        public decimal BE { get; set; }
        /// <summary>
        /// BF列
        /// </summary>
        [ExcelColumnIndex("BF")]
        public decimal BF { get; set; }
        /// <summary>
        /// BG列
        /// </summary>
        [ExcelColumnIndex("BG")]
        public decimal BG { get; set; }

    }
    /// <summary>
    /// 年将率在Excel中的 列
    /// </summary>
    public class AnnualGeneralRateColumn
    {
        /// <summary>
        /// BH列
        /// </summary>
        [ExcelColumnIndex("BH")]
        public decimal BH { get; set; }
        /// <summary>
        /// BI列
        /// </summary>
        [ExcelColumnIndex("BI")]
        public decimal BI { get; set; }
        /// <summary>
        /// BJ列
        /// </summary>
        [ExcelColumnIndex("BJ")]
        public decimal BJ { get; set; }
        /// <summary>
        /// BK列
        /// </summary>
        [ExcelColumnIndex("BK")]
        public decimal BK { get; set; }    

    }
    /// <summary>
    /// 未税价在Excel中的 列
    /// </summary>
    public class UntaxedPriceColumn
    {
        /// <summary>
        /// BL列
        /// </summary>
        [ExcelColumnIndex("BL")]
        public decimal BL { get; set; }
        /// <summary>
        /// BM列
        /// </summary>
        [ExcelColumnIndex("BM")]
        public decimal BM { get; set; }
        /// <summary>
        /// BN列
        /// </summary>
        [ExcelColumnIndex("BN")]
        public decimal BN { get; set; }
        /// <summary>
        /// BO列
        /// </summary>
        [ExcelColumnIndex("BO")]
        public decimal BO { get; set; }
    }
    /// <summary>
    /// 单价库中 返点率and年将率and年未税价
    /// </summary>
    public class UInitPriceFormYearOrValueMode: YearOrValueMode
    {
        /// <summary>
        ///类型定义
        /// </summary>
        public UInitPriceFormType UInitPriceFormType { get; set; }
    }
    /// <summary>
    /// 单价库中 返点率and年将率and年未税价 类型定义
    /// </summary>
    public enum UInitPriceFormType
    {
        /// <summary>
        /// 返点率
        /// </summary>
        RebateRate,
        /// <summary>
        /// 年将率
        /// </summary>
        AnnualDeclineRate,
        /// <summary>
        /// 年未税价
        /// </summary>
        AnnualUntaxedPrice
    }
}
