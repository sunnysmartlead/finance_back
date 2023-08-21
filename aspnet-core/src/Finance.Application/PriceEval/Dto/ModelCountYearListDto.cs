using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 模组数量年份
    /// </summary>
    public class ModelCountYearListDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 主表 模组数量（ModelCount） 的Id
        /// </summary>
        public virtual long ProductId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public virtual decimal Quantity { get; set; }

        /// <summary>
        /// 核价表Json
        /// </summary>
        public virtual string TableJson { get; set; }
    }
}
