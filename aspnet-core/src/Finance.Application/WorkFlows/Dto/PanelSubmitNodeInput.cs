using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.WorkFlows.Dto
{
    /// <summary>
    /// 核价看板专用接口输入Dto
    /// </summary>
    public class PanelSubmitNodeInput
    {
        /// <summary>
        /// 节点实例Id
        /// </summary>
        public virtual long NodeInstanceId { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public virtual List<string> FinanceDictionaryDetailIds { get; set; }

        /// <summary>
        /// 审批评论
        /// </summary>
        public virtual string Comment { get; set; }
    }
}
