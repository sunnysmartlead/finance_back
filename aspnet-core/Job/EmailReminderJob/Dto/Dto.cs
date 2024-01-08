using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EmailReminderJob.Dto
{
    public class ResultDto
    {
        public int Result { get; set; }
        public string? TargetUrl { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
        public bool UnAuthorizedRequest { get; set; }       

    }
    /// <summary>
    /// 邮件类型
    /// </summary>
    public enum EmailType
    {
        /// <summary>
        /// 没有邮件密码(尝试使用默认密码发送)
        /// </summary>
        [Description("没有邮件密码(尝试使用默认密码发送)")]
        NoEmailPassword,
        /// <summary>
        /// 邮件密码将过期
        /// </summary>
        [Description("邮件密码即将过期")]
        EmailPasswordWillExpire,
        /// <summary>
        /// 邮件密码正常
        /// </summary>
        [Description("邮件密码正常")]
        EmailPasswordIsNormal
    }
    /// <summary>
    /// 发送邮件Dto
    /// </summary>
    public class SendMailDto
    {
        /// <summary>
        /// 发送类型
        /// </summary>
        public EmailType? EmailType { get; set; }
        /// <summary>
        /// 私钥(为防止外部程序调用导致邮件异常)
        /// </summary>        
        public string? PrivateKey { get; set; }
    }
}
