using BakOracle.Job;
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
                    services.AddTransient<Backups>();
                    services.AddTransient<DapperBacktTow>();
                    services.AddTransient<DapperBack>();
                    services.AddSchedule(options =>
                    {
                        var triggerBuilder = Triggers.DailyAt(1)
                       .SetTriggerId("BackOracle")   // ��ҵ������ Id
                       .SetDescription("ÿ��İ�ҹһ�㱸�����ݿ�(ֻ������������)")  // ��ҵ����������
                       .SetRunOnStart(true);   // ��ҵ�������Ƿ�����ʱִ��һ��                                                   
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
                    options.ServiceName = "���ݿⱸ�ݷ���";
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