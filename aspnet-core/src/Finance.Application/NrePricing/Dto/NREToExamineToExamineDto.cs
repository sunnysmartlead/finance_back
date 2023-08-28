using Finance.Dto;
using Finance.Ext;
using Finance.ProductDevelopment.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// NRE 审核接交互类
    /// </summary>
    public class NREToExamineToExamineDto : ToExamineDto
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        [FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 审核界面类型（1：“模具费审核”，2：“环境试验费审核”，3：“EMC试验费审核”）
        /// </summary>
        public NRECHECKTYPE NreCheckType { get; set; }
        /// <summary>
        /// 回退单个的id,//只有模具费 需要 单挑退回其余都不需要 可传空
        /// </summary>
        public List<long> NreId { get; set; }
    }
    /// <summary>
    /// 表示BNRE审核界面类型（1：“模具费审核”，2：“环境试验费审核”，3：“EMC试验费审核”）
    /// </summary>
    public enum NRECHECKTYPE
    {
        /// <summary>
        /// 模具费
        /// </summary>
        [Description("模具费")]
        MoldCost = 1,
        /// <summary>
        /// 环境试验费 
        /// </summary>
        [Description("环境试验费 ")]
        EnvironmentalTestingFee = 2,
        /// <summary>
        ///  EMC试验费
        /// </summary>
        [Description("EMC试验费 ")]
        EMCTestFee = 3,
    }
}
