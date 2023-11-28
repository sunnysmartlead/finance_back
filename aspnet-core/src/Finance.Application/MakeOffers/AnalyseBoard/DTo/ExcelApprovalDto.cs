using System.Collections.Generic;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class ExcelApprovalDto
{
    /// <summary>
    /// 日期
    /// </summary>
    public string Date { get; set; }

    /// <summary>
    /// 记录编号
    /// </summary>
    public string RecordNumber { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    public int Versions { get; set; }

    /// <summary>
    /// 直接客户名称
    /// </summary>
    public string DirectCustomerName { get; set; }

    /// <summary>
    /// 终端客户名称
    /// </summary>
    public string TerminalCustomerName { get; set; }

    /// <summary>
    /// (报价形式
    /// </summary>
    public string OfferForm { get; set; }

    /// <summary>
    /// SOP时间
    /// </summary>
    public int SopTime { get; set; }

    /// <summary>
    /// 项目生命周期
    /// </summary>
    public int ProjectCycle { get; set; }

    /// <summary>
    /// 销售类型
    /// </summary>
    public string ForSale { get; set; }

   
    
    /// <summary>
    /// 币种
    /// </summary>
    public string bz { get; set; }
    /// <summary>
    /// 贸易方式
    /// </summary>
    public string modeOfTrade { get; set; }

    /// <summary>
    /// 付款方式
    /// </summary>
    public string PaymentMethod { get; set; }

    /// <summary>
    /// (汇率
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// (项目名称
    /// </summary>
    public string ProjectName { get; set; }
 /// <summary>
    /// (NRE信息
    /// </summary>
    public List<NreExcel> nres { get; set; }
    /// <summary>
    /// (本次报价增加客供料毛利率
    /// </summary>
    public  List<PartsSecondModel> componenSocondModels { get; set; }

    /// <summary>
    /// (内部核价
    /// </summary>
    public List<PricingSecondModel> pricingMessageSecondModels { get; set; }

    /// <summary>
    /// 样品阶段
    /// </summary>
    public List<SampleExcel> samples { get; set; }

    /// <summary>
    /// 梯度走量
    /// </summary>
    public List<MotionGradientSecondModel> mess { get; set; }

    /// <summary>
    /// (报价策略
    /// </summary>
    public  List<BiddingStrategySecondModel> BiddingStrategySecondModels { get; set; }

    /// <summary>
    /// 走量
    /// </summary>
    public  List<SopSecondModel> Sop { get; set; }
}