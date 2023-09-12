using System;
using System.Collections.Generic;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 报价表  交互类
/// </summary>
public class QuotationListSecondDto
{
     /// <summary>
        /// 查询日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 记录编号
        /// </summary>
        public string RecordNumber { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int Versions { get; set; }
        /// <summary>
        /// 报价形式
        /// </summary>
        public string OfferForm { get; set; }
        /// <summary>
        /// 样品报价类型
        /// </summary>
        public string SampleQuotationType { get; set; }
        /// <summary>
        /// 直接客户名称
        /// </summary>
        public string DirectCustomerName { get; set; }
        /// <summary>
        /// 客户性质
        /// </summary>
        public string ClientNature { get; set; }
        /// <summary>
        /// 终端客户名称
        /// </summary>
        public string TerminalCustomerName { get; set; }
        /// <summary>
        /// 终端客户性质
        /// </summary>
        public string TerminalClientNature { get; set; }
        /// <summary>
        /// 开发计划
        /// </summary>
        public string DevelopmentPlan { get; set; }
        /// <summary>
        /// Sop时间
        /// </summary>
        public int SopTime { get; set; }
        /// <summary>
        /// 项目声明周期
        /// </summary>
        public int ProjectCycle { get; set; }
        /// <summary>
        /// 销售类型
        /// </summary>
        public string ForSale { get; set; }
        /// <summary>
        /// 贸易方式
        /// </summary>
        public string modeOfTrade { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentMethod { get; set; }
        /// <summary>
        /// 报价币种
        /// </summary>
        public string QuoteCurrency { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }
        /// <summary>
        /// 走量信息
        /// </summary>
        public List<MotionMessageModel> MotionMessage { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// Nre相关
        /// </summary>
        public List<AnalyseBoardNreDto> nres { get; set; }

        /// <summary>
        /// 样品报价
        /// </summary>
        public List<OnlySampleDto> SampleOffer { get; set; }
        /// <summary>
        /// 核心部件
        /// </summary>
        public List<ComponenSocondModel>  componenSocondModels{ get; set; }
        
        /// <summary>
        /// 内部核价信息
        /// </summary>
        public List<PricingMessageSecondModel> pricingMessageSecondModels{ get; set; }
        
        /// <summary>
        /// 报价策略
        /// </summary>
        
        public List<BiddingStrategySecondModel> BiddingStrategySecondModels{ get; set; }
        
}

/// <summary>
/// 报价策略
/// </summary>
public class BiddingStrategySecondModel
{
    /// <summary>
    /// 梯度Id
    /// </summary>
    public virtual long GradientId { get; set; }
    /// <summary>
    /// 梯度Id
    /// </summary>
    public virtual string gradient { get; set; }
    /// <summary>
    /// 模组数量主键
    /// </summary>
    public long ProductId { get; set; }
    /// <summary>
    /// 产品
    /// </summary>
    public string Product { get; set; }         
    /// <summary>
    /// Sop年成本
    /// </summary>
    public decimal SopCost { get; set; }
    /// <summary>
    /// 全生命周期成本
    /// </summary>
    public decimal FullLifeCyclecost { get; set; }
    /// <summary>
    /// 销售收入
    /// </summary>
    public decimal SalesRevenue { get; set; }
    /// <summary>
    /// 销售成本
    /// </summary>
    public decimal SellingCost { get; set; }
    /// <summary>
    /// 毛利率
    /// </summary>
    public decimal GrossMargin { get; set; }
    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// 佣金
    /// </summary>
    public decimal Commission { get; set; }
    /// <summary>
    /// Sop年毛利率
    /// </summary>
    public decimal SopGrossMargin { get; set; }
    /// <summary>
    /// 含佣金的毛利率
    /// </summary>
    public decimal GrossMarginCommission { get; set; }
    
    /// <summary>
    /// 全生命周期毛利率
    /// </summary>
    public decimal TotallifeCyclegrossMargin{ get; set; }
    
    /// <summary>
    /// 增加客供料毛利率
    /// </summary>
    public decimal ClientGrossMargin { get; set; }
    /// <summary>
    /// 剔除NRE分摊费用毛利率
    /// </summary>
    public decimal NreGrossMargin { get; set; }
}

/// <summary>
/// 总经理审批——报价策略
/// </summary>
public class ManagerApprovalOfferModel
{
 
    /// <summary>
    /// 产品
    /// </summary>
    public string Product { get; set; }         
    /// <summary>
    /// Sop年成本
    /// </summary>
    public decimal SopCost { get; set; }
    /// <summary>
    /// 全生命周期成本
    /// </summary>
    public decimal FullLifeCyclecost { get; set; }
    /// <summary>
    /// 销售收入
    /// </summary>
    public decimal SalesRevenue { get; set; }
    /// <summary>
    /// 销售成本
    /// </summary>
    public decimal SellingCost { get; set; }
    /// <summary>
    /// 毛利率
    /// </summary>
    public decimal GrossMargin { get; set; }
    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// 佣金
    /// </summary>
    public decimal Commission { get; set; }
    /// <summary>
    /// Sop年毛利率
    /// </summary>
    public decimal SopGrossMargin { get; set; }
    /// <summary>
    /// 含佣金的毛利率
    /// </summary>
    public decimal GrossMarginCommission { get; set; }
    /// <summary>
    /// 增加客供料毛利率
    /// </summary>
    public decimal ClientGrossMargin { get; set; }
    /// <summary>
    /// 剔除NRE分摊费用毛利率
    /// </summary>
    public decimal NreGrossMargin { get; set; }
}