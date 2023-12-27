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
                        var triggerBuilder = Triggers.DailyAt(8)
                       .SetTriggerId("ExamineMail")   // ��ҵ������ Id
                       .SetDescription("ÿ������ϰ˵㴥��(��������Ƿ�����)")  // ��ҵ����������
                       .SetRunOnStart(true);   // ��ҵ�������Ƿ�����ʱִ��һ��    
                       //.SetMaxNumberOfErrors(9999)// ��ҵ���������������
                       //.SetNumRetries(999)   // ��ҵ�������������Դ���
                       //.SetRetryTimeout(5); // ��ҵ���������Լ��ʱ��                      
                        // ע����ҵ����������ҵ������  ÿ������ϰ˵㴥��
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
                    options.ServiceName="�����ʼ����ѷ���";
                })
                .Build();
            host.Run();
        }
    }
}