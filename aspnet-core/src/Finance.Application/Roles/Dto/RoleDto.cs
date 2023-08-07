using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using Finance.Authorization.Roles;

namespace Finance.Roles.Dto
{
    /// <summary>
    /// 角色信息
    /// </summary>
    public class RoleDto : EntityDto<int>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }
        ///// <summary>
        ///// 显示名称
        ///// </summary>
        //[Required]
        //[StringLength(AbpRoleBase.MaxDisplayNameLength)]
        //public string DisplayName { get; set; }
        /// <summary>
        /// 标准名称
        /// </summary>
        public string NormalizedName { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }
        /// <summary>
        /// 角色权限
        /// </summary>
        public List<string> GrantedPermissions { get; set; }
    }
}