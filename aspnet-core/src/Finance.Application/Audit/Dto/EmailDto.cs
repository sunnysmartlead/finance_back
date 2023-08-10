using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Audit.Dto
{
    /// <summary>
    /// 邮箱相关的Dto
    /// </summary>
    public class EmailDto
    {
        public long Id { get; set; }    
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string EmailPassword { get; set; }
        /// <summary>
        /// 密码维护人邮箱地址
        /// </summary>
        public string MaintainerEmail { get; set; }
    }
}
