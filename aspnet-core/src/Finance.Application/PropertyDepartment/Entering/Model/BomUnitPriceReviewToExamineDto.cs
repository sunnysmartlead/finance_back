using Finance.Dto;
using Finance.ProductDevelopment.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 电子/结构单价表id
        /// </summary>
        public List<long> UnitPriceId { get; set; }
        /// <summary>
        /// 确认人id
        /// </summary>
        public List<long> PeopleId { get; set; }
        /// <summary>
        /// 审核界面类型
        /// </summary>
        public BOMCHECKTYPE BomCheckType { get; set; }
    }
   
}
