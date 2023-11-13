using System.Collections.Generic;

namespace Finance.MakeOffers.AnalyseBoard.Model;

public class NRESecondModel
{
  
    /// <summary>
    /// 费用名称
    /// </summary>
    public string SolutionName { get; set; }
 
    /// <summary>
    /// 手板件费
    /// </summary>
    public decimal shouban { get; set; }
    /// <summary>
    /// 模具费
    /// </summary>
    public decimal moju { get; set; }
    /// <summary>
    /// 生产设备费
    /// </summary>
    public decimal scsb { get; set; }
    /// <summary>
    /// 工装费
    /// </summary>
    public decimal gz { get; set; }
    /// <summary>
    /// 治具费
    /// </summary>
    public decimal yj { get; set; }

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
    /// 检具费
    /// </summary>
    public decimal jianju { get; set; }
    /// <summary>
    /// 其他费用
    /// </summary>
    public decimal qt { get; set; }

}