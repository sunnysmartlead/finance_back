using Finance.Ext;
using System;
using System.Collections.Generic;
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
        /// 是否同意
        /// </summary>
        public bool AgreeOrNot { get; set; }
        /// <summary>
        /// 备注(必填)
        /// </summary>
        [FriendlyRequired("备注")]
        public string Remarks { get; set; }
    }
}
