using System.Collections.Generic;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 总经理报价审批
/// </summary>
public class ManagerApprovalOfferDto
{
    /// <summary>
    /// 是否仅含样品
    /// </summary>
    public bool issample { get; set; }
    /// <summary>
    /// 单价汇总
    /// </summary>
    public List<UnitPriceSumModel> UnitPriceSum{ get; set; }
    /// <summary>
    /// Nre汇总
    /// </summary>
    public List<NREUnitSumModel> NreUnitSumModels{ get; set; }
    /// <summary>
    /// 报价毛利率测算-实际数量
    /// </summary>
    public List<ManagerApprovalOfferNre> ManagerApprovalOfferNres{ get; set; }
    /// <summary>
    /// NRE
    /// </summary>
    public  AnalyseBoardNreDto nre{get; set;}
    /// <summary>
    /// 样品报价
    /// </summary>
    public List<OnlySampleDto> SampleOffer { get; set; }

}
/// <summary>
/// 总经理报价审批
/// </summary>
public class ManagerApprovalOfferNre
{
    /// <summary>
    /// 方案名
    /// </summary>
    public string solutionName { get; set; }
    /// <summary>
    /// 方案表ID
    /// </summary>
    public long SolutionId { get; set; }
    /// <summary>
    /// 本次报价-单价
    /// </summary>
    public decimal OfferUnitPrice { get; set; }
    /// <summary>
    /// 本次报价-毛利率
    /// </summary>
    public decimal OfferGrossMargin { get; set; }
    /// <summary>
    /// (本次报价增加客供料毛利率
    /// </summary>
    public decimal OfferClientGrossMargin { get; set; }
    /// <summary>
    /// 本次报价剔除NRE分摊费用毛利率
    /// </summary>
    public decimal OfferNreGrossMargin { get; set; }
    /// <summary>
    /// 销售收入
    /// </summary>
    public decimal SalesRevenue { get; set; }

    /// <summary>
    /// 销售成本
    /// </summary>
    public decimal SellingCost { get; set; }
    /// <summary>
    /// SOP成本
    /// </summary>
    public decimal SopCost { get; set; }
    /// <summary>
    /// 全生命周期成本
    /// </summary>
    public decimal fullCost { get; set; }
  
}
