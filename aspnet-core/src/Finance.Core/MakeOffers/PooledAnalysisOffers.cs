using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    /// 报价分析看板中的 汇总分析表  实体类
    /// </summary>
    public class PooledAnalysisOffers : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 毛利率 存json  模型为 GrossMarginModel
        /// </summary>
        public string GrossMarginList { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }
    }
}