using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.NrePricing.Model
{
    /// <summary>
    /// 带零件id 的 品保录入模型
    /// </summary>
    public class QADepartmentPartModel
    {
        /// <summary>
        /// 零件的id
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        ///品保录入  试验项目表 模型
        /// </summary>
        public List<EnvironmentalExperimentFeeModel> EnvironmentalExperimentFees { get; set; }
        /// <summary>
        /// 品保录入  项目制程QC量检具表 模型
        /// </summary>
        public List<QADepartmentQCModel>  QAQCDepartments { get; set; }
    }
}
