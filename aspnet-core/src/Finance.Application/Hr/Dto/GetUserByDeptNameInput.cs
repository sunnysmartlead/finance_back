using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Hr.Dto
{
    public class GetUserByDeptNameInput : PagedResultRequestDto
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public virtual string DeptName { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public virtual string Name { get; set; }
    }
}
