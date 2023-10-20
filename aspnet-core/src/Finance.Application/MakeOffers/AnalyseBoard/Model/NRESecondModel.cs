using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class NRESecondModel
{
    /// <summary>
    /// 名称
    /// </summary>
    public string NreName { get; set; }
    /// <summary>
    /// 费用名称
    /// </summary>
    public string CostName { get; set; }
 
    /// <summary>
    /// 费用
    /// </summary>
    public List<string> Costs { get; set; }

    /// <summary>
    /// 总费用
    /// </summary>
    public decimal? Cost { get; set; }

}