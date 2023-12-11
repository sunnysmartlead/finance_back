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
    /// 工装费用 修改项 实体类
    /// </summary>
    [Table("NRE_TModify")]
    public class ToolingCostsModify : FullAuditedEntity<long>
    {
        /// <summary>
        /// 费用类型 判断是工装费用还是测试线费用  1是工装费用2是测试线费用
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
