using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    public class SubmitNodeInput : ISubmitNodeInput
    {
        /// <summary>
        /// 节点实例Id
        /// </summary>
        public virtual long NodeInstanceId { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public virtual string FinanceDictionaryDetailId { get; set; }

        /// <summary>
        /// 审批评论
        /// </summary>
        public virtual string Comment { get; set; }
    }

    public interface ISubmitNodeInput
    {
        /// <summary>
        /// 节点实例Id
        /// </summary>
        public long NodeInstanceId { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string FinanceDictionaryDetailId { get; set; }

        /// <summary>
        /// 审批评论
        /// </summary>
        public string Comment { get; set; }
    }
}
