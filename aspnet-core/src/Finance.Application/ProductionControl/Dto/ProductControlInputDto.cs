using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProductionControl.Dto
{
    /// <summary>
    /// 物流成本保存Dto
    /// </summary>
    public class ProductControlInputDto
    {
        /// <summary>
        /// 物流成本列表
        /// </summary>
        public List<ProductionControlInfo> InfoList { get; set; }
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 零件Id
        /// </summary>
        public long ProductId { get; set; }
    }

    /// <summary>
    /// 零件信息Dto
    /// </summary>
    public class ProductInfoDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 零件Id
        /// </summary>
        public long ProductId { get; set; }
    }
}
