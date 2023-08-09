using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.DataTableVersion
{
    /// <summary>
    /// 根据两个流程号，获取产品 列表
    /// </summary>
    public class GetProductListByAuditFlowIdsInput : PagedResultRequestDto
    {
        /// <summary>
        /// 版本1 流程 Id
        /// </summary>
        public virtual int QuoteVersion1AuditFlowId { get; set; }

        /// <summary>
        /// 版本2 流程 Id
        /// </summary>
        public virtual int QuoteVersion2AuditFlowId { get; set; }
    }
}
