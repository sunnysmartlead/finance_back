using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 核价看板审核输入Dto
    /// </summary>
    public class PriceBoardCheckDto
    {
        /// <summary>
        /// 审核流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 核价看板审核意见
        /// </summary>
        public bool IsAgree { get; set; }
        /// <summary>
        /// 审核意见说明
        /// </summary>
        public string OpinionDescription { get; set; }
        /// <summary>
        /// 退回列表
        /// </summary>
        public List<string> BackProcessIdentifiers { get; set; }
    }
}
