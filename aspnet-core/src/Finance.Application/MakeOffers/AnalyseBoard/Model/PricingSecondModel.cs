using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class PricingSecondModel
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 走量
    /// </summary>
    public string pcs{ get; set; }
    /// <summary>
    /// 费用
    /// </summary>
    public List<string> Costs { get; set; }
}