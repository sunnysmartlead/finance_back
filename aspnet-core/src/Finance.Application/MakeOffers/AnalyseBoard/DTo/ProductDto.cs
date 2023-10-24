namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 产品报价清单
/// </summary>
public class ProductDto
{
    /// <summary>
    /// 产品
    /// </summary>
   
    public virtual string ProductName { get; set; }
    /// <summary>
    /// 走量
    /// </summary>
    public decimal Motion { get; set; }
    /// <summary>
    /// 年份
    /// </summary>
    public string Year { get; set; }
    /// <summary>
    /// 单价
    /// </summary>
    public string UntilPrice { get; set; }
}