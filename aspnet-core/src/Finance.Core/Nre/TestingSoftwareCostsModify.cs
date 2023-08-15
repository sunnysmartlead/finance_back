using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// 测试软件费用 修改项 实体类
    /// </summary>
    public class TestingSoftwareCostsModify : FullAuditedEntity<long>
    {
        /// <summary>
        /// 修改项的id
        /// </summary>
        public long ModifyId { get; set; }
        /// <summary>
        /// 软件项目
        /// </summary>
        public string SoftwareProject { get; set; }
        /// <summary>
        /// 费用/H
        /// </summary>
        public decimal CostH { get; set; }
        /// <summary>
        /// 小时
        /// </summary>
        public decimal Hour { get; set; }
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
