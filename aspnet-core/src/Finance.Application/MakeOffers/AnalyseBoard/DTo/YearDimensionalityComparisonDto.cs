using Finance.Dto;
using Finance.MakeOffers.AnalyseBoard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.DTo
{
    /// <summary>
    /// 年份维度对比 交互类
    /// </summary>
    public class YearDimensionalityComparisonDto
    {
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossMargin { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string Project { get; set; }
        /// <summary>
        /// Sop
        /// </summary>
        public List<SopOrValueMode> YearList { get; set; }
        /// <summary>
        /// 总和
        /// </summary>
        public decimal Totak { get; set; }
    }
}
