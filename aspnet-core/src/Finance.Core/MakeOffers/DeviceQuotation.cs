using Abp.Domain.Entities.Auditing;

namespace Finance.MakeOffers;
/// <summary>
/// 报价分析看板中的 NRE设备
/// </summary>
public class DeviceQuotation: FullAuditedEntity<long>
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