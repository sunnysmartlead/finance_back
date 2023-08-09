using Abp.Auditing;
using Abp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Login.Dto
{
    /// <summary>
    /// 登录实体类
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 登录帐号或者邮箱
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string UserNameOrEmailAddress { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }
        /// <summary>
        /// 记忆登录状态
        /// </summary>
        public bool RememberClient { get; set; }
    }
}
