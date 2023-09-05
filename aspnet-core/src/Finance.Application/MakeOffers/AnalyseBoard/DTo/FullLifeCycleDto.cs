using System.Collections.Generic;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class FullLifeCycleDto
{
    /// <summary>
    /// 项目名
    /// </summary>
    public string itemName { get; set; }
    public List<GrossMarginModel> grosss{ get; set; }
}