using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class NREUnitSumModel
{
    /// <summary>
    /// 产品
    /// </summary>
    public string Product { get; set; }    
    
    
    public List<SolutuionAndValue>  solutuionAndValues{ get; set; }   
}