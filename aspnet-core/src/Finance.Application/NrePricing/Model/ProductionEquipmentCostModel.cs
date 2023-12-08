using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
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
        /// 需求：名称和单价一样的话要合并数据  这个Ids 就是储存合并后所有数据的id
        /// </summary>
        public HashSet<long> Ids { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
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
        /// 设备状态
        /// </summary>        
        public string DeviceStatus { get; set; }
        /// <summary>
        /// 设备状态名称
        /// </summary>        
        public string DeviceStatusName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
