using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 生成核价表返回的信息
    /// </summary>
    public class CreatePriceEvaluationTableResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public virtual bool IsSuccess { get; set; }

        /// <summary>
        /// 返回消息（成功则为“添加成功”，失败则为失败原因）
        /// </summary>
        public virtual string Message { get; set; }
    }
}
