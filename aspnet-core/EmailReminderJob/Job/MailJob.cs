using Sundial;

namespace EmailReminderJob.Job
{
    public class MailJob : IJob
    {
        private MailAppService mailAppService { get; set; }
        public MailJob(MailAppService _mailAppService)
        {
            mailAppService = _mailAppService;
        }
        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            mailAppService.GetIsMain();
        }
    }
}
