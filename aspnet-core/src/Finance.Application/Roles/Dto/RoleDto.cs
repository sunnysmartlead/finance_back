using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using Finance.Authorization.Roles;

namespace Finance.Roles.Dto
{
    /// <summary>
    /// ��ɫ��Ϣ
    /// </summary>
    public class RoleDto : EntityDto<int>
    {
        /// <summary>
        /// ��ɫ����
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }
        ///// <summary>
        ///// ��ʾ����
        ///// </summary>
        //[Required]
        //[StringLength(AbpRoleBase.MaxDisplayNameLength)]
        //public string DisplayName { get; set; }
        /// <summary>
        /// ��׼����
        /// </summary>
        public string NormalizedName { get; set; }
        /// <summary>
        /// ��ɫ����
        /// </summary>
        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }
        /// <summary>
        /// ��ɫȨ��
        /// </summary>
        public List<string> GrantedPermissions { get; set; }
    }
}