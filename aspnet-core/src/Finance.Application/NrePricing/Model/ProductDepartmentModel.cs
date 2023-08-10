using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// 产品部-电子工程师 录入 实验费  模型
    /// </summary>
    public class ProductDepartmentModel
    {
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }


        /// <summary>
        /// 方案表ID
        /// </summary>
        public long SolutionId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SolutionNum { get; set; }


        /// <summary>
        /// 实验费 模型
        /// </summary>
        public List<LaboratoryFeeModel> laboratoryFeeModels { get; set; }
    }
}
