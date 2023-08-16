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
        public decimal Coefficient { get; set; }
        /// <summary>
        /// 计价单位
        /// </summary>
        public decimal Unit { get; set; }
        /// <summary>
        /// 时间-摸底
        /// </summary>
        public decimal DataThoroughly { get; set; }
        /// <summary>
        /// 时间-DV
        /// </summary>
        public decimal DataDV { get; set; }
        /// <summary>
        /// 时间-PV
        /// </summary>
        public decimal DataPV { get; set; }
        /// <summary>
        /// 数量=摸底+DV+PV+单位 后面需要带单位所以是 string
        /// </summary>
        public string Count { get; set; }      
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
