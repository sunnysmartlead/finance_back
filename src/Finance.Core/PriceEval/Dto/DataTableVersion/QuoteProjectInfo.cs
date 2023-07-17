using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.DataTableVersion
{
    /// <summary>
    /// 核报价项目信息（此处返回的Id是AuditFlowId，调用GetCostDetailVariance方法时，传入此Id）
    /// </summary>
    public class QuoteProjectInfo : EntityDto<long>
    {
        /// <summary>
        /// 核报价项目名称
        /// </summary>
        public virtual string QuoteProjectName { get; set; }

        /// <summary>
        /// 核报价流程版本
        /// </summary>
        public virtual int QuoteVersion { get; set; }
        /// <summary>
        /// 核价是否有效(0:无效,1:有效)
        /// </summary>
        public virtual bool IsValid { get; set; }

    }
}
