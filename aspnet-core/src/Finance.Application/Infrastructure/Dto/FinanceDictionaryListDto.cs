using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Dto
{
    /// <summary>
    /// 财务字典dto
    /// </summary>
    public class FinanceDictionaryListDto : FullAuditedEntityDto<string>
    {
        /// <summary>
        /// 字典显示名
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }

    /// <summary>
    /// 财务字典和明细
    /// </summary>
    public class FinanceDictionaryAndDetailListDto : FinanceDictionaryListDto
    {
        /// <summary>
        /// 字典明细
        /// </summary>
        public virtual List<FinanceDictionaryDetailListDto> FinanceDictionaryDetailList { get; set; }
    }
}
