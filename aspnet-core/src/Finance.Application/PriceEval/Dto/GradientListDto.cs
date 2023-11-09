using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class GradientListDto : EntityDto<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 核价表主键
        /// </summary>
        public virtual long PriceEvaluationId { get; set; }

        /// <summary>
        /// 梯度序号
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// 梯度(K/Y)
        /// </summary>
        public virtual decimal GradientValue { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }
    }
}
