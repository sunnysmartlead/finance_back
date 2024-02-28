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
    /// <summary>
    /// 价格CNY
    /// </summary>
    public decimal price{ get; set; }
    /// <summary>
    /// 价格USD
    /// </summary>
    public decimal priceUSD { get; set; }
    /// <summary>
    /// 汇率
    /// </summary>
    public decimal ExchangeRate { get; set; }
}



