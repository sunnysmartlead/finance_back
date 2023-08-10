using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment.Dto
{
    public class _3DFileDto
    {
        /// <summary>
        /// 审核流程Id
        /// </summary>
        [Required]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 零件号
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 客户特殊性需求
        /// </summary>
        public string CustomerSpecialRequest { get; set; }
        /// <summary>
        /// 3D爆炸图文件ID
        /// </summary>
        public long ThreeDFileId { get; set; }
        /// <summary>
        /// 3D爆炸图文件名称
        /// </summary>
        public string ThreeDFileName { get; set; }
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
