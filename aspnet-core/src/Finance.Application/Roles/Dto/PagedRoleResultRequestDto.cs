using Abp.Application.Services.Dto;

namespace Finance.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 角色名/显示名/描述
        /// </summary>
        public string Keyword { get; set; }
    }
}

