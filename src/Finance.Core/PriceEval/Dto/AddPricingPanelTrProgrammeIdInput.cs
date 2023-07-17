using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 添加核价表TR方案Id 方法输入
    /// </summary>
    public class AddPricingPanelTrProgrammeIdInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 文件管理表Id
        /// </summary>
        [Required]
        public virtual long FileManagementId { get; set; }
    }
}
