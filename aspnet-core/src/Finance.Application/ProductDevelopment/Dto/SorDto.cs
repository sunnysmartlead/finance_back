using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProductDevelopment.Dto
{
    public class SorDto
    {
        /// <summary>
        /// 审核流程Id
        /// </summary>
        [Required]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 客户特殊性需求
        /// </summary>
        public string CustomerSpecialRequest { get; set; }
        /// <summary>
        /// SOR文件ID
        /// </summary>
        public long SorFileId { get; set; }
        /// <summary>
        /// SOR文件名称
        /// </summary>
        public string SorFileName { get; set; }
        /// <summary>
        /// 是否成功标志位
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }
    }
}
