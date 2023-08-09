using Abp.Domain.Entities.Auditing;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.FinanceMaintain
{
    /// <summary>
    /// 基础单价表
    /// </summary>
    public class UInitPriceForm : FullAuditedEntity<long>
    {
        /// <summary>
        /// 价格主数据号
        /// </summary>          
        public virtual string PriceMasterDataNumber { get; set; }
        /// <summary>
        /// 采购组编码
        /// </summary>        
        public virtual string ProcurementGroupCode { get; set; }
        /// <summary>
        /// 采购组名称
        /// </summary>
        public virtual string ProcurementGroupName { get; set; }
        /// <summary>
        /// 采购组织编码
        /// </summary>
        public virtual string PurchaseOrganizationCode { get; set; }
        /// <summary>
        /// 采购组织名称
        /// </summary>
        public virtual string PurchasingOrganizationName { get; set; }
        /// <summary>
        /// 工厂编码
        /// </summary>
        public virtual string FactoryCode { get; set; }
        /// <summary>
        /// 工厂名称
        /// </summary>
        public virtual string FactoryName { get; set; }
        /// <summary>
        /// 供应商编码
        /// </summary>
        public virtual string SupplierCode { get; set; }
        /// <summary>
        /// 供应商优先级
        /// </summary>
        public virtual SupplierPriority SupplierPriority { get; set; }
        /// <summary>
        /// 供应商ERP编码
        /// </summary>
        public virtual string SupplierErpCode { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public virtual string SupplierName { get; set; }
        /// <summary>
        /// 一级物料簇编码
        /// </summary>
        public virtual string FirstLevelMaterialClusterCode { get; set; }
        /// <summary>
        /// 一级物料簇名称
        /// </summary>
        public virtual string FirstLevelMaterialClusterName { get; set; }
        /// <summary>
        /// 二级物料簇编码
        /// </summary>
        public virtual string SecondaryMaterialClusterCode { get; set; }
        /// <summary>
        /// 二级物料簇名称
        /// </summary>
        public virtual string SecondaryMaterialClusterName { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public virtual string MaterialCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public virtual string MaterialName { get; set; }
        /// <summary>
        /// 采购类别
        /// </summary>
        public virtual string PurchaseCategory { get; set; }
        /// <summary>
        /// 基本单位
        /// </summary>
        public virtual string BasicUnit { get; set; }
        /// <summary>
        /// 是否客户指定
        /// </summary>
        public virtual string IsItSpecifiedByTheCustomer { get; set; }
        /// <summary>
        /// 是否冻结
        /// </summary>
        public virtual FreezeOrNot FreezeOrNot { get; set; }
        /// <summary>
        /// 同步PO状态
        /// </summary>
        public virtual string SynchronizePOstatus { get; set; }
        /// <summary>
        /// 物料冻结
        /// </summary>
        public virtual string MaterialFreezing { get; set; }
        /// <summary>
        /// 采购订单冻结
        /// </summary>
        public virtual string PurchaseOrderFreeze { get; set; }
        /// <summary>
        /// 收获冻结
        /// </summary>
        public virtual string HarvestFreeze { get; set; }
        /// <summary>
        /// 付款冻结
        /// </summary>
        public virtual string PaymentFreeze { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public virtual string Founder { get; set; }
        /// <summary>
        /// 生效日期
        /// </summary>
        public virtual DateTime EffectiveDate { get; set; }
        /// <summary>
        /// 失效日期
        /// </summary>      
        public virtual DateTime ExpirationDate { get; set; }
        /// <summary>
        /// 价格基数
        /// </summary>      
        public virtual string PriceBase { get; set; }
        /// <summary>
        /// 定价单位
        /// </summary>                
        public virtual string PricingUnit { get; set; }
        /// <summary>
        /// 货币编码
        /// </summary>                 
        public virtual string CurrencyCode { get; set; }
        /// <summary>
        /// 税率
        /// </summary>         
        public virtual string TaxRate { get; set; }
        /// <summary>
        /// 未税价
        /// </summary>         
        public virtual string UntaxedPrice { get; set; }
        /// <summary>
        /// 含税价
        /// </summary>         
        public virtual string PriceIncludingTax { get; set; }
        /// <summary>
        /// 成本基价
        /// </summary>         
        public virtual string CostBasePrice { get; set; }
        /// <summary>
        /// 返利未税价
        /// </summary>         
        public virtual string RebateBeforeTaxPrice { get; set; }
        /// <summary>
        /// 返利含税价
        /// </summary>         
        public virtual string RebatePriceIncludingTax { get; set; }
        /// <summary>
        /// 过量交货限度%
        /// </summary>         
        public virtual string ExcessiveDeliveryLimit { get; set; }
        /// <summary>
        /// 交货不足限度%
        /// </summary>         
        public virtual string InsufficientDeliveryLimit { get; set; }
        /// <summary>
        /// 计划天数
        /// </summary>         
        public virtual string PlannedDays { get; set; }
        /// <summary>
        /// 关税
        /// </summary>         
        public virtual string Tariff { get; set; }
        /// <summary>
        /// 原净价
        /// </summary>         
        public virtual string OriginalNetPrice { get; set; }
        /// <summary>
        /// 价格变动类型
        /// </summary>         
        public virtual string TypeOfPriceChange { get; set; }
        /// <summary>
        /// 原有效日期
        /// </summary>        
        public virtual DateTime OriginalEffectiveDate { get; set; }
        /// <summary>
        /// 原失效日期
        /// </summary>      
        public virtual string OriginalExpirationDate { get; set; }
        /// <summary>
        /// 返利模式
        /// </summary>      
        public virtual string RebateMode { get; set; }
        /// <summary>
        /// 返点信息
        /// </summary>      
        public virtual string RebateInformation { get; set; }
        /// <summary>
        /// 创建方式
        /// </summary>      
        public virtual string CreationMethod { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>      
        public virtual string VersionNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>      
        public virtual string Remarks { get; set; }
        /// <summary>
        /// 报价类型
        /// </summary>      
        public virtual QuotationType QuotationType { get; set; }
        /// <summary>
        /// 阶梯/批量起始数量
        /// </summary>      
        public virtual decimal BatchStartQuantity { get; set; }
        /// <summary>
        /// 阶梯/批量未税价（当期价格）
        /// </summary>      
        public virtual decimal BulkUntaxedPrice { get; set; }
        /// <summary>
        /// 阶梯/批量含税价
        /// </summary>      
        public virtual string BatchPriceIncludingTax { get; set; }
        /// <summary>
        /// 阶梯/批量返利未税价
        /// </summary>      
        public virtual string BatchRebateWithoutTaxPrice { get; set; }
        /// <summary>
        /// 阶梯/批量返利含税价
        /// </summary>      
        public virtual string BatchRebateIncludingTaxPrice { get; set; }
        /// <summary>
        /// 原产地信息
        /// </summary>      
        public virtual string OriginInformation { get; set; }
        /// <summary>
        /// 物料管制状态
        /// </summary>
        public virtual MaterialControlStatus MaterialControlStatus { get; set; }
        /// <summary>
        /// MOQ
        /// </summary>
        public virtual decimal Moq { get; set; }
        /// <summary>
        /// 累积采购数量
        /// </summary>
        public virtual decimal AccumulatedProcurementQuantity { get; set; }
        /// <summary>
        /// 返点率and年将率and年未税价 集合 (存json)  实体类 UInitPriceFormYearOrValueMode
        /// 原因:为什么不把他们单独存在一个类里,因为单价库中没有唯一值,如果要确定某一条数据需要 物料编码  生效日期  失效日期   梯度/批量起始数量  四个属性确认,这样就会很多产生冗余
        /// </summary>
        public string UInitPriceFormYearOrValueModes { get; set; }

    }
    /// <summary>
    /// 供应商优先级
    /// </summary>
    public enum SupplierPriority
    {
        /// <summary>
        /// 现货/临时
        /// </summary>
        [Description("现货/临时")]
        SpotOrTemporary,
        /// <summary>
        /// 核心
        /// </summary>
        [Description("核心")]
        Core
    }
    /// <summary>
    /// 单价库中冻结状态的值
    /// </summary>
    public enum FreezeOrNot
    {
        /// <summary>
        /// 解冻
        /// </summary>
        [Description("解冻")]
        Thaw,
        /// <summary>
        /// 冻结
        /// </summary>
        [Description("冻结")]
        Frozen
    }
    /// <summary>
    /// 单价库中的报价类型
    /// </summary>
    public enum QuotationType
    {
        /// <summary>
        /// 阶梯
        /// </summary>
        [Description("阶梯")]
        Ladder,
        /// <summary>
        /// 批量
        /// </summary>
        [Description("批量")]
        Batch,
        /// <summary>
        /// 空
        /// </summary>
        [Description("/")]
        Nothing
    }
    /// <summary>
    /// 单价库中的物料管制状态
    /// </summary>
    public enum MaterialControlStatus
    {
        /// <summary>
        /// ECCN
        /// </summary>
        [Description("ECCN")]
        ECCN,
        /// <summary>
        /// EAR99
        /// </summary>
        [Description("EAR99")]
        EAR99,
        /// <summary>
        /// 待定
        /// </summary>
        [Description("待定")]
        Undetermined,
        /// <summary>
        /// 不涉及
        /// </summary>
        [Description("不涉及")]
        NotInvolved,
    }
}
