using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Hr.Dto
{
    /// <summary>
    /// 部门更新
    /// </summary>
    public class UpdateDepartmentInput : EntityDto<long>
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [Required]
        [RegularExpression(FinanceConsts.DepartmentNameRegular)]//不可包含斜线“/”
        public virtual string Name { get; set; }
    }
}
