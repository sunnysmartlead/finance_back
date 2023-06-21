﻿using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.UpdateLog
{
    /// <summary>
    /// 更新日志表
    /// </summary>
    public class UpdateLogInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 版本号id
        /// </summary>
        public long VersionsId { get; set; }
        /// <summary>
        /// 日志标识
        /// </summary>
        public string Identify { get; set; }
        /// <summary>
        /// 更新内容
        /// </summary>
        public string Content  { get; set; }
        /// <summary>
        /// 所属模块
        /// </summary>
        public string Module { get; set; }

    }
}
