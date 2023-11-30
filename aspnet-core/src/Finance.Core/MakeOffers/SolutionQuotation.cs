using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Finance.MakeOffers;
/// <summary>
/// 报价分析看板中的 报价方案表
/// </summary>
public class SolutionQuotation: FullAuditedEntity<long>
{
    /// <summary>
    /// 流程Id
    /// </summary>
    public virtual long AuditFlowId { get; set; }
    /// <summary>
    /// 方案的id
    /// </summary>
    public virtual long SolutionId { get; set; }
    /// <summary>
    /// 附件id
    /// </summary>
    public long Productld { get; set; }
    /// <summary>
    /// 模组名称
    /// </summary>
    public string ModuleName { get; set; }
    /// <summary>
    /// 方案名称
    /// </summary>
    public string SolutionName { get; set; }
    /// <summary>
    /// 附件url
    /// </summary>
    public string Product { get; set; }
    /// <summary>
    /// 报价反馈仅保存
    /// </summary>
    public bool IsCOB { get; set; }
    /// <summary>
    /// 电子工程师(用户ID)
    /// </summary>
    public long ElecEngineerId { get; set; }
    /// <summary>
    /// 结构工程师(用户ID)
    /// </summary>
    public long StructEngineerId { get; set; }
    /// <summary>
    /// 仅保存
    /// </summary>
    public bool IsFirst { get; set; }
    /// <summary>
    /// 线体数量
    /// </summary>
    public decimal numberLine { get; set; }
    /// <summary>
    /// 共线分摊率
    /// </summary>
    public decimal collinearAllocationRate { get; set; }
    /// <summary>
    /// 启用状态，0启用，1不用\
    /// </summary>
    public int status { get; set; }
    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }
    
    /// <summary>
    /// 报价次数
    /// </summary>
    public int ntime { get; set; }
    
    /// <summary>
    /// 方案list
    /// </summary>
    [Column(TypeName = "CLOB")]
    public string SolutionListJson { get; set; }
}