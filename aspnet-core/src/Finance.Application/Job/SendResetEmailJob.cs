using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Finance.Audit;
using Finance.Authorization.Users;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Job
{
    public class SendResetEmailJob : AsyncBackgroundJob<TaskReset>, ITransientDependency
    {
        private readonly IRepository<NodeInstance, long> _nodeInstanceRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<NoticeEmailInfo, long> _noticeEmailInfoRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SendResetEmailJob(IRepository<NodeInstance, long> nodeInstanceRepository, IRepository<User, long> userRepository, IRepository<NoticeEmailInfo, long> noticeEmailInfoRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _nodeInstanceRepository = nodeInstanceRepository;
            _userRepository = userRepository;
            _noticeEmailInfoRepository = noticeEmailInfoRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async override Task ExecuteAsync(TaskReset resetTaskInput)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    #region 邮件发送

                    SendEmail email = new SendEmail();
                    string loginIp = email.GetLoginAddr();
                    var emailInfoList = await _noticeEmailInfoRepository.GetAllListAsync();

                    var userInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == resetTaskInput.TargetUserId);
                    var task = await _nodeInstanceRepository.FirstOrDefaultAsync(p => p.Id == resetTaskInput.NodeInstanceId);
                    var resetUser = await _userRepository.FirstOrDefaultAsync(p => p.Id == resetTaskInput.ResetUserId);

                    if (userInfo != null)
                    {
                        string emailAddr = userInfo.EmailAddress;
                        string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":8081/login";
                        string emailBody = $"核价报价提醒：您有新的工作流，由{resetUser.Name}重置给您。（{task.Name}——流程号：{ task.WorkFlowInstanceId}）需要完成（<a href=\"{loginAddr}\" >系统地址</a>）";
                        try
                        {
                            await email.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), $"{task.Name},流程号{task.WorkFlowInstanceId}", emailBody, emailAddr, emailInfoList.Count == 0 ? null : emailInfoList.FirstOrDefault());
                        }
                        catch (Exception)
                        {
                        }
                    }

                    #endregion
                }
                uow.Complete();
            }
        }
    }
}
