using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Sundial;
using System.Net.Http;

namespace EmailReminderJob.Job
{
    public class MailAppService 
    {        
        private string Api=string.Empty;
        public MailAppService() 
        {
            Api = Program.Config["Api"];
        }
      
        /// <summary>
        /// 调用api接口判断邮件密码状态
        /// </summary>
        /// <returns></returns>
        public async void GetIsMain()
        {
            using (HttpClient sharedClient = new(){BaseAddress = new Uri(Api)})
            {
                using HttpResponseMessage response = await sharedClient.GetAsync("api/services/app/EmailJob/CheckEmailPassword");
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"{jsonResponse}\n");
            }              
        }
    }
}
