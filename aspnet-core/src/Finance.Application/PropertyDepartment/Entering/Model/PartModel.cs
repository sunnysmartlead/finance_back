using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Model
{
    /// <summary>
    /// 方案 模型
    /// </summary>
    public class SolutionModel
    {
        /// <summary>
        /// 方案的id
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 零件ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 方案的名称
        /// </summary>
        public string SolutionName { get; set; }
    }
}
