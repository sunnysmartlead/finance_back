using Finance.MakeOffers.AnalyseBoard.Model;
using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.DTo
{
    /// <summary>
    /// 审批 报价表  交互类
    /// </summary>
    public class AuditQuotationListDto: QuotationListDto
    {
        /// <summary>
        /// 流程号Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 是否审批通过 0 否 1 是
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 审批说明
        /// </summary>
        public string BackReason { get; set; }
    }
    /// <summary>
    /// 报价表  交互类
    /// </summary>
    public class QuotationListDto
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
        /// 核心部件
        /// </summary>
        public List<CoreComponentModel> CoreComponent { get; set; }
        /// <summary>
        /// Nre费用
        /// </summary>
        public List<NreCostMessageModel> NreCost { get; set; }
        /// <summary>
        /// 内部核价信息
        /// </summary>
        public List<PricingMessage> PricingMessage { get; set; }
        /// <summary>
        /// 报价策略
        /// </summary>
        public List<BiddingStrategyModel> BiddingStrategy { get; set; }
        /// <summary>
        /// 费用表
        /// </summary>
        public List<ExpensesStatementModel> ExpensesStatement { get; set; }
    }

    /// <summary>
    /// 走量信息 模型
    /// </summary>
    public class MotionMessageModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string MessageName { get; set; }
        /// <summary>
        /// Sop
        /// </summary>
        public List<SopOrValueMode> Sop { get; set; }
    }
    /// <summary>
    /// 核心部件 模型
    /// </summary>
    public class CoreComponentModel
    {
        /// <summary>
        /// 模组数量主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 部件名称
        /// </summary>
        public string ComponentName { get; set; }
        /// <summary>
        /// 零件种类模型
        /// </summary>
        public List<ComponenModel> ProductSubclass { get; set; }
    }
    /// <summary>
    /// 零件 模型
    /// </summary>
    public class ComponenModel
    {
        /// <summary>
        /// 核心部件
        /// </summary>
        public string CoreComponent { get; set; }
        /// <summary>
        ///型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
    /// <summary>
    /// Nre费用信息 模型
    /// </summary>
    public class NreCostMessageModel
    {
        /// <summary>
        /// 模组数量主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// Nre 费用 模组名称
        /// </summary>
        public string NreCostModuleName { get; set; }
        /// <summary>
        /// Nre费用 模型
        /// </summary>
        public List<NreCostModel> NreCostModels { get; set; }
    }
    /// <summary>
    /// Nre费用 模型
    /// </summary>
    public class NreCostModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal Cost { get; set; }
    }
    /// <summary>
    /// 价信息 模型
    /// </summary>
    public class PricingMessage
    {
        /// <summary>
        /// 模组数量主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 内部核价信息 模组名称
        /// </summary>
        public string PricingMessageName { get; set; }
        /// <summary>
        /// 内部核价信息 模型
        /// </summary>
        public List<PricingMessageModel> PricingMessageModels { get; set; }
    }
    /// <summary>
    /// 内部核价信息 模型
    /// </summary>
    public class PricingMessageModel
    {  
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 成本值
        /// </summary>
        public decimal CostValue { get; set; }
    }
    /// <summary>
    /// 报价策略 模型
    /// </summary>
    public class BiddingStrategyModel
    {
        /// <summary>
        /// 模组数量主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 产品
        /// </summary>
        public string Product { get; set; }         
        /// <summary>
        /// 成本
        /// </summary>
        public decimal Cost { get; set; }
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
        /// 含佣金的毛利率
        /// </summary>
        public decimal GrossMarginCommission { get; set; }
    }
    /// <summary>
    /// 费用表 模型
    /// </summary>
    public class ExpensesStatementModel
    {
        /// <summary>
        /// 表单名称  如{手板件费用,模具费,生产设备费}
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// 核价金额
        /// </summary>
        public decimal PricingMoney { get; set; }
        /// <summary>
        /// 报价系数
        /// </summary>
        public decimal OfferCoefficient { get; set; }
        /// <summary>
        /// 报价金额
        /// </summary>
        public decimal OfferMoney { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
