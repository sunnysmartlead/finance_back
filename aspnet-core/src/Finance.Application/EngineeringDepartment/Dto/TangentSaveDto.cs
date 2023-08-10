using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment.Dto
{
    /// <summary>
    /// 获取工序工时传入Dto
    /// </summary>
    public class TangentGetDto
    {
        /// <summary>
        /// 流程表ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// MoudelCount表id
        /// </summary>
        public long ProductId { get; set; }


    }
    /// <summary>
    /// 保存工序工时是传入Dto
    /// </summary>
    public class TangentSaveDto
    {

        /// <summary>
        /// 流程表ID
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// MoudelCount表id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 切线工时详细信息
        /// </summary>
        public List<TangentHoursDetail> TangentHoursDetailList { get; set; }
        /// <summary>
        /// UPH
        /// </summary>
        [Range(1, (double)decimal.MaxValue)]
        public decimal UPH { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }
    /// <summary>
    /// 切线工时
    /// </summary>
    public class TangentHoursDetail
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int year { get; set; }
        /// <summary>
        /// 标准人工工时
        /// </summary>
        public double LaborTime { get; set; }
        /// <summary>
        /// 标准机器工时
        /// </summary>
        public double MachineHours { get; set; }
        /// <summary>
        /// 原【人员数量】字段改成【人均跟线数量】
        /// </summary>
        public double PersonnelNumber { get; set; }
    }
}
