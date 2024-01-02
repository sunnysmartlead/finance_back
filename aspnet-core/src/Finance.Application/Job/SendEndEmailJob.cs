using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Finance.Audit;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;
using Finance.PriceEval;
using Finance.WorkFlows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Job
{
    public class SendEndEmailJob : AsyncBackgroundJob<NodeInstance>, ITransientDependency
    {
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<NoticeEmailInfo, long> _noticeEmailInfoRepository;
        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SendEndEmailJob(WorkflowInstanceAppService workflowInstanceAppService, IRepository<User, long> userRepository, IRepository<NoticeEmailInfo, long> noticeEmailInfoRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<Role, int> roleRepository, IRepository<UserRole, long> userRoleRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _workflowInstanceAppService = workflowInstanceAppService;
            _userRepository = userRepository;
            _noticeEmailInfoRepository = noticeEmailInfoRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async override Task ExecuteAsync(NodeInstance nodeInstance)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    #region 邮件发送

                    //#if !DEBUG
                    SendEmail email = new SendEmail();
                    string loginIp = email.GetLoginAddr();
                    var emailInfoList = await _noticeEmailInfoRepository.GetAllListAsync();


                    var priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == nodeInstance.WorkFlowInstanceId);
                    var role = await _roleRepository.GetAllListAsync(p =>
                    p.Name == StaticRoleNames.Host.FinanceTableAdmin || p.Name == StaticRoleNames.Host.EvalTableAdmin
            || p.Name == StaticRoleNames.Host.Bjdgdgly);
                    var userIds = await _userRoleRepository.GetAll().Where(p => role.Select(p => p.Id).Contains(p.RoleId)).Select(p => p.UserId).ToListAsync();

                    if (priceEvaluation != null)
                    {
                        userIds.Add(priceEvaluation.ProjectManager);
                        if (priceEvaluation.CreatorUserId.HasValue
                            && priceEvaluation.CreatorUserId != priceEvaluation.ProjectManager)
                        {
                            userIds.Add(priceEvaluation.CreatorUserId.Value);
                        }
                    }
                    userIds = userIds.Distinct().ToList();
                    foreach (var userId in userIds)
                    {
                        var userInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == userId);

                        if (userInfo != null)
                        {
                            string emailAddr = userInfo.EmailAddress;
                            string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":8081/login";
                            string emailBody = $"核价报价提醒：您有新的工作流{priceEvaluation.Title}（{nodeInstance.Name}——流程号：{nodeInstance.WorkFlowInstanceId}）需要完成（<a href=\"{loginAddr}\" >系统地址</a>）";

                            try
                            {
                                if (!emailAddr.Contains("@qq.com"))
                                {
                                    await email.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), $"{nodeInstance.Name},流程号{nodeInstance.WorkFlowInstanceId}", emailBody, emailAddr, emailInfoList.Count == 0 ? null : emailInfoList.FirstOrDefault());
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    #endregion
                }
                uow.Complete();
            }
        }
    }
}
