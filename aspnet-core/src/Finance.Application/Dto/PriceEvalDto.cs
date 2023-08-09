using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Dto
{
    /// <summary>
    /// 核价表Dto
    /// </summary>
    public class PriceEvalDto
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        public virtual long? AuditFlowId { get; set; }

        /// <summary>
        /// 模组数量Id（即零件Id）
        /// </summary>
        public virtual long? ProductId { get; set; }

    }
}
