using Finance.Dto;
using Finance.Ext;
using Finance.PropertyDepartment.Entering.Dto;
using Finance.PropertyDepartment.Entering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.Entering.Model
{
    /// <summary>
    /// 结构件单价录入 模型
    /// </summary>
    public class StructuralMemberEnteringModel: ToExamineDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        [FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification,true)]
        public long AuditFlowId { get; set; }
      
        /// <summary>
        /// 确认还是提交  确认 0/false  提交 1/true
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        ///  结构料单价录入界面
        /// </summary>
        public List<StructuralMaterialModel> StructuralMaterialEntering { get; set; }
    }
    /// <summary>
    /// 结构件单价复制信息录入 模型
    /// </summary>
    public class StructuralMemberEnteringModelCopy : ToExamineDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        [FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification, true)]
        public long AuditFlowId { get; set; }

        /// <summary>
        /// 确认还是提交  确认 0/false  提交 1/true
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        ///  结构料单价录入界面
        /// </summary>
        public List<ConstructionModelCopy> StructuralMaterialEntering { get; set; }
    }
}
