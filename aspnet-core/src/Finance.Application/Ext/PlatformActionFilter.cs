using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{

    /// <summary>
    /// action方法过滤器
    /// </summary>
    public class PlatformActionFilter : Attribute, IActionFilter
    {
        private static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        private readonly IAbpSession _abpSession;


        public PlatformActionFilter()
        {
            _abpSession = IocManager.Instance.Resolve<IAbpSession>(); ;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
        /// <summary>
        /// action 执行之前
        /// </summary>
        /// <param name="filterContext"></param>
        /// <exception cref="FriendlyException"></exception>
        public virtual void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string httpMethod = WebUtility.HtmlEncode(filterContext.HttpContext.Request.Method);
            if (httpMethod == "POST")
            {
                //使用请求路径作为唯一key
                string path = filterContext.HttpContext.Request.Path;
                string cacheToken = $"{_abpSession.UserId}_{path}";
                string keyValue = new Guid().ToString() + DateTime.Now.Ticks;
                if (path != null)
                {
                    //var cache = iZen.Utils.Core.iCache.CacheManager.GetCacheValue(cacheToken);
                    var cv = cache.Get(cacheToken);
                    if (cv == null)
                    {
                        //iZen.Utils.Core.iCache.CacheManager.SetChacheValueSeconds(cacheToken, keyValue, 1);
                        //设置缓存1秒过期
                        cache.Set(cacheToken, keyValue, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromSeconds(3) });                     
                    }
                    else
                    {
                        throw new FriendlyException($"您重复提交了数据！--PlatformActionFilter");
                    }
                }
                return;
            }
            this.OnActionExecuting(filterContext);
        }

    }
}
