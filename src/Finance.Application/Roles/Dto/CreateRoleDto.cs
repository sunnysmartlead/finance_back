using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using Finance.Authorization.Roles;

namespace Finance.Roles.Dto
{
    public class CreateRoleDto
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
        /// ��׼����(��ĸȫ��תΪ��д)
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

        public CreateRoleDto()
        {
            GrantedPermissions = new List<string>();
        }
    }
}
