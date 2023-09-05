using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;
/// <summary>
/// 单价汇总
/// </summary>
public class UnitPriceSumModel
{
    /// <summary>
    /// 产品
    /// </summary>
    public string Product { get; set; }    
    
    
    public List<SolutuionAndValue>  solutuionAndValues{ get; set; }   
}

/// <summary>
/// 单价汇总
/// </summary>
public class SolutuionAndValue
{
    /// <summary>
    /// 方案id
    /// </summary>
    public long? SolutionId { get; set; }
    /// <summary>
    /// 值
    /// </summary>
    public decimal? value { get; set; }
}

