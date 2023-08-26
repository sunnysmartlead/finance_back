using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 总经理报价审批
/// </summary>
public class ManagerApprovalOfferDto
{
    public List<OnlySampleDto> onlySampleDto { get; set; }
    public AnalyseBoardNreDto analyseBoardNreDto { get; set; }
    
}