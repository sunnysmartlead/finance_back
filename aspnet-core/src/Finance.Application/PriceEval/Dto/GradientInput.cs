using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 梯度
    /// </summary>
    public class GradientInput
    {
        /// <summary>
        /// 梯度序号
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// 梯度(K/Y)
        /// </summary>
        public virtual decimal GradientValue { get; set; }

        /// <summary>
        /// 显示用梯度
        /// </summary>
        public virtual decimal DisplayGradientValue { get; set; }

        /// <summary>
        /// 系统取梯度（废弃）
        /// </summary>
        public virtual decimal SystermGradientValue { get; set; }
    }
}
