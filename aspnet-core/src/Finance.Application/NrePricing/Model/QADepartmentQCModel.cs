using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// Nre 品保部 录入 项目制程QC 表
    /// </summary>
    public class QADepartmentQCModel
    {
        /// <summary>
        /// 需求：名称和单价一样的话要合并数据  这个Ids 就是储存合并后所有数据的id
        /// </summary>
        public HashSet<long> Ids { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
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
        /// 使用工站
        /// </summary>
        public string UseWorkstation { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
