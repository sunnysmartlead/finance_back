using Abp.Authorization;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// 校验前端传的用户Id和Token是否一致 (也就是确保前端页面显示的用户和Token对应,然后不会导致后端获取ID时候不准)
    /// </summary>
    public class CheckTokenAndUserId
    {
        private readonly RequestDelegate _next;
        private readonly IAbpSession _abpSession;
        public CheckTokenAndUserId(RequestDelegate next, IAbpSession abpSession)
        {
            _next = next;
            _abpSession = abpSession;
        }
        public async Task InvokeAsync(HttpContext context, IWebHostEnvironment env)        
        {
            //本地运行直接跳过和登录地址
            if (!env.IsDevelopment())
            {
                // 获取当前用户的ID  
                var userId = _abpSession.UserId; 
                //获取Headers中的UserId
                var UserId = context.Request.Headers["UserId"].FirstOrDefault();
                try
                {
                    //如果都不是空的
                    if (userId is not null && !string.IsNullOrEmpty(UserId))
                    {
                        //Base64解码
                        var enCoder = Convert.FromBase64String(UserId);
                        UserId = Encoding.UTF8.GetString(enCoder);
                        //匹配是否一样
                        if (userId.ToString() != UserId)
                        {
                            //不一样 就是有人恶意篡改Token或者Headers["UserId"] 重新登录
                            context.Response.StatusCode = 401;
                            return;
                        }
                    }//如果 Token不是空的而前端传的UserId 是空的重新登录
                    else if (string.IsNullOrEmpty(UserId) && userId is not null)
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                }
                catch (Exception)
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }         
            await _next.Invoke(context);            
        }
    }
    public static class CheckTokenAndUserIdExtensions
    {
        public static IApplicationBuilder UseCheckTokenAndUserId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckTokenAndUserId>();
        }
    }
}
