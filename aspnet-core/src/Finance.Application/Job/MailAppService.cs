using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Finance.Audit;
using Microsoft.Extensions.Logging;
using Sundial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Job
{
    public class MailAppService : FinanceAppServiceBase
    {
        private readonly IRepository<NoticeEmailInfo, long> repository;
        private readonly ISchedulerFactory _schedulerFactory;

        public MailAppService()
        {
            repository = IocManager.Instance.Resolve<IRepository<NoticeEmailInfo, long>>();
            _schedulerFactory = IocManager.Instance.Resolve<ISchedulerFactory>();
        }
        /// <summary>
        /// 是否发邮件提醒修改密码
        /// </summary>
        /// <returns></returns>
        public async void GetIsMain()
        {
            using (IUnitOfWorkCompleteHandle unitOfWork = IocManager.Instance.Resolve<IUnitOfWorkManager>().Begin())
            {
                NoticeEmailInfo emailInfoList = await repository.FirstOrDefaultAsync(p => p.Id != 0);
                EmailService.emailInfoListL = emailInfoList;
                if (emailInfoList != null)
                {
                    //检车任务是否存在
                    var isExist = _schedulerFactory.ContainsJob(jobId: "SendNoPasswordMailJob");
                    if(isExist) RemoveJob(1); //如果已经设置密码,就删除该任务
                    DateTime date = DateTime.Now;
                    DateTime dateTime = (DateTime)(emailInfoList.LastModificationTime == null ? emailInfoList.CreationTime : emailInfoList.LastModificationTime);
                    //判断密码是否快过期         
                    if (date.Subtract(dateTime).Days >= FinanceConsts.Mail_Password_Date)
                    {
                        //检车任务是否存在
                        var isSendPasswordOutMainJob = _schedulerFactory.ContainsJob("SendPasswordOutMainJob");
                        //快过期了,进行邮件提醒
                        if (!isSendPasswordOutMainJob)
                        {
                            AddJob(2);
                        }
                   
                         }else
                    {
                        //清除任务
                        RemoveJob(2);
                    }
                }
                else
                {
                    //检车任务是否存在
                    var isSendNoPasswordMailJob = _schedulerFactory.ContainsJob("SendNoPasswordMailJob");
                    //没有密码,发邮件提示设置密码
                    if(!isSendNoPasswordMailJob) AddJob(1);
                }
            }
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="prop">1 邮件提醒没有设置密码 2邮件提醒修改密码</param>
        public void AddJob(int prop)
        {
            if (prop != 0)
            {
                switch (prop)
                {
                    case 1:
                        var triggerBuilderNO = Triggers.DailyAt(9)
        .SetTriggerId("SendNoPasswordMailJob")   // 作业触发器 Id
        .SetDescription("每天早上九点钟触发(发送邮件提醒没有设置邮箱密码)");  // 作业触发器描述 
                        //每天早上九点钟触发
                        _schedulerFactory.AddJob<SendNoPasswordMailJob>("SendNoPasswordMailJob",triggerBuilderNO);
                        break;
                    case 2:
                        TriggerBuilder triggerBuilder = Triggers.DailyAt(9)
.SetTriggerId("SendPasswordOutMainJob")   // 作业触发器 Id
.SetDescription("每天早上九点钟触发(发送邮件提醒改修改密码了)");  // 作业触发器描述 
                        //每天早上九点钟触发
                        _schedulerFactory.AddJob<SendPasswordOutMainJob>("SendPasswordOutMainJob", triggerBuilder);
                        break;
                }
            }

        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="prop">1 邮件提醒没有设置密码 2邮件提醒修改密码</param>
        public void RemoveJob(int prop)
        {
            if (prop != 0)
            {
                switch (prop)
                {
                    case 1:
                        _schedulerFactory.RemoveJob("SendNoPasswordMailJob");
                        break;
                    case 2:
                        _schedulerFactory.RemoveJob("SendPasswordOutMainJob");
                        break;
                }
            }

        }
    }
}
