using Abp.Dependency;
using Abp.Domain.Repositories;
using Finance.Audit;
using Finance.Ext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Sundial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Job
{

    public static class EmailService  
    {
        public static NoticeEmailInfo emailInfoListL { get; set; }
    }
    /// <summary>
    /// 邮件定时任务服务
    /// </summary>
    public class EmailJobAppService: FinanceAppServiceBase
    {
        /// <summary>
        /// 私钥
        /// </summary>
        private readonly string PrivateKey = "6L+Z5piv5LiA5Liq6LSi5Yqh57O757uf6LCD55So6YKu5Lu25Y+R5Yqo55qE56eB6ZKlIQ==";
        private readonly SendEmail _sendEmail;
        private readonly IRepository<NoticeEmailInfo, long> nticeEmailInfo;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sendEmail"></param>
        /// <param name="_noticeEmailInfo"></param>
        public EmailJobAppService(SendEmail sendEmail, IRepository<NoticeEmailInfo, long> _noticeEmailInfo)
        {
            this._sendEmail = sendEmail;
            nticeEmailInfo= _noticeEmailInfo;
        }
        /// <summary>
        /// 检查邮箱密码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<EmailType> CheckEmailPassword()
        {
            NoticeEmailInfo emailInfoList = await nticeEmailInfo.FirstOrDefaultAsync(p => p.Id != 0);
            if (emailInfoList != null)
            {
                DateTime date = DateTime.Now;
                DateTime dateTime = (DateTime)(emailInfoList.LastModificationTime == null ? emailInfoList.CreationTime : emailInfoList.LastModificationTime);
                //判断密码是否快过期         
                if (date.Subtract(dateTime).Days >= FinanceConsts.Mail_Password_Date)
                {
                    return EmailType.EmailPasswordWillExpire;
                }
            }
            else
            {
                return EmailType.NoEmailPassword;
            }
            return EmailType.EmailPasswordIsNormal;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sendMailDto">发动那个邮件的类型</param>
        [HttpPost]
        public void SendMail(SendMailDto sendMailDto)
        {
            if(sendMailDto is null|| sendMailDto.PrivateKey!= PrivateKey) new FriendlyException("格式/私钥不正确");
            switch (sendMailDto.EmailType) 
            {
                case EmailType.NoEmailPassword:
                    NoEmailPassword();
                    break;
                case EmailType.EmailPasswordWillExpire:
                    EmailPasswordWillExpire();
                    break;
                case EmailType.EmailPasswordIsNormal:
                    break;                
            }
        }
        /// <summary>
        /// 没有密码时候发送的邮件(使用默认密码)
        /// </summary>
        internal  void NoEmailPassword()
        {
            NoticeEmailInfo emailInfoList = nticeEmailInfo.FirstOrDefault(p => p.Id != 0);
            string loginIp = _sendEmail.GetLoginAddr();
            string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":" + JobConstant.Port;
            string Subject = "报价核价系统发送邮件模块未设置密码!";
            string Body = "报价核价系统发送邮件模块未设置密码,请尽快设置,否则系统将无法运行";
            string emailBody = Body + "（" + " <a href=\"" + loginAddr + "\" >系统地址</a>" + "）";
            Task.Run(async () => {
                await _sendEmail.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), Subject: Subject, Body: emailBody, "wslinzhang@sunnyoptical.com", emailInfoList == null ? null : emailInfoList);
            });   
        }
        /// <summary>
        /// 密码即将过期时候发动的邮件
        /// </summary>
        internal  void EmailPasswordWillExpire()
        {
            NoticeEmailInfo emailInfoList = nticeEmailInfo.FirstOrDefault(p => p.Id != 0);
            string loginIp = _sendEmail.GetLoginAddr();
            string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":" + JobConstant.Port;
            string Subject = "报价核价系统发送邮件模块密码即将过期!";
            string Body = "报价核价系统发送邮件模块密码即将过期,请尽快修改密码,切记,密码必须与集团邮箱服务器域账号密码一致,请确认后修改,否则系统将无法运行";
            string emailBody = Body + "（" + " <a href=\"" + loginAddr + "\" >系统地址</a>" + "）";
            Task.Run(async () => {
                await _sendEmail.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), Subject: Subject, Body: emailBody, emailInfoList.MaintainerEmail, emailInfoList == null ? null : emailInfoList);
            });        
        }
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
        public EmailType EmailType { get; set; }
        /// <summary>
        /// 私钥(为防止外部程序调用导致邮件异常)
        /// </summary>
        [FriendlyRequired("私钥")]
        public string PrivateKey { get; set; }
    }
}
