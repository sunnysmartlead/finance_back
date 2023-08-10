using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace Finance.Roles.Dto
{
    public class RoleListDto : EntityDto, IHasCreationTime
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsStatic { get; set; }

        public bool IsDefault { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
