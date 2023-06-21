using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Authorization;

namespace Finance.Roles.Dto
{
    [AutoMapFrom(typeof(Permission))]
    public class PermissionDto //: EntityDto<long>
    {
        /// <summary>
        /// Ȩ������
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ȩ����ʾ����
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Ȩ������
        /// </summary>
        public string Description { get; set; }
    }
}
