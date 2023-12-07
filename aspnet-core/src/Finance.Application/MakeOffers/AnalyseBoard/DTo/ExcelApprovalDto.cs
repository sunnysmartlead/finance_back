using System.Collections.Generic;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class ExcelApprovalDto
{
    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }

    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long auditFlowId { get; set; }

    /// <summary>
    /// 日期
    /// </summary>
    public string date { get; set; }

    /// <summary>
    /// 记录编号
    /// </summary>
    public string recordNumber { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    public int versions { get; set; }

    /// <summary>
    /// 直接客户名称
    /// </summary>
    public string directCustomerName { get; set; }

    /// <summary>
    /// 终端客户名称
    /// </summary>
    public string terminalCustomerName { get; set; }

    /// <summary>
    /// (报价形式
    /// </summary>
    public string offerForm { get; set; }

    /// <summary>
    /// SOP时间
    /// </summary>
    public int sopTime { get; set; }

    /// <summary>
    /// 项目生命周期
    /// </summary>
    public int projectCycle { get; set; }

    /// <summary>
    /// 销售类型
    /// </summary>
    public string forSale { get; set; }


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
    public string paymentMethod { get; set; }

    /// <summary>
    /// (汇率
    /// </summary>
    public decimal exchangeRate { get; set; }

    /// <summary>
    /// (项目名称
    /// </summary>
    public string projectName { get; set; }

    /// <summary>
    /// (NRE信息
    /// </summary>
    public List<NreExcel> nres { get; set; }

    /// <summary>
    /// (本次报价增加客供料毛利率
    /// </summary>
    public List<PartsSecondModel> componenSocondModels { get; set; }

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
    public List<BiddingStrategySecondModel> biddingStrategySecondModels { get; set; }

    /// <summary>
    /// 走量
    /// </summary>
    public List<SopSecondModel> sop { get; set; }
}