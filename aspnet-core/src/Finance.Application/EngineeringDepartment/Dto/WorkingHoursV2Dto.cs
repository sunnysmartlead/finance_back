using Abp.Application.Services.Dto;
using Finance.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment.Dto
{
    public class WorkingHoursV2Dto
    {
        /// <summary>
        /// 年份
        /// </summary>

        public int Year { get; set; }
        /// <summary>
        /// 人工工时
        /// </summary>

        public double LaborHour { get; set; }
        /// <summary>
        /// 机器工时
        /// </summary>

        public double MachineHour { get; set; }
        /// <summary>
        /// 人均跟线数量
        /// </summary>

        public double PerFollowUpQuantity { get; set; }
    }
    public class EditWorkingHoursV2Dto : EntityDto<long>
    {

        /// <summary>
        /// 年份
        /// </summary>

        public int Year { get; set; }
        /// <summary>
        /// 人工工时
        /// </summary>

        public double LaborHour { get; set; }
        /// <summary>
        /// 机器工时
        /// </summary>

        public double MachineHour { get; set; }
        /// <summary>
        /// 人均跟线数量
        /// </summary>

        public double PerFollowUpQuantity { get; set; }

    }



    /// <summary>
    /// 获取字典列表方法的参数输入
    /// </summary>
    public class GetWorkingHoursV2DtoListInput : PagedInputDto
    {
        /// <summary>
        /// 年份
        /// </summary>

        public int Year { get; set; }
        /// <summary>
        /// 人工工时
        /// </summary>

        public double LaborHour { get; set; }
        /// <summary>
        /// 机器工时
        /// </summary>

        public double MachineHour { get; set; }
        /// <summary>
        /// 人均跟线数量
        /// </summary>

        public double PerFollowUpQuantity { get; set; }
    }
}
