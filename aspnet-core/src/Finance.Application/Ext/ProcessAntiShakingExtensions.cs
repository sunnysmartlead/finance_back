using Abp.Dependency;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    public static class ProcessAntiShakingExtensions
    {
        /// <summary>
        /// 提交防抖
        /// </summary>
        /// <param name="name"></param>
        /// <param name="object"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async static Task ProcessAntiShaking(this object @object,string name)
        {
            ICacheManager _cacheManager= IocManager.Instance.Resolve<ICacheManager>();
            #region 流程防抖
            var cacheJson = JsonConvert.SerializeObject(@object);
            var code = cacheJson.GetHashCode().ToString();
            var cache = await _cacheManager.GetCache(name).GetOrDefaultAsync(code);
            if (cache is null)
            {
                await _cacheManager.GetCache(name).SetAsync(code, code, new TimeSpan(0, 0, 5));
            }
            else
            {
                throw new FriendlyException($"您重复提交了数据！");
            }
            #endregion
        }
    }
}
