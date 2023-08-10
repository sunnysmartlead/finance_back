using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Finance.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit
{
    public class SendEmail: ISingletonDependency
    {
        private readonly IConfigurationRoot _appConfiguration;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SendEmail()
        {
            var env = IocManager.Instance.Resolve<IWebHostEnvironment>();
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }


        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="isInternet"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="mailTo"></param>
        /// <param name="emailInfo"></param>
        /// <param name="IsBodyHtml"></param>
        /// <returns></returns>
        internal async Task<bool> SendEmailToUser(bool isInternet, string Subject, string Body, string mailTo, NoticeEmailInfo emailInfo = null, bool IsBodyHtml = true)
        {
            string mailPassword = null;
            MailMessage mailMessage;
            if (isInternet)
            {
                mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(FinanceConsts.MailFrom_Tencent, "智领核价报价系统"); // 发送人和收件人
                mailMessage.To.Add(new MailAddress(mailTo));
            }
            else
            {
                mailPassword = emailInfo == null ? FinanceConsts.MailUserPassword_Sunny : emailInfo.EmailPassword;
                mailMessage = new MailMessage(FinanceConsts.MailFrom_Sunny, mailTo); // 发送人和收件人
            }
            mailMessage.Subject = Subject;//主题
            mailMessage.Body = Body;//内容
            mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
            mailMessage.IsBodyHtml = IsBodyHtml;
            mailMessage.Priority = MailPriority.Normal;//优先级

            try
            {
                if(isInternet)
                {
                    SmtpClient SMTPClient_Tencent = new SmtpClient()
                    {
                        DeliveryMethod = SmtpDeliveryMethod.Network,//指定电子邮件发送方式
                        Host = FinanceConsts.SmtpServer_Tencent, //指定SMTP服务器
                        Port = FinanceConsts.SmtpPort_Tencent,//指定SMTP服务器端口
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new System.Net.NetworkCredential(FinanceConsts.MailFrom_Tencent, FinanceConsts.MailUserPassword_Tencent)//用户名和密码
                    };
                    await SMTPClient_Tencent.SendMailAsync(mailMessage); // 发送邮件
                }
                else
                {
                    SmtpClient SMTPClient_Sunny = new SmtpClient()
                    {
                        DeliveryMethod = SmtpDeliveryMethod.Network,//指定电子邮件发送方式
                        Host = FinanceConsts.SmtpServer_Sunny, //指定SMTP服务器
                        Port = FinanceConsts.SmtpPort_Sunny,//指定SMTP服务器端口
                        Credentials = new System.Net.NetworkCredential(FinanceConsts.MailFrom_Sunny, mailPassword)//用户名和密码
                    };
                    await SMTPClient_Sunny.SendMailAsync(mailMessage); // 发送邮件
                }
                return true;
            }
            catch (SmtpException ex)
            {
                throw new FriendlyException("通知邮件发送失败" + ex.Message);
            }
        }

        /// <summary>
        /// 获取登录服务器的IP
        /// </summary>
        /// <returns></returns>
        internal string GetLoginAddr()
        {
            string loginIP = null;
            var ipList = new List<IPAddress>();
            var addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (var add in addressList)
            {
                try
                {
                    long a = add.ScopeId;
                }
                catch
                {
                    ipList.Add(add);
                }
            }
            if(ipList.Count > 0)
            {
                loginIP = ipList[0].ToString();
            }
            return loginIP;
        }
    }
}
