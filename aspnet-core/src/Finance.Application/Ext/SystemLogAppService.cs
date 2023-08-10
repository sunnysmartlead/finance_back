using Finance.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.IO;
using MiniExcelLibs;
using Abp.Application.Services.Dto;
using Finance.Ext.Dto;
using Abp.Collections.Extensions;
using Abp.Extensions;

namespace Finance.Ext
{
    public class SystemLogAppService : FinanceAppServiceBase
    {
        private readonly IConfigurationRoot _appConfiguration;
        public SystemLogAppService(IWebHostEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }


        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<PagedResultDto<SystemLog>> GetAll(SystemLogGetInput input)
        {
            //读取文件保存的根目录
            string fileSaveRootDir = ParameterValidatorAttribute.GetFileSaveRootDir(_appConfiguration);

            string filePath;
            if (input.CreationTime.HasValue)
            {
                filePath = $"{fileSaveRootDir}{input.CreationTime:yyyy-MM-dd}{FinanceConsts.SystemLogFileType}";
            }
            else
            {
                filePath = $"{fileSaveRootDir}{DateTime.Now:yyyy-MM-dd}{FinanceConsts.SystemLogFileType}";
            }
            if (File.Exists(filePath))
            {
                var data = MiniExcel.Query<SystemLog>(filePath)
                         .WhereIf(!input.RequestId.IsNullOrWhiteSpace(), p => p.RequestId == input.RequestId)
                         .WhereIf(input.IsException.HasValue, p => p.IsException == input.IsException)
                         .Skip(input.SkipCount).Take(input.MaxResultCount);
                return new PagedResultDto<SystemLog>(data.Count(), data.ToList());
            }
            else
            {
                return new PagedResultDto<SystemLog>(0, null);
            }
        }
    }
}
