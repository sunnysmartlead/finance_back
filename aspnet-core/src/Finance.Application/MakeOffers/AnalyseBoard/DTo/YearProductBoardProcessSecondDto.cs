using Spire.Pdf.Exporting.XPS.Schema;
using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.DTo;
/// <summary>
/// 年份维度对比 动态单价表计算 交互类 二开
/// </summary>
public class YearProductBoardProcessSecondDto
{
    /// <summary>
    /// 流程表id
    /// </summary>
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 毛利率
    /// </summary>
    public decimal GrossMargin { get; set; }

}

/// <summary>
/// 年份维度对比 动态单价表计算 交互类 二开
/// </summary>
public class YearProductBoardProcessSecond_DynamicProductDto
{
    /// <summary>
    /// 流程表id
    /// </summary>
    public long AuditFlowId { get; set; }
    /// <summary>
    /// 毛利率
    /// </summary>
    public decimal GrossMargin { get; set; }
    /// <summary>
    /// 动态单价表计算 交互类
    /// </summary>
    public List<DynamicProductBoardModelSecond> Boards { get; set; }
}
/// <summary>
/// 动态单价表计算 模型
/// </summary>
public class DynamicProductBoardModelSecond
{
    /// <summary>
    /// 模组数量主键
    /// </summary>
    public long ProductId { get; set; }
    /// <summary>
    /// 梯度数量主键 若无需获取对应梯度
    /// 此行不传
    /// </summary>
    public long? GradientId { get; set; }
    /// <summary>
    /// 本次报价-单价
    /// </summary>
    public decimal UnitPrice { get; set; }
}