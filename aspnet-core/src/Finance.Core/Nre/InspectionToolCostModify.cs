using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// 检具费用  修改项实体类
    /// </summary>
    public class InspectionToolCostModify : FullAuditedEntity<long>
    {
        /// <summary>
        /// 修改项的id
        /// </summary>
        public long ModifyId { get; set; }
        /// <summary>
        /// 项目制程QC量检具
        /// </summary>
        public string Qc { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
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
