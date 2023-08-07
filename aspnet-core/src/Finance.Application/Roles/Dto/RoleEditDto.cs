using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Finance.Authorization.Roles;

namespace Finance.Roles.Dto
{
    public class RoleEditDto: EntityDto<int>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsStatic { get; set; }
    }
}