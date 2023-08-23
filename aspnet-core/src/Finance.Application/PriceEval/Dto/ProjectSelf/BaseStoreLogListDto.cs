using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Application.Services.Dto;

namespace Finance.PriceEval.Dto.ProjectSelf
{
    /// <summary>
    /// 基础库日志返回
    /// </summary>
    public class BaseStoreLogListDto : CreationAuditedEntityDto<long>
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public virtual OperationType OperationType { get; set; }

        /// <summary>
        /// 操作的记录数量
        /// </summary>
        public virtual long Count { get; set; }

        /// <summary>
        /// 文本说明
        /// </summary>
        public virtual string Text { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public virtual string UserName { get; set; }
    }
}
