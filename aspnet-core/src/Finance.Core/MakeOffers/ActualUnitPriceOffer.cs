using Abp.Domain.Entities.Auditing;

namespace Finance.MakeOffers;

/// <summary>
/// 报价毛利率测算-阶梯数量
/// </summary> 
public class ActualUnitPriceOffer : FullAuditedEntity<long>
{
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }

    /// <summary>
    /// 梯度
    /// </summary>       
    public virtual string Kv { get; set; }
    /// <summary>
    /// 产品名称
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// 梯度
    /// </summary>
    public string gradient { get; set; }
    /// <summary>
    /// 目标价(内部) 存json 实体类GrossMarginModel
    /// </summary>
    public string InteriorTarget { get; set; }
    /// <summary>
    /// 目标价(客户)存json 实体类GrossMarginModel
    /// </summary>
    public string ClientTarget { get; set; }
    /// <summary>
    /// 本次报价存  json 实体类GrossMarginModel 
    /// </summary>
    public string Offer { get; set; }    
}