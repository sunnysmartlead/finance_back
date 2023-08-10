using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.DTo
{
    /// <summary>
    /// 计算 项目看板 通过  毛利率 推其他
    /// </summary>
    public class SpreadSheetCalculateDto
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 报价的值
        /// </summary>
        public decimal Value { get; set; }
    }
}
