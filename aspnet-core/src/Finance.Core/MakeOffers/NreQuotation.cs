﻿using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Finance.Nre;
/// <summary>
/// Nre 报价NRE 实体类
/// </summary>
public class NreQuotation: FullAuditedEntity<long>
{
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 方案的id
    /// </summary>
    public long? SolutionId { get; set; }
    /// <summary>
    /// 表单名称  如{手板件费用,模具费,生产设备费}
    /// </summary>
    public string FormName { get; set; }
    /// <summary>
    /// 核价金额
    /// </summary>
    public decimal PricingMoney { get; set; }
    /// <summary>
    /// 报价系数
    /// </summary>        
    [Column(TypeName = "decimal(18,4)")]
    public decimal OfferCoefficient { get; set; }
    /// <summary>
    /// 报价金额
    /// </summary>
    public decimal OfferMoney { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
    /// <summary>
    /// 线体数量
    /// </summary>
    public decimal numberLine { get; set; }
    /// <summary>
    /// 共线分摊率
    /// </summary>
    public decimal collinearAllocationRate { get; set; }
    /// <summary>
    /// 版本
    /// </summary>
    public int version { get; set; }
    /// <summary>
    /// 类别  0报价分析看板，1报价反馈
    /// </summary>
    public int ntype { get; set; }
}