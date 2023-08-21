using Finance.Ext;
using Finance.NrePricing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.NrePricing.Dto
{
    /// <summary>
    /// 产品部-电子工程师 录入 交互类
    /// </summary>
    public class ProductDepartmentDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        ///  实验费 实体类
        /// </summary>
        public List<ProductDepartmentModel> ProductDepartmentModels { get; set; }
    }
    /// <summary>
    /// 产品部-电子工程师 录入 交互类(单个零件)
    /// </summary>
    public class ProductDepartmentSingleDto
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        [FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)]
        public long AuditFlowId { get; set; }

        /// <summary>
        /// 方案的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)]
        public long SolutionId { get; set; }
        /// <summary>
        ///  实验费 实体类
        /// </summary>
        public List<LaboratoryFeeModel> ProductDepartmentModels { get; set; }
        /// <summary>
        /// 是否提交 true 提价  false 保存
        /// </summary>
        public bool IsSubmit { get; set; }
    }

}
