using Abp.Domain.Entities.Auditing;

namespace Finance.MakeOffers;

public class ProjectBoardSecondOffers: FullAuditedEntity<long>
{
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 项目名称
    /// </summary>
    public string ProjectName { get; set; }
    /// <summary>
    /// 梯度Id
    /// </summary>
    public virtual long GradientId { get; set; }
    /// <summary>
    /// 目标价(内部) 
    /// </summary>
    public decimal InteriorTarget { get; set; }
    /// <summary>
    /// 目标价(客户)
    /// </summary>
    public decimal ClientTarget { get; set; }
    /// <summary>
    /// 本次报价存 
    /// </summary>
    public decimal Offer { get; set; }    
    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }    
    /// <summary>
    /// 标题
    /// </summary>
    public string title { get; set; }
    /// <summary>
    /// 类别  0报价分析看板，1报价反馈
    /// </summary>
    public int ntype { get; set; }
}