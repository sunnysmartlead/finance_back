using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// 治具费用修改项实体类
    /// </summary>
    public class FixtureCostsModify : FullAuditedEntity<long>
    {
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
