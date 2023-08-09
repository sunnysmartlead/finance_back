using Finance.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.PropertyDepartment.Entering.Dto
{
    /// <summary>
    /// 提交电子物料 交互类
    /// </summary>
    public class SubmitElectronicDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        [FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)]
        public long AuditFlowId { get; set; }
      
        /// <summary>
        /// 资源部 填写 电子BOM表单实体类
        /// </summary>
        public List<ElectronicDto> ElectronicDtoList { get; set; }
        /// <summary>
        /// 确认还是提交  确认 0/false  提交 1/true
        /// </summary>
        public bool IsSubmit { get; set; }
    }
}
