using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// 录入营销部 交互类
    /// </summary>
    public class InitialSalesDepartmentDto: ReturnSalesDepartmentDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }        
    }
}
