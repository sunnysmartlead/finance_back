using Sundial;

namespace BakOracle.Job
{
    public class MailJob : IJob
    {
        private Backups backups { get; set; }
        private DapperBack dapperBack { get; set; }
        public MailJob(Backups _backups, DapperBack dapperBack)
        {
            backups = _backups;
            this.dapperBack = dapperBack;
        }
        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            dapperBack.Main();
        }
    }
}
