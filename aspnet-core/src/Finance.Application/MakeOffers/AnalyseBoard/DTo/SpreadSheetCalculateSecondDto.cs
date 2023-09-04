namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class SpreadSheetCalculateSecondDto
{
    /// <summary>
    /// 流程号Id
    /// </summary>
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 梯度Id
    /// </summary>
    public virtual long GradientId { get; set; }
    /// <summary>
    /// MoudelCount表id
    /// </summary>
    
    public long ProductId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long? SolutionId { get; set; }
}