using System.Collections.Generic;

namespace Finance.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        /// <summary>
        /// 角色信息
        /// </summary>
        public RoleEditDto Role { get; set; }
        /// <summary>
        /// 全部的权限
        /// </summary>
        public List<FlatPermissionDto> Permissions { get; set; }
        /// <summary>
        /// 该角色拥有的权限
        /// </summary>
        public List<string> GrantedPermissionNames { get; set; }
    }
}