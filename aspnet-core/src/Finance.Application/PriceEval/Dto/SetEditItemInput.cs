using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class SetEditItemInput
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        
        /// <summary>
        /// 修改项
        /// </summary>

        public virtual List<Material> EditItem { get; set; }
}
}
