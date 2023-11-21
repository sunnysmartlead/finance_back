using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Hr.Dto
{
    public class GetUserByDepartmentIdInput: PagedResultRequestDto
    {
        /// <summary>
        /// 从GetDepartmentByName接口获取到的部门Id
        /// </summary>
        public virtual  long DepartmentId {  get; set; }
    }
}
