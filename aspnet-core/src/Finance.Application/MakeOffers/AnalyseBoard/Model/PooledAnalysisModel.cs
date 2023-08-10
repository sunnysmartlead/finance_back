using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 报价分析看板返回实体类=>汇总分析表 模型
    /// </summary>
    public class PooledAnalysisModel : GrossMarginListModel
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
       
    }
}
