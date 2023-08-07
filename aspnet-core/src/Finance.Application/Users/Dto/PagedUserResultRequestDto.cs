using Abp.Application.Services.Dto;
using System;

namespace Finance.Users.Dto
{
    //custom PagedResultRequestDto
    public class PagedUserResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 用户名/名称
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
