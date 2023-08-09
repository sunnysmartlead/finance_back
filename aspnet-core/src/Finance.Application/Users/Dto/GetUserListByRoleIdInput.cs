using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Users.Dto
{
    /// <summary>
    /// 获取用户列表根据角色Name  方法的输入
    /// </summary>
    public class GetUserListByRoleIdInput : PagedResultRequestDto
    {
        /// <summary>
        /// 获取拥有此角色的用户列表传入的角色Id
        /// </summary>
        public virtual long RoleId { get; set; }
    }

    /// <summary>
    /// 根据角色名获取用户列表 方法输入
    /// </summary>
    public class GetUserListByRoleNameInput : PagedResultRequestDto
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public virtual string RoleName { get; set; }
    }

    public class GetUserListByRoleNamesInput : PagedResultRequestDto
    
    {
        /// <summary>
        /// 角色名列表
        /// </summary>
        public virtual List<string> RoleNames { get; set; }
    }
}
