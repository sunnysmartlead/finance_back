using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Model
{
    /// <summary>
    /// Sop 模型
    /// </summary>
    public class SopOrValueMode
    {
        /// <summary>
        /// SopKey
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public decimal Value { get; set; }
        /// <summary>
        /// sop值
        /// </summary>
        public decimal sopValue { get; set; }
        /// <summary>
        /// quan值
        /// </summary>
        public decimal fullValue { get; set; }
        
    }
}
