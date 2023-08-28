using Castle.MicroKernel.SubSystems.Conversion;
using Finance.Ext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// 实验费用 修改项 交互类
    /// </summary>
    public class ExperimentalExpensesModifyDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
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
        /// 实验项目
        /// </summary>
        public string TestItem { get; set; }
        /// <summary>
        /// 是否指定第三方 (是 true   否 false)
        /// </summary>
        public bool IsThirdParty { get; set; }
        /// <summary>
        /// 是否指定第三方 (true/是 false/否)
        /// </summary>
        public string IsThirdPartyName { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 调整系数
        /// </summary>
        public decimal AdjustmentCoefficient { get; set; }
        /// <summary>
        /// 计价单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 计数-摸底
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CountBottomingOut { get; set; }
        /// <summary>
        /// 计数-DV
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CountDV { get; set; }
        /// <summary>
        /// 计数-PV
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CountPV { get; set; }
        /// <summary>
        /// 总费用
        /// </summary>
        public decimal AllCost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
