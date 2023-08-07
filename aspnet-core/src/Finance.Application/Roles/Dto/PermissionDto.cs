using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Authorization;

namespace Finance.Roles.Dto
{
    [AutoMapFrom(typeof(Permission))]
    public class PermissionDto //: EntityDto<long>
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 权限显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }
    }
}
