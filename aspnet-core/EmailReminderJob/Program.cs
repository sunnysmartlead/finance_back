using EmailReminderJob.Job;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Sundial;

namespace EmailReminderJob
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
                    services.AddTransient<MailAppService>();
                    services.AddSchedule(options =>
                    {
                        var triggerBuilder = Triggers.PeriodSeconds(3)
                       .SetTriggerId("ExamineMail")   // 作业触发器 Id
                       .SetDescription("每天的早上八点触发(检查密码是否快过期)")  // 作业触发器描述
                       .SetRunOnStart(true);   // 作业触发器是否启动时执行一次    
                        // 注册作业，并配置作业触发器  每天的早上八点触发
                       options.AddJob<MailJob>("ExamineMail", triggerBuilder);
                    });                  
                    //services.AddHostedService<Worker>();

                })
                .ConfigureWebHostDefaults(builder => builder.Configure(app =>
                {
                    app.UseStaticFiles();
                    app.UseScheduleUI(options =>
                    {
                        options.RequestPath = "/Job";
                    });                
                }))
                .UseWindowsService(options =>
                {
                    options.ServiceName="财务邮件提醒服务";
                })
                .Build();
            host.Run();
        }
    }
}