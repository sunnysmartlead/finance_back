using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit
{
    /// <summary>
    /// 通信邮箱信息
    /// </summary>
    public class NoticeEmailInfo : FullAuditedEntity<long>
    {
        /// <summary>
        /// 通知邮箱地址
        /// </summary>
        [Column("EMAILADDRESS")]
        public string EmailAddress { get; set; }
        /// <summary>
        /// 通知邮箱密码
        /// </summary>
        [Column("EMAILPASSWORD")]
        public string EmailPassword { get; set; }

        /// <summary>
        /// 维护人邮箱
        /// </summary>
        [Column("MAINTAINEREMAIL")]
        public string MaintainerEmail { get; set; }
    }
}
