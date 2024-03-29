using BakOracle.Job;
using BakOracle.tool;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Sundial;

namespace BakOracle
{
    public class Program
    {
        internal static IConfiguration Config { get; private set; }
        public static void Main(string[] args)
        {
            Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddTransient<ADOBack>();
                    services.AddTransient<ADOBackTwo>();
                    services.AddTransient<Backups>();
                    services.AddTransient<DapperBacktTow>();
                    services.AddTransient<DapperBack>();
                    services.AddTransient<ImportTool>();
                    services.AddSchedule(options =>
                    {
                        var triggerBuilder = Triggers.DailyAt(1)
                       .SetTriggerId("BackOracle")   // 作业触发器 Id
                       .SetDescription("每天的半夜一点备份数据库(只保留最近七天的)")  // 作业触发器描述
                       .SetRunOnStart(true);   // 作业触发器是否启动时执行一次                                                   
                        options.AddJob<MailJob>("BackOracle", triggerBuilder);
                    });
                    //services.AddHostedService<Worker>();                
                })
                .ConfigureWebHostDefaults(builder =>
                {                    
                    builder.Configure(app =>
                    {
                        app.UseStaticFiles();
                        app.UseScheduleUI(options =>
                        {
                            options.RequestPath = "/BakJob";
                        });
                    });
                })
                .UseWindowsService(options =>
                {
                    options.ServiceName = "数据库备份服务";
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddLog4Net("log4net.config");
                })
                .Build();

            host.Run();
        }
    }
}