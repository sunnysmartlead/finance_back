using Abp.Domain.Entities.Auditing;

namespace Finance.MakeOffers;
/// <summary>
/// 报价分析看板中的 样品报价
/// </summary>
public class SampleQuotation : FullAuditedEntity<long>
{
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 方案的id
    /// </summary>
    public long SolutionId { get; set; }
    
    /// <summary>
    /// 样品阶段名称（从字典明细表取值，FinanceDictionaryId是【SampleName】）
    /// </summary>
    public virtual string Name { get; set; }


    /// <summary>
    /// 需求量
    /// </summary>
    public virtual decimal Pcs { get; set; }
    /// <summary>
    /// 成本
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 毛利率
    /// </summary>
    public decimal GrossMargin { get; set; }

    /// <summary>
    /// 销售收入
    /// </summary>
    public decimal SalesRevenue { get; set; }
}