using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Finance.Audit;
using Finance.Authorization.Users;
using Finance.WorkFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Job
{
    public class SendEmailJob : AsyncBackgroundJob<NodeInstance>, ITransientDependency
    {
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<NoticeEmailInfo, long> _noticeEmailInfoRepository;

        public async override Task ExecuteAsync(NodeInstance nodeInstance)
        {
            #region 邮件发送

            SendEmail email = new SendEmail();
            string loginIp = email.GetLoginAddr();
            var allAuditFlowInfos = await _workflowInstanceAppService.GetTaskByWorkflowInstanceId(nodeInstance.WorkFlowInstanceId, nodeInstance.Id);
            foreach (var task in allAuditFlowInfos)
            {
                foreach (var userId in task.TaskUserIds)
                {
                    var userInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == userId);

                    if (userInfo != null)
                    {
                        string emailAddr = userInfo.EmailAddress;
                        var emailInfoList = await _noticeEmailInfoRepository.GetAllListAsync();
                        string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":8081/login";
                        string emailBody = "核价报价提醒：您有新的工作流（" + task.NodeName + "——流程号：" + task.WorkFlowInstanceId + "）需要完成（" + "<a href=\"" + loginAddr + "\" >系统地址</a>" + "）";

                        try
                        {
                            await email.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), $"{task.NodeName},流程号{task.WorkFlowInstanceId}", emailBody, emailAddr, emailInfoList.Count == 0 ? null : emailInfoList.FirstOrDefault());
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            #endregion
        }
    }
}
