using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 工时
    /// </summary>
    public class ProcessHoursEnterListItemDto : EntityDto<long>
    {

        public bool IsDeleted { get; set; }
        public bool IsCOB { get; set; }
        //删除人
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        //删除时间
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        //维护时间
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        //维护人
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        //创建时间
        public System.DateTime CreationTime { get; set; }
        //创建人
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        //
        public string DevelopTotalPrice { get; set; }
        public System.Nullable<System.Decimal> DeviceTotalPrice { get; set; }
        public string FixtureName { get; set; }
        public decimal FixtureNumber { get; set; }
        public System.Nullable<System.Decimal> FixturePrice { get; set; }
        public string FrockName { get; set; }
        public System.Nullable<System.Decimal> FrockNumber { get; set; }
        public System.Nullable<System.Decimal> FrockPrice { get; set; }
        public System.Nullable<System.Decimal> HardwareDeviceTotalPrice { get; set; }
        //
        public System.Nullable<System.Decimal> HardwareTotalPrice { get; set; }
        public string OpenDrawingSoftware { get; set; }
        //工序名称
        public string ProcessName { get; set; }
        //工序编码
        public string ProcessNumber { get; set; }
        public long? AuditFlowId { get; set; }
        public long? SolutionId { get; set; }
        //设备信息
        public ProcessHoursEnterResponseDeviceDto DeviceInfo { get; set; } = new ProcessHoursEnterResponseDeviceDto();
        //追溯部分(硬件及软件开发费用)
        public ProcessHoursEnterDevelopCostInfoDeviceDto DevelopCostInfo { get; set; } = new ProcessHoursEnterDevelopCostInfoDeviceDto();
        //工装治具部分
        public ProcessHoursEnterToolInfoDto ToolInfo { get; set; } = new ProcessHoursEnterToolInfoDto();

        public List<ProcessHoursEnterSopInfoDto> SopInfo { get; set; } = new List<ProcessHoursEnterSopInfoDto>();
        public List<ProcessHoursEnteritemDto> SopInfoAll { get; set; } = new List<ProcessHoursEnteritemDto>();



    }
}