using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// 带 零件 id 的模具清单 模型
    /// </summary>
    public class MouldInventoryPartModel
    {
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        ///  Nre核价 模组清单模型
        /// </summary>
        public List<MouldInventoryModel> MouldInventoryModels { get; set; }
       
    }
}
