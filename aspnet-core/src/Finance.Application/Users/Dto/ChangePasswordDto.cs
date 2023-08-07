using System.ComponentModel.DataAnnotations;

namespace Finance.Users.Dto
{
    public class ChangePasswordDto
    {
        /// <summary>
        /// 当前的密码
        /// </summary>
        [Required]
        public string CurrentPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        public string NewPassword { get; set; }
    }
}
