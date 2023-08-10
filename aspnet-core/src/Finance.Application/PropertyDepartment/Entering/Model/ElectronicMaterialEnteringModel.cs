using Finance.PropertyDepartment.Entering.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Entering.Model
{
    /// <summary>
    /// 电子料单价录入 模型 
    /// </summary>
    public class ElectronicMaterialEnteringModel
    {
        /// <summary>
        /// 主流程的id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 零件的ID
        /// </summary>
        public long ProductId { get; set; }      
        /// <summary>
        /// 电子BOM单价表单
        /// </summary>
        public List<ElectronicDto> ElectronicBomList { get; set; }
    }
   
}
