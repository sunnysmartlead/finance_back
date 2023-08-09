using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// 差旅费 实体类
    /// </summary>
    public class TravelExpense : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }



        /// <summary>
        /// 事由外键
        /// </summary>
        public string ReasonsId { get; set; }      
        /// <summary>
        /// 人数
        /// </summary>
        public int PeopleCount { get; set; }
        /// <summary>
        /// 费用/天
        /// </summary>
        public decimal CostSky { get; set; }
        /// <summary>
        /// 天数
        /// </summary>
        public int SkyCount { get; set; }
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
