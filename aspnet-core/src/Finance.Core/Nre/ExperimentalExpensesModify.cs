using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// 实验费用 修改项 实体类
    /// </summary>
    [Table("NRE_EEModify")]
    public class ExperimentalExpensesModify : FullAuditedEntity<long>
    {
        /// <summary>
        /// 实验费类型 判断是EMC实验费还是环境实验费  1是EMC实验费2是环境实验费
        /// </summary>
        public int ExpenseType { get; set; }
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 修改项的id
        /// </summary>
        public long ModifyId { get; set; }
        /// <summary>
        /// 试验项目(根据与客户协定项目)
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 是否指定第三方
        /// </summary>
        public bool IsThirdParty { get; set; }
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
