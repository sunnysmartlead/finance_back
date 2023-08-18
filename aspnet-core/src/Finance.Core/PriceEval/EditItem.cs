
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 修改项
    /// </summary>
    [Table("Pe_EditItem")]
    public class EditItem : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public virtual long AuditFlowId { get; set; }


        /// <summary>
        /// 修改项
        /// </summary>

        public virtual string EditItemJson { get; set; }
    }
}
