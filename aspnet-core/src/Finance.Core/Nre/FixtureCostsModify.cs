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
    /// 治具费用修改项实体类
    /// </summary>
    [Table("NRE_FCModify")]
    public class FixtureCostsModify : FullAuditedEntity<long>
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
        /// 治具名称
        /// </summary>
        public string ToolingName { get; set; }
        /// <summary>
        /// 治具单价
        /// </summary> 
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 治具数量
        /// </summary> 
        public int Number { get; set; }
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
