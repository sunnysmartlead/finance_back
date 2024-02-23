using System.Collections.Generic;
using Finance.DemandApplyAudit;
using Finance.Dto;
using Finance.MakeOffers.AnalyseBoard.Model;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class IsOfferSecondDto:ToExamineDto
{
    /// <summary>
    /// 是否报价 true/1 是  false/0 否
    /// </summary>
    public bool IsOffer { get; set; }
    /// <summary>
    /// 不报价原因
    /// </summary>
    public string NoOfferReason { get; set; }
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }

    /// <summary>
    /// 附件id
    /// </summary>
    public long ProductId { get; set; }
    /// <summary>
    /// 附件url
    /// </summary>
    public string Product { get; set; }
    /// <summary>
    /// 附件名称
    /// </summary>
    public string ProductName { get; set; }
    /// <summary>
    /// Nre相关
    /// </summary>
    public List<AnalyseBoardNreDto> nres { get; set; }

    /// <summary>
    /// 样品报价
    /// </summary>
    public List<OnlySampleDto> SampleOffer { get; set; }

    /// <summary>
    /// SOP单价表（SOP年）
    /// </summary>
    public List<SopAnalysisModel> Sops { get; set; }
    /// <summary>
    /// 项目看板
    /// </summary>
    public List<BoardModel> ProjectBoard { get; set; }

    /// <summary>
    /// 报价毛利率测算 实际数量
    /// </summary>
    public List<QuotedGrossMarginActualModel> QuotedGrossMargins { get; set; }

    /// <summary>
    /// 报价毛利率测算阶梯数量
    /// </summary>
    public List<GradientGrossMarginCalculateModel> GradientQuotedGrossMargins { get; set; }

    /// <summary>
    /// 项目全生命周期汇总分析表-实际数量
    /// </summary>
    public List<PooledAnalysisModel> FullLifeCycle { get; set; }

    /// <summary>
    /// 报价方案
    /// </summary>
    public List<Solution> Solutions{ get; set; }
    /// <summary>
    /// 版本
    /// </summary>
    public int version{ get; set; }
    /// <summary>
    /// 报价次数
    /// </summary>
    public int ntime { get; set; }
    /// <summary>
    /// 0 报价分析看板，1 报价反馈
    /// </summary>
    public int ntype { get; set; }
    /// <summary>
    /// 仅保存
    /// </summary>
    public bool IsFirst { get; set; }
}

public class IsDeleteSecondDto
{
    /// <summary>
    /// 仅保存
    /// </summary>
    public bool IsFirst { get; set; }
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 版本
    /// </summary>
    public int version{ get; set; }
}