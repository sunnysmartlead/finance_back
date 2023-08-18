using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// Nre 测试软件费用 模型
    /// </summary>
    public class SoftwareTestingCotsModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
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
        public decimal Hour  { get; set; }
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
