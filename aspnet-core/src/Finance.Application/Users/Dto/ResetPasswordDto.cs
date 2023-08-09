using System.ComponentModel.DataAnnotations;

namespace Finance.Users.Dto
{
    public class ResetPasswordDto
    {
        /// <summary>
        /// 管理的密码
        /// </summary>
        [Required]
        public string AdminPassword { get; set; }
        /// <summary>
        /// 用户的ID
        /// </summary>
        [Required]
        public long UserId { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        public string NewPassword { get; set; }
    }
}
