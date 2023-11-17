using Abp.Domain.Entities.Auditing;

namespace Finance.MakeOffers;
/// <summary>
/// 报价毛利率测算-阶梯数量
/// </summary> 
public class GradientGrossCalculate : FullAuditedEntity<long>
{
    /// <summary>
    /// 梯度
    /// </summary>
    public string gradient { get; set; }
    /// <summary>
    /// 梯度Id
    /// </summary>
    public virtual long GradientId { get; set; }
    /// <summary>
    /// 方案的id
    /// </summary>
    public long SolutionId { get; set; }
    /// <summary>
    /// 产品
    /// </summary>
    public string product { get; set; }
 
    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }
    /// <summary>
    /// 流程号Id
    /// </summary>
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 目标价（内部）单价
    /// </summary>
    public decimal InteriorPrice{ get; set; }
    /// <summary>
    /// 目标价（内部）毛利率
    /// </summary>
    public decimal InteriorGrossMargin{ get; set; }
    /// <summary>
    /// 目标价（内部）增加客供料毛利率
    /// </summary>
    public decimal InteriorClientGrossMargin{ get; set; }
    /// <summary>
    /// 目标价（内部）剔除分摊费用毛利率
    /// </summary>
    public decimal InteriorNreGrossMargin{ get; set; }
    
    /// <summary>
    /// 目标价（客户）单价
    /// </summary>
    public decimal ClientPrice{ get; set; }
    /// <summary>
    /// 目标价（客户）毛利率
    /// </summary>
    public decimal ClientGrossMargin{ get; set; }
    /// <summary>
    /// 目标价（客户）增加客供料毛利率
    /// </summary>
    public decimal ClientClientGrossMargin{ get; set; }
    /// <summary>
    /// 目标价（客户）剔除分摊费用毛利率
    /// </summary>
    public decimal ClientNreGrossMargin{ get; set; }
    /// <summary>
    /// 本次报价-单价
    /// </summary>
    public decimal OfferUnitPrice { get; set; }
    /// <summary>
    /// 本次报价-毛利率
    /// </summary>
    public decimal OfferGrossMargin { get; set; }
    /// <summary>
    /// (本次报价增加客供料毛利率
    /// </summary>
    public decimal OfferClientGrossMargin { get; set; }
    /// <summary>
    /// 本次报价剔除NRE分摊费用毛利率
    /// </summary>
    public decimal OfferNreGrossMargin { get; set; }
    /// <summary>
    /// 类别  0报价分析看板，1报价反馈
    /// </summary>
    public int ntype { get; set; }

}