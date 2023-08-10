using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Ext
{
    /// <summary>
    /// 系统日志
    /// </summary>
    public class SystemLog
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// 请求Id
        /// </summary>
        public virtual string RequestId { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public virtual string Path { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public virtual string Arguments { get; set; }

        /// <summary>
        /// 请求来源
        /// </summary>
        public virtual string Referer { get; set; }

        /// <summary>
        /// 请求IP
        /// </summary>
        public virtual string Ip { get; set; }

        /// <summary>
        /// 调用方法名
        /// </summary>
        public virtual string MethodName { get; set; }

        /// <summary>
        /// 程序集信息
        /// </summary>
        public virtual string TargetType { get; set; }

        /// <summary>
        /// 用户代理信息（操作系统/浏览器等）
        /// </summary>
        public virtual string UserAgent { get; set; }

        /// <summary>
        /// 返回状态码
        /// </summary>
        public virtual int ResponseStatusCode { get; set; }

        ///// <summary>
        ///// 返回值
        ///// </summary>
        //public virtual string ReturnValue { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        public virtual bool IsException { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        public virtual string ExceptionMessage { get; set; }

        /// <summary>
        /// 异常堆栈
        /// </summary>
        public virtual string ExceptionStackTrace { get; set; }
    }
}
