using Finance.NrePricing.Model;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    ///Nre核价  资源部录入(初始值) 交互类
    /// </summary>
    public class InitialResourcesManagementDto
    {
        /// <summary>
        /// 零件
        /// </summary>
        public List<SolutionModel> partModels { get; set; }
        /// <summary>
        ///  Nre核价 带 零件 id 的模具清单 模型
        /// </summary>
        public List<MouldInventoryPartModel> mouldInventoryModels { get; set; }
    }
}
