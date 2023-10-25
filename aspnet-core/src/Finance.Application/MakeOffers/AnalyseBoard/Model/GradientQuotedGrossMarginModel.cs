namespace Finance.MakeOffers.AnalyseBoard.Model;

public class GradientQuotedGrossMarginModel:QuotedGrossMarginModel
{
    /// <summary>
    /// 梯度
    /// </summary>
    public string gradient { get; set; }
    
    
    /// <summary>
    /// 数据id
    /// </summary>
    public long Id{ get; set; }
    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }
}