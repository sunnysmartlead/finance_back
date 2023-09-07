using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers
{
    /// <summary>
    ///报价 项目看板实体类 实体类
    /// </summary>
    public class ProjectBoardOffers : FullAuditedEntity<long>
    {
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 梯度
        /// </summary>
        public string gradient { get; set; }
        /// <summary>
        /// 目标价(内部) 存json 实体类GrossMarginModel
        /// </summary>
        public string InteriorTarget { get; set; }
        /// <summary>
        /// 目标价(客户)存json 实体类GrossMarginModel
        /// </summary>
        public string ClientTarget { get; set; }
        /// <summary>
        /// 本次报价存  json 实体类GrossMarginModel 
        /// </summary>
        public string Offer { get; set; }      
    }
}
