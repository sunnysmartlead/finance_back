using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

/// <summary>
/// 内部核价信息 
/// </summary>
public class PricingMessageSecondModel
{
    /// <summary>
    /// 方案名称
    /// </summary>
    public string SolutionName { get; set; }
    /// <summary>
    /// 方案id
    /// </summary>
    public long SolutionId { get; set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    
    public List<SopOrAll> sops { get; set; }
}
/// <summary>
/// 梯度(K/Y) sop 全年
/// </summary>
public class SopOrAll
{
    /// <summary>
    /// 梯度(K/Y)
    /// </summary>
    public  decimal GradientValue { get; set; }
    /// <summary>
    /// sop
    /// </summary>
    public  decimal sop { get; set; }
    /// <summary>
    /// 全生命周期
    /// </summary>
    public  decimal all { get; set; }
    
}
