using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Hr.Dto
{
    /// <summary>
    /// 部门新增
    /// </summary>
    public class AddDepartment
    {
        /// <summary>
        /// 部门名称（不可包含斜线“/”）
        /// </summary>
        [Required]
        [RegularExpression(FinanceConsts.DepartmentNameRegular)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 父部门
        /// </summary>
        public virtual long? Fid { get; set; }
    }
}
