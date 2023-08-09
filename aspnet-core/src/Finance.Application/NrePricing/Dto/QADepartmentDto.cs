using Finance.NrePricing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// Nre 品保部录入交互类
    /// </summary>
    public class QADepartmentDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 带零件id 的 品保录入模型
        /// </summary>
        public List<QADepartmentPartModel>  QADepartments { get; set; }
    }
}
