using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class NREUnitSumModel
{
    /// <summary>
    /// 产品
    /// </summary>
    public string Product { get; set; }    
    
    /// <summary>
    /// 核价成本
    /// </summary>
    public decimal cost{ get; set; }  
    /// <summary>
    /// 报价金额
    /// </summary>
    public decimal number{ get; set; }  
}