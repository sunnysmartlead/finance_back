﻿using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Finance.Audit;
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
    public class SendEmailJob : AsyncBackgroundJob<NodeInstance>, ITransientDependency
    {
        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<NoticeEmailInfo, long> _noticeEmailInfoRepository;
        private readonly IRepository<WorkflowInstance, long> _workflowInstanceRepository;
        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SendEmailJob(WorkflowInstanceAppService workflowInstanceAppService, IRepository<User, long> userRepository, IRepository<NoticeEmailInfo, long> noticeEmailInfoRepository, IRepository<WorkflowInstance, long> workflowInstanceRepository, IUnitOfWorkManager unitOfWorkManager, IRepository<PriceEvaluation, long> priceEvaluationRepository)
        {
            _workflowInstanceAppService = workflowInstanceAppService;
            _userRepository = userRepository;
            _noticeEmailInfoRepository = noticeEmailInfoRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _priceEvaluationRepository = priceEvaluationRepository;
        }

        public async override Task ExecuteAsync(NodeInstance nodeInstance)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    #region 邮件发送

                    SendEmail email = new SendEmail();
                    string loginIp = email.GetLoginAddr();
                    var allAuditFlowInfos = await _workflowInstanceAppService.GetTaskByWorkflowInstanceId(nodeInstance.WorkFlowInstanceId, nodeInstance.Id);
                    var emailInfoList = await _noticeEmailInfoRepository.GetAllListAsync();
                    var pmName = await (from p in _priceEvaluationRepository.GetAll()
                                        join u in _userRepository.GetAll() on p.ProjectManager equals u.Id
                                        where p.AuditFlowId == nodeInstance.WorkFlowInstanceId
                                        select u.Name).FirstOrDefaultAsync();

                    foreach (var task in allAuditFlowInfos)
                    {
                        var workflowInstance = await _workflowInstanceRepository.GetAsync(task.WorkFlowInstanceId);

                        foreach (var userId in task.TaskUserIds)
                        {
                            var userInfo = await _userRepository.FirstOrDefaultAsync(p => p.Id == userId);

                            if (userInfo != null)
                            {
                                string emailAddr = userInfo.EmailAddress;
                                string loginAddr = "http://" + (loginIp.Equals(FinanceConsts.AliServer_In_IP) ? FinanceConsts.AliServer_Out_IP : loginIp) + ":8081/login";
                                string emailBody = $"核价报价提醒：您有新的工作流{workflowInstance.Title}，项目经理：{pmName}（{task.NodeName}——流程号：{task.WorkFlowInstanceId}）需要完成（<a href=\"{loginAddr}\" >系统地址</a>）";

                                try
                                {
                                    await email.SendEmailToUser(loginIp.Equals(FinanceConsts.AliServer_In_IP), $"{workflowInstance.Title}{task.NodeName},流程号{task.WorkFlowInstanceId}", emailBody, emailAddr, emailInfoList.Count == 0 ? null : emailInfoList.FirstOrDefault());
                                }
                                catch (Exception)
                                {
                                }
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
