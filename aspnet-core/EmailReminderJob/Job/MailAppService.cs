


using EmailReminderJob.Dto;
using EmailReminderJob.Ext;
using RestSharp;
using System.Text.Json;

namespace EmailReminderJob.Job
{
    public class MailAppService 
    {
        private readonly ILogger<MailAppService> _logger; 
        /// <summary>
        /// 私钥
        /// </summary>
        private string PrivateKey = string.Empty;
        private string Api=string.Empty;
        public MailAppService(ILogger<MailAppService> logger) 
        {
            Api = Program.Config["Api"];
            PrivateKey= Program.Config["PrivateKey"];
            _logger = logger;
        }
      
        /// <summary>
        /// 调用api接口判断邮件密码状态
        /// </summary>
        /// <returns></returns>
        public async void GetIsMain()
        {
            try
            {
                RestClientOptions options = new RestClientOptions(Api + "api/services/app/EmailJob/CheckEmailPassword");
                RestClient client = new RestClient(options);
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                ResultDto? response = await client.GetAsync<ResultDto>(request);
                if (response is not null)
                {
                    EmailType? emailType = response.Result.ParseEnum<EmailType>();
                    switch (emailType)
                    {
                        case EmailType.NoEmailPassword:
                        case EmailType.EmailPasswordWillExpire:
                            SendEmail(new SendMailDto() { EmailType = emailType, PrivateKey = PrivateKey });
                            break;
                        case null:
                        case EmailType.EmailPasswordIsNormal:
                            break;
                    }
                }
                _logger.LogInformation($"MailAppService=>GetIsMain {DateTime.Now},data:{JsonSerializer.Serialize(response)},接口访问");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error:=>>>>>>>>>>>>>>>>{ex.Message} MailAppService=>GetIsMain {DateTime.Now},接口访问错误");
            }
        }
        public async  void SendEmail(SendMailDto resultDto)
        {
            try
            {
                RestClientOptions? options = new(Api + "api/services/app/EmailJob/SendMail");
                var client = new RestClient(options);
                var request = new RestRequest().AddJsonBody(resultDto);
                var response = await client.PostAsync(request);

                _logger.LogInformation($"MailAppService=>SendEmail {DateTime.Now},data:{JsonSerializer.Serialize(response)},接口访问");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error:=>>>>>>>>>>>>>>>>{ex.Message} MailAppService=>SendEmail {DateTime.Now},接口访问错误");
            }          
        }      
    }    
}
