using System.Collections.Generic;
using Finance.Nre;
using Finance.NrePricing.Dto;

namespace Finance.MakeOffers.AnalyseBoard.DTo;

public class AnalyseBoardNreDto
{
    /// <summary>
    /// 方案名
    /// </summary>
    public string solutionName { get; set; }
    /// <summary>
    /// 方案表ID
    /// </summary>
    public long? SolutionId { get; set; }
    /// <summary>
    /// 流程号Id
    /// </summary> 
    public long AuditFlowId { get; set; }
    public List<NreQuotation> models{ get; set; }
    public List<DeviceQuotation> devices{ get; set; }
    
    /// <summary>
    /// 线体数量
    /// </summary>
    public decimal numberLine { get; set; }
    /// <summary>
    /// 共线分摊率
    /// </summary>
    public decimal collinearAllocationRate { get; set; }
}

public class AnalyseBoardNreModel
{  
    /// <summary>
    ///序号
    /// </summary>
    public virtual int Index { get; set; }
    /// <summary>
    ///费用名称
    /// </summary>
       public string CostName { get; set; }
    /// <summary>
    /// 核价金额
    /// </summary>
    public string PricingMoney { get; set; }
    /// <summary>
    /// 报价系数
    /// </summary>
    public decimal OfferCoefficient { get; set; }
    /// <summary>
    /// 报价金额
    /// </summary>
    public decimal OfferMoney { get; set; }
    /// <summary>
    /// 备注
    /// </summary>
    public decimal Remark { get; set; }
    
}

public class DeviceModel
{
    /// <summary>
    ///设备名称
    /// </summary>
    public string DeviceName { get; set; }
    /// <summary>
    /// 设备单价
    /// </summary>
    public decimal DevicePrice{ get; set; }
    /// <summary>
    /// 设备数量
    /// </summary>
    public decimal Number { get; set; }
    /// <summary>
    /// 设备金额
    /// </summary>
    public decimal equipmentMoney { get; set; }
    
    
}