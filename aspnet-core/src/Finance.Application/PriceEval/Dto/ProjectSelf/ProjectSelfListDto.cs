using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace Finance.PriceEval.Dto.ProjectSelf
{
    /// <summary>
    /// 项目自建表返回的结果
    /// </summary>
    public class ProjectSelfListDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// 客户
        /// </summary>
        public virtual string Custom { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public virtual string CustomName { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 子项目代码
        /// </summary>
        public virtual string SubCode { get; set; }

        /// <summary>
        /// 子项目描述
        /// </summary>
        public virtual string SubDescription { get; set; }
    }
}
