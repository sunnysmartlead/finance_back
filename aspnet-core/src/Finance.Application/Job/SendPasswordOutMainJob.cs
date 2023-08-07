using Finance.Audit;
using Sundial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Job
{
    internal class SendPasswordOutMainJob : IJob
    {
        private readonly SendEmail _sendEmail;
        public SendPasswordOutMainJob(SendEmail sendEmail)
        {
            this._sendEmail = sendEmail;
        }
        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            NoticeEmailInfo emailInfo = EmailService.emailInfoListL;
            string loginIp = _sendEmail.GetLoginAddr();
            string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":"+ JobConstant.Port;
            string Subject = "报价核价系统发送邮件模块密码即将过期!";
            string Body = "报价核价系统发送邮件模块密码即将过期,请尽快修改密码,切记,密码必须与集团邮箱服务器域账号密码一致,请确认后修改,否则系统将无法运行";
            string emailBody = Body + "（" + " <a href=\"" + loginAddr + "\" >系统地址</a>" + "）";
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            Task.Run(async () => {
                await _sendEmail.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), Subject: Subject, Body: emailBody, emailInfo.MaintainerEmail, emailInfo == null ? null : emailInfo);
            });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            await Task.CompletedTask;
        }
    }
}
