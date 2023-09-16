using System.Collections.Generic;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.Method;

public class AcceptanceBidDto
{
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