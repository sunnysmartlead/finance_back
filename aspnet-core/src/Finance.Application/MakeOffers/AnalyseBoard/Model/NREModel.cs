using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 报价审核表 Nre费用模型
    /// </summary>
    public class NREModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string NreName { get; set; }
        /// <summary>
        /// 费用名称
        /// </summary>
        public string CostName { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public string Cost { get; set; }
 
    }
}
