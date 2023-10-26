namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class GrossMarginSecondDto
{
    /// <summary>
    /// 毛利率
    /// </summary>
    public decimal GrossMargin { get; set; }
    /// <summary>
    /// 增加客供料毛利率
    /// </summary>
    public decimal ClientGrossMargin { get; set; }
    /// <summary>
    /// 剔除NRE分摊费用毛利率
    /// </summary>
    public decimal NreGrossMargin { get; set; }
    
    
}