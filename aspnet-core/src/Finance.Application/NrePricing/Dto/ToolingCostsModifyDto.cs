using Finance.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// 工装费用 修改项 交互类
    /// </summary>
    public class ToolingCostsModifyDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long  Id { get; set; }
        /// <summary>
        /// 流程号Id
        /// </summary> 
        [FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)]
        public long SolutionId { get; set; }
        /// <summary>
        /// 修改项的id
        /// </summary>
        public long ModifyId { get; set; }
        /// <summary>
        /// 事由外键
        /// </summary>
        public string ReasonsId { get; set; }
        /// <summary>
        /// 工装名称
        /// </summary>      
        public string WorkName { get; set; }
        /// <summary>
        /// 工装单价
        /// </summary> 
        public decimal UnitPriceOfTooling { get; set; }
        /// <summary>
        /// 工装数量
        /// </summary> 
        public int ToolingCount { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
