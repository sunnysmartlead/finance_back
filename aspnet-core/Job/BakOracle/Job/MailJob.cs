using BakOracle.tool;
using Sundial;

namespace BakOracle.Job
{
    public class MailJob : IJob
    {
        private Backups backups { get; set; }
        private DapperBack dapperBack { get; set; }
        private DapperBacktTow dapperBacktwo { get; set; }

        private ADOBack  aDOBack { get; set; }
        private ADOBackTwo aDOBackTwo { get; set; }

        private ImportTool importTool { get; set; }

        public MailJob(Backups _backups, DapperBack dapperBack, DapperBacktTow dapperBacktwo, ADOBack aDOBack, ADOBackTwo aDOBackTwo, ImportTool importTool)
        {
            backups = _backups;
            this.dapperBack = dapperBack;
            this.dapperBacktwo = dapperBacktwo;
            this.aDOBack = aDOBack;
            this.aDOBackTwo = aDOBackTwo;
            this.importTool = importTool;
        }
        public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
        {
            importTool.Main();
        }
    }
}
