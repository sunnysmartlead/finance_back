using Finance.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProjectManagement
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static class ConfigHelper
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    {
        private static IConfigurationRoot _appConfiguration = AppConfigurations.Get(System.Environment.CurrentDirectory);

        //用法1(有嵌套)：GetAppSetting("Authentication", "JwtBearer:SecurityKey")
        //用法2：GetAppSetting("App", "ServerRootAddress")
        public static string GetAppSetting(string section, string key)
        {
            return _appConfiguration.GetSection(section)[key];
        }

        public static string GetConnectionString(string key)
        {
            return _appConfiguration.GetConnectionString(key);
        }


    }

}
