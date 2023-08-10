using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// Sop 走量模型
    /// </summary>
    public class SopModel
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 走量
        /// </summary>
        public decimal Motion { get; set; }
        /// <summary>
        /// 年将率
        /// </summary>
        public decimal YearDrop { get; set; }
        /// <summary>
        /// 年度返利要求
        /// </summary>
        public decimal RebateRequest { get; set; }
        /// <summary>
        /// 一次性折让
        /// </summary>
        public decimal DiscountRate { get; set; }
    }
}
