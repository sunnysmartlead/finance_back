namespace Finance.MakeOffers.AnalyseBoard.Model;

public class GradientMarginModel
{
    /// <summary>
    /// 梯度
    /// </summary>
    public string gradient { get; set; }
    
    
}

public class GradientMarginprojectModel
{
    /// <summary>
    /// 产品
    /// </summary>
    public string item { get; set; }
    public QuotedGrossMarginSimple QuotedGrossMarginSimple { get; set; }

}