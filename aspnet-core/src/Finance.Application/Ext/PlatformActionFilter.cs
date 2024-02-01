using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{

    /// <summary>
    /// action方法过滤器 其中包含lock锁 他能确保对共享资源的独占访问权限,但是也使接口变成了同步并非异步,会使接口的访问时间变长
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PlatformActionFilterlock : ActionFilterAttribute, IActionFilter
    {
        private readonly ICacheManager _cacheManager;
        private readonly IAbpSession _abpSession;

        /// <summary>
        /// 
        /// </summary>
        public PlatformActionFilterlock()
        {
            _abpSession = IocManager.Instance.Resolve<IAbpSession>();
            _cacheManager = IocManager.Instance.Resolve<ICacheManager>(); ;
        }
        /// <summary>
        /// action 执行之后
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {

        }
        /// <summary>
        /// action 执行之前
        /// </summary>
        /// <param name="filterContext"></param>
        /// <exception cref="FriendlyException"></exception>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                string httpMethod = WebUtility.HtmlEncode(filterContext.HttpContext.Request.Method);
                lock (_cacheManager)
                {
                    //使用请求路径作为唯一key
                    string path = filterContext.HttpContext.Request.Path;
                    var cacheJson = JsonConvert.SerializeObject(path);
                    var code = cacheJson.GetHashCode().ToString();
                    string cacheToken = $"{_abpSession.UserId}_{code}";
                    var cache = _cacheManager.GetCache(code).GetOrDefault(cacheToken);
                    if (cache is null)
                    {
                        _cacheManager.GetCache(code).Set(cacheToken, cacheToken, new TimeSpan(0, 0, 1));
                    }
                    else
                    {
                        throw new Exception($"您重复提交了数据！--PlatformActionFilter");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}--PlatformActionFilter");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
