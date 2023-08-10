using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.UpdateLog
{
    /// <summary>
    /// 版本表
    /// 版本号规则：主版本号.次版本号.修订版本号
    /// 确认为系统bug，则当天修复，当天发版，修订版本号 加 1。
    /// 如果 .csproj 文件有变更，则当天发版，修订版本号 加 1。
    /// 如果新增 拓展包，为了版本号统一，则当天发版，修订版本号 加 1。
    /// 如果涉及到代码重构，则当天发版，次版本号 加 1，修订版本号 清 0。
    /// 如果进行二次开发 主版本号升级，则当天发版，主版本号 加 1。
    /// </summary>
    public class Versions: AuditedEntity<long>
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNumber { get; set; }
        /// <summary>
        /// 环境标识
        /// </summary>
        public string Identify { get; set; }
        /// <summary>
        /// 是否正常
        /// </summary>
        public bool IsStart { get; set; }     
    }
}
