using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.Entering.Model
{
    /// <summary>
    /// 梯度模型
    /// </summary>
    public class GradientValueModel
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }
        /// <summary>
        /// 梯度
        /// </summary>       
        public virtual decimal Kv { get; set; }
    }
}
