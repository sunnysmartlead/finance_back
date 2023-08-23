using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// 生产设备费用 修改项交互类
    /// </summary>    
    public class ProductionEquipmentCostsModifyDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 修改项的id
        /// </summary>
        public long ModifyId { get; set; }
        /// <summary>
        /// 生产设备名
        /// </summary>     
        public string EquipmentName { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>        
        public string DeviceStatus { get; set; }
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
