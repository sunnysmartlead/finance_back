using Abp.Application.Services.Dto;
using Finance.Hr.Dto;
using Finance.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Login.Dto
{
    /// <summary>
    /// 登录返回的实体类
    /// </summary>
    public class LoginResultModel
    {
        /// <summary>
        /// token
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// token有效期
        /// </summary>
        public int ExpireInSeconds { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public UserModel User { get; set; }
    }
    /// <summary>
    /// 登录返回的用户信息
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户所在公司
        /// </summary>
        public DepartmentListDto UserCompany { get; set; }

        ///// <summary>
        ///// 用户所在公司 Id
        ///// </summary>
        //public long UserCompanyId { get; set; }

        /// <summary>
        /// 用户名（登录账户）
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户名（显示用）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户工号
        /// </summary>
        public long UserNumber { get; set; }

        /// <summary>
        /// 用户部门
        /// </summary>
        public DepartmentListDto UserDepartment { get; set; }

        ///// <summary>
        ///// 用户部门 Id
        ///// </summary>
        //public long UserDepartmentId { get; set; }

        /// <summary>
        /// 用户岗位
        /// </summary>
        public string UserJobs { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public ListResultDto<RoleDto> UserRole { get; set; }

    }
}
