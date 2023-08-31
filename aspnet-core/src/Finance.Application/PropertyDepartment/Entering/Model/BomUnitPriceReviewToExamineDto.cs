using Finance.Dto;
using Finance.Ext;
using Finance.ProductDevelopment.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.PropertyDepartment.Entering.Model
{
    /// <summary>
    /// BOM单价审核 专用交互类
    /// </summary>
    public class BomUnitPriceReviewToExamineDto : ToExamineDto
    {
        /// <summary>
        /// 审核流程Id
        /// </summary>
        [FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 电子单价表id
        /// </summary>
        public List<long> ElectronicsUnitPriceId { get; set; }
        /// <summary>
        /// 结构单价表id
        /// </summary>
        public List<long> StructureUnitPriceId { get; set; }
        /// <summary>
        /// 审核界面类型 （1：“电子Bom审核”，2：“结构Bom审核”，3：“电子Bom单价审核”，4：“结构Bom单价审核”,5:"Bom单价审核"）
        /// </summary>
        public BOMCHECKTYPE BomCheckType { get; set; }
    }
   
}
