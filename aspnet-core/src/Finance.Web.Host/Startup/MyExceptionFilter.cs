using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.Web.Configuration;
using Abp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Web.Host.Startup
{
    public class MyExceptionFilter : AbpExceptionFilter
    {
        public ILogger Logger { get; set; }

        public MyExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IAbpAspNetCoreConfiguration configuration, IAbpWebCommonModuleConfiguration abpWebCommonModuleConfiguration) : base(errorInfoBuilder, configuration, abpWebCommonModuleConfiguration)
        {
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get http code
        /// </summary>
        /// <param name="context"></param>
        /// <param name="wrapOnError"></param>
        /// <returns></returns>
        protected override int GetStatusCode(ExceptionContext context, bool wrapOnError)
        {
            var customException = context.Exception as FriendlyException;
            if (customException != null)
            {
                return customException.HttpCode;
            }
            else
            {
                return base.GetStatusCode(context, wrapOnError);
            }
        }
    }
}
