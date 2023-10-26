using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ProcessHoursEnteritemDto: EntityDto<long>
    {
        public bool IsDeleted { get; set; }
        public System.Nullable<System.Int64> DeleterUserId { get; set; }
        public System.Nullable<System.DateTime> DeletionTime { get; set; }
        public System.Nullable<System.DateTime> LastModificationTime { get; set; }
        public System.Nullable<System.Int64> LastModifierUserId { get; set; }
        public System.DateTime CreationTime { get; set; }
        public System.Nullable<System.Int64> CreatorUserId { get; set; }
        public System.Nullable<System.Decimal> LaborHour { get; set; }
        public System.Nullable<System.Decimal> MachineHour { get; set; }
        public System.Nullable<System.Decimal> PersonnelNumber { get; set; }
        public System.Nullable<System.Decimal> ProcessHoursEnterId { get; set; }
        public string Year { get; set; }
        public string YearInt { get; set; }
        public long ModelCountYearId { get; set; }
    }
}