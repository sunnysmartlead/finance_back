using Sundial;

namespace BakOracle.Job
{
    public class MailJob : IJob
    {
        private Backups backups { get; set; }
        private DapperBack dapperBack { get; set; }
        private DapperBacktTow dapperBacktwo { get; set; }

        private ADOBack  aDOBack { get; set; }
        public MailJob(Backups _backups, DapperBack dapperBack, DapperBacktTow dapperBacktwo, ADOBack aDOBack)
        {
            backups = _backups;
            this.dapperBack = dapperBack;
            this.dapperBacktwo = dapperBacktwo;
            this.aDOBack = aDOBack;
        }
        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            aDOBack.Main();
        }
    }
}
