using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// 差旅费 模型
    /// </summary>
    public class TravelExpenseModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 事由外键
        /// </summary>
        public string ReasonsId { get; set; }
        /// <summary>
        /// 事由名称
        /// </summary>
        public string ReasonsName { get; set; }
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
