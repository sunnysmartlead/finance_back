using Abp.Extensions;
using Abp.ObjectMapping;
using Castle.MicroKernel;
using Finance.Dto;
using NPOI.SS.Formula.Functions;
using Rougamo;
using Rougamo.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Finance.Ext;
using Abp.Domain.Repositories;
using Finance.PriceEval;
using Finance.Audit;
using Abp.Dependency;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using MiniExcelLibs;
using Microsoft.Extensions.Configuration;
using Finance.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Abp.Runtime.Session;
using NPOI.HPSF;
using MiniExcelLibs.Utils;

namespace Finance.Ext
{
    /// <summary>
    /// 统一参数验证和统一异样处理
    /// </summary>
    public class ParameterValidatorAttribute : MoAttribute
    {

        private readonly IRepository<ModelCount, long> _modelCountRepository;
        private readonly IRepository<AuditFlow, long> _auditFlowRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfigurationRoot _appConfiguration;
        public IAbpSession AbpSession { get; set; }
        public ParameterValidatorAttribute()
        {
#if !DEBUG
            _modelCountRepository = IocManager.Instance.Resolve<IRepository<ModelCount, long>>();
            _auditFlowRepository = IocManager.Instance.Resolve<IRepository<AuditFlow, long>>();
            _httpContextAccessor = IocManager.Instance.Resolve<IHttpContextAccessor>();
            var env = IocManager.Instance.Resolve<IWebHostEnvironment>();
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
            AbpSession = NullAbpSession.Instance;
#endif
        }

        public override AccessFlags Flags => AccessFlags.Public;

        /// <summary>
        /// 读取文件保存的根目录
        /// </summary>
        /// <returns></returns>
        internal static string GetFileSaveRootDir(IConfigurationRoot appConfiguration)
        {
            string fileSaveRootDir = appConfiguration["FilePathManage:SystemLogPath"];
            if (!Directory.Exists(fileSaveRootDir))
            {
                Directory.CreateDirectory(fileSaveRootDir);
            }
            return fileSaveRootDir;
        }

        private string Log(MethodContext context)
        {
            #region 统一日志
            if (!_httpContextAccessor.HttpContext.Request.Headers.RequestId.Any())
            {
                _httpContextAccessor.HttpContext.Request.Headers.RequestId = Guid.NewGuid().ToString();
            }

            var requestId = _httpContextAccessor.HttpContext.Request.Headers.RequestId;
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var arguments = JsonConvert.SerializeObject(context.Arguments);
            var referer = _httpContextAccessor.HttpContext.Request.Headers.Referer;
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent;
            var path = _httpContextAccessor.HttpContext.Request.Path;
            //var returnValue = JsonConvert.SerializeObject(context.ReturnValue);
            var methodName = context.Method.Name;
            var targetType = context.TargetType.FullName;

            var responseStatusCode = _httpContextAccessor.HttpContext.Response.StatusCode;

            var systemLog = new SystemLog
            {
                CreationTime = DateTime.Now,
                RequestId = requestId,
                Path = path,
                Arguments = arguments,
                Referer = referer,
                Ip = ip,
                UserAgent = userAgent,
                //ReturnValue = returnValue,
                ResponseStatusCode = responseStatusCode,
                MethodName = methodName,
                TargetType = targetType
            };
            if (context.Exception is not null)
            {
                systemLog.ExceptionMessage = context.Exception.Message;
                systemLog.ExceptionStackTrace = context.Exception.StackTrace;
                systemLog.IsException = true;
            }
            else
            {
                systemLog.IsException = false;
            }

            //读取文件保存的根目录
            string fileSaveRootDir = GetFileSaveRootDir(_appConfiguration);

            var filePath = $"{fileSaveRootDir}{DateTime.Now:yyyy-MM-dd}{FinanceConsts.SystemLogFileType}";
            if (File.Exists(filePath))
            {
                //以此方式调用，避免MiniExcel.Insert调用，产生文件被进程占用导致写入失败。
                using FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.SequentialScan);
                stream.Insert(new[] { systemLog }, "Sheet1", ExcelType.CSV, null);

                //MiniExcel.Insert(filePath, new[] { systemLog });
            }
            else
            {
                MiniExcel.SaveAs(filePath, new[] { systemLog });
            }

            return requestId;

            #endregion
        }

        [ParameterValidator]
        public override void OnEntry(MethodContext context)
        {
            #region 统一参数验证
            var args = context.Arguments.FirstOrDefault(p => p.GetType().IsClass)?.AutoMapping<PriceEvalDto>();
            if (context.Method.Name != "PriceEvaluationStart")
            {
                if (args is not null)
                {
                    if (args.AuditFlowId.HasValue)
                    {
                        var af = _auditFlowRepository.GetAll().Any(p => p.Id == args.AuditFlowId.Value);
                        if (!af)
                        {
                            throw new FriendlyException($"流程Id【{args.AuditFlowId}】不存在。");
                        }
                    }

                    if (args.ProductId.HasValue)
                    {
                        var md = _modelCountRepository.GetAll().Any(p => p.Id == args.ProductId.Value);
                        if (!md)
                        {
                            throw new FriendlyException($"模组Id【{args.ProductId}】不存在。");
                        }
                    }

                    if (args.AuditFlowId.HasValue && args.ProductId.HasValue)
                    {
                        var md = _modelCountRepository.GetAll().Any(p => p.AuditFlowId == args.AuditFlowId.Value && p.Id == args.ProductId.Value);
                        if (!md)
                        {
                            throw new FriendlyException($"模组Id【{args.ProductId}】在流程Id【{args.AuditFlowId}】中不存在。");
                        }
                    }
                }
            }
            #endregion
        }

        public override void OnException(MethodContext context)
        {
            string requestId = Log(context);
            throw new FriendlyException($"{context.Exception.Message}【{requestId}】", context.Exception.StackTrace);
        }

        public override void OnSuccess(MethodContext context)
        {
        }

        public override void OnExit(MethodContext context)
        {
            Log(context);
        }
    }
}
