using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 核价看板产品选择下拉框下拉数据接口 输入参数
    /// </summary>
    public class GetPricingPanelTimeSelectListInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }
    }
}
