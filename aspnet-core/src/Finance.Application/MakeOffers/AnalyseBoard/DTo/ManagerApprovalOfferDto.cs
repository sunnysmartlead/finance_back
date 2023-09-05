using System.Collections.Generic;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 总经理报价审批
/// </summary>
public class ManagerApprovalOfferDto
{

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
}
/// <summary>
/// 总经理报价审批
/// </summary>
public class ManagerApprovalOfferNre
{
    /// <summary>
    /// Nre相关
    /// </summary>
    public AnalyseBoardNreDto analyseBoardNreDto{ get; set; }

    public List<ManagerApprovalOfferModel> ManagerApprovalOfferModels{ get; set; }
}
