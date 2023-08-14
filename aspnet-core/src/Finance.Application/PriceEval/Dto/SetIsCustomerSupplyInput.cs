using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class SetIsCustomerSupplyInput
    {
        /// <summary>
        /// 审批流程主表Id
        /// </summary>
        [Required]
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// BOM是否客供
        /// </summary>
        public virtual List<BomIsCustomerSupply> BomIsCustomerSupplyList { get; set; }
    }


    public class BomIsCustomerSupply : Entity<string> 
    {
        /// <summary>
        /// 是否客供（核价看板页面录入的是否客供）
        /// </summary>
        public virtual bool IsCustomerSupply { get; set; }
    }
}
