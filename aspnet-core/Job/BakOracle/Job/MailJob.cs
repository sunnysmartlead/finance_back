using Sundial;

namespace BakOracle.Job
{
    public class MailJob : IJob
    {
        private Backups backups { get; set; }
        public MailJob(Backups _backups)
        {
            backups = _backups;
        }
        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            backups.GetIsMain();
        }
    }
}
