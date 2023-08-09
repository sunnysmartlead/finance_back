using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment.Dto
{
    public class WorkingHourDto
    {
        /// <summary>
        /// 审批流程表ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// ModelCount表id
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 工时逐行数据
        /// </summary>
        public List<WorkingHourDetail> WorkingHourDetailList { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 设备部分-最多设备数
        /// </summary>
        public int EquipmentNum { get; set; }
        /// <summary>
        /// 追溯部分-最多设备数
        /// </summary>
        public int RetrospectNum { get; set; }
        /// <summary>
        /// 工装部分-最多设备数
        /// </summary>
        public int ToolingNum { get; set; }
        /// <summary>
        /// SOP循环数
        /// </summary>
        public int SOPNum { get; set; }

    }

    public class WorkingHourDetail
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int SequenceNumber { get; set; }
        /// <summary>
        /// 工序
        /// </summary>
        public string Procedure { get; set; }
        /// <summary>
        /// 设备部分
        /// </summary>
        public EquipmentPart EquipmentPart { get; set; }
        /// <summary>
        /// 追溯部分（硬件及软件开发费用）
        /// </summary>
        public RetrospectPart RetrospectPart { get; set; }
        /// <summary>
        /// 工装治具部分
        /// </summary>
        public ToolingFixturePart ToolingFixturePart { get; set; }
        /// <summary>
        /// 工时
        /// </summary>
        public List<HumanMachineHoursDetail> HumanMachineHoursDetailList { get; set; }

    }
    /// <summary>
    /// 设备部分
    /// </summary>
    public class EquipmentPart
    {
        /// <summary>
        /// 设备信息
        /// </summary>
        public List<EquipmentDetail> EquipmentDetails { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public double Total { get; set; }

    }

    /// <summary>
    /// 追溯部分（硬件及软件开发费用）
    /// </summary>
    public class RetrospectPart
    {
        /// <summary>
        /// 设备信息
        /// </summary>
        public List<EquipmentDetail> EquipmentDetails { get; set; }
        /// <summary>
        /// 硬件总价
        /// </summary>
        public double HardwareTotal { get; set; }
        /// <summary>
        /// 追溯软件
        /// </summary>
        public string RetrospectSoftware { get; set; }
        /// <summary>
        /// 开发费-追溯
        /// </summary>
        public double RetrospectFee { get; set; }
        /// <summary>
        /// 开图软件
        /// </summary>
        public string OpenGraphSoftware { get; set; }
        /// <summary>
        /// 开发费-开图
        /// </summary>
        public double OpenGraphFee { get; set; }
        /// <summary>
        /// 软硬件总价
        /// </summary>
        public double Total { get; set; }
    }

    /// <summary>
    /// 工装治具部分
    /// </summary>
    public class ToolingFixturePart
    {
        /// <summary>
        /// 设备信息
        /// </summary>
        public List<EquipmentDetail> EquipmentDetails { get; set; }
        /// <summary>
        /// 工装名称
        /// </summary>
        public string ToolingName { get; set; }
        /// <summary>
        /// 工装数量
        /// </summary>
        public int ToolingNum { get; set; }
        /// <summary>
        /// 工装单价
        /// </summary>
        public double ToolingPrice { get; set; }
        /// <summary>
        /// 测试线名称
        /// </summary>
        public string TestName { get; set; }
        /// <summary>
        /// 测试线数量
        /// </summary>
        public int TestNum { get; set; }
        /// <summary>
        /// 测试线单价
        /// </summary>
        public double TestPrice { get; set; }
        /// <summary>
        /// 工装治具总价
        /// </summary>
        public double Total { get; set; }
    }

    public class HumanMachineHoursDetail
    {
        /// <summary>
        /// 年
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 标准人工工时
        /// </summary>
        public double LaborTime { get; set; }
        /// <summary>
        /// 标准机器工时
        /// </summary>
        public double MachineHours { get; set; }
        /// <summary>
        /// 人员数量
        /// </summary>
        public double PersonnelNumber { get; set; }
    }

    public class EquipmentDetail
    {
        /// <summary>
        /// 设备
        /// </summary>
        public string EquipmentName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        ///数量
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public double Price { get; set; }

    }
}
