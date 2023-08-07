using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// 模具清单 模型
    /// </summary>
    public class MouldInventoryModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 结构BOM Id
        /// </summary>
        public long StructuralId { get; set; }      
        /// <summary>
        /// 模具名称
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// 模穴数
        /// </summary>
        public int MoldCavityCount { get; set; }
        /// <summary>
        /// 模次数
        /// </summary>
        public int ModelNumber { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public double Count { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 提交人名称
        /// </summary>
        public string PeopleName { get; set; }
        /// <summary>
        /// 提交人 Id
        /// </summary>
        public long PeopleId { get; set; }
        /// <summary>
        /// 是否提交 true/1 提交  false/0 未提交
        /// </summary>
        public bool IsSubmit { get; set; }
    }
}
