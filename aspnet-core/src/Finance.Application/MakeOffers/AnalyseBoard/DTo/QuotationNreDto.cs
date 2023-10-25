namespace Finance.MakeOffers.AnalyseBoard.DTo;
//
/// <summary>
/// NRE报价清单
/// </summary>
public class QuotationNreDto
{
    /// <summary>
    /// 产品
    /// </summary>
   
    public virtual string Product { get; set; }
    /// <summary>
    /// 需求量
    /// </summary>
    public decimal Pcs { get; set; }
    /// <summary>
    /// 手板件费
    /// </summary>
    public decimal shouban { get; set; }
    /// <summary>
    /// 模具费
    /// </summary>
    public decimal moju { get; set; }
    /// <summary>
    /// 工装治具费
    /// </summary>
    public decimal gzyj { get; set; }

    /// <summary>
    /// 实验费
    /// </summary>
    public decimal sy { get; set; }
    /// <summary>
    /// 测试软件费
    /// </summary>
    public decimal csrj { get; set; }
    /// <summary>
    /// 差旅费
    /// </summary>
    public decimal cl { get; set; }
    /// <summary>
    /// 其他费用
    /// </summary>
    public decimal qt { get; set; }
    
}