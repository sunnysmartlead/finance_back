namespace Finance.MakeOffers.AnalyseBoard.Model;

public class GradientQuotedGrossMarginModel:QuotedGrossMarginModel
{
    /// <summary>
    /// 梯度
    /// </summary>
    public string gradient { get; set; }
    /// <summary>
    /// 梯度Id
    /// </summary>
    public long gradientId { get; set; }
}