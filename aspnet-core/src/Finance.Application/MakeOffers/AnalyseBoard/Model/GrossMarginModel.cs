using Finance.MakeOffers.AnalyseBoard.Method;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// 报价分析毛利率 模型
    /// </summary>

    public class GrossMarginModel
    {
        private decimal grossMargin;  
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossMargin
        {
            get { return grossMargin.GetDecimal(2); }   // get 方法
            set { grossMargin = value; }  // set 方法
        }
        private decimal grossMarginNumber;
        /// <summary>
        /// 毛利率值
        /// </summary>
        public decimal GrossMarginNumber
        {
            get { return grossMarginNumber.GetDecimal(2); }   // get 方法
            set { grossMarginNumber = value; }  // set 方法
        }

    }
}
