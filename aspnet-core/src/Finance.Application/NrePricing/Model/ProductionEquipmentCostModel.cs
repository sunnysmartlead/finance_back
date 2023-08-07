using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// Nre 生产设备费用 模型
    /// </summary>
    public class ProductionEquipmentCostModel
    {
        /// <summary>
        /// 生产设备名
        /// </summary>     
        public string EquipmentName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>        
        public int Number { get; set; }
        /// <summary>
        /// 单价
        /// </summary>      
        public decimal UnitPrice { get; set; }
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
