using Finance.Ext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Dto
{
    /// <summary>
    /// /审批方法通用交互类
    /// </summary>
    public class ToExamineDto
    {
        /// <summary>
        /// 节点实例Id
        /// </summary>
        public virtual long NodeInstanceId { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Opinion { get; set; }
        /// <summary>
        /// 审批评论
        /// </summary>
        public string Comment { get; set; }
    }   
}
