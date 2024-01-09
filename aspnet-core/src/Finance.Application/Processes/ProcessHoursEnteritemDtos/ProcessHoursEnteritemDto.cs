using Abp.Application.Services.Dto;
using System;

namespace Finance.Processes
{
    /// <summary>
    /// 工时
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
        /// <summary>
        /// 标准人工工时
        /// </summary>
        public System.Nullable<System.Decimal> LaborHour { get; set; }
        /// <summary>
        /// 标准机器工时
        /// </summary>
        public System.Nullable<System.Decimal> MachineHour { get; set; }
        /// <summary>
        /// 人员数量
        /// </summary>
        public System.Nullable<System.Decimal> PersonnelNumber { get; set; }
        /// <summary>
        /// 工时工序id
        /// </summary>
        public System.Nullable<System.Decimal> ProcessHoursEnterId { get; set; }
        /// <summary>
        /// 年度
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 年度
        /// </summary>
        public string YearInt { get; set; }
        /// <summary>
        /// 年度id
        /// </summary>
        public long ModelCountYearId { get; set; }
    }
}