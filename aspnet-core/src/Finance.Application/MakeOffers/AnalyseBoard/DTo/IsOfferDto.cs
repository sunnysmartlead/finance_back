using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.NrePricing.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.DTo
{
    /// <summary>
    /// 是否报价 接口 交互类
    /// </summary>
    public class IsOfferDto
    {
        /// <summary>
        /// Nre 资源部录入
        /// </summary>
        public List<InitialSalesDepartmentDto> Nre { get; set; }
        /// <summary>
        /// 单价表
        /// </summary>
        public List<UnitPriceModel> UnitPrice { get; set; }
        /// <summary>
        /// 汇总分析
        /// </summary>
        public List<PooledAnalysisModel> PooledAnalysis { get; set; }
        /// <summary>
        /// 动态单价表 交互类
        /// </summary>
        public DynamicUnitPrice ProductBoard { get; set; }
        /// <summary>
        /// 项目看板
        /// </summary>
        public List<ProjectBoardModel> ProjectBoard { get; set; }
        /// <summary>
        /// 是否报价 true/1 是  false/0 否
        /// </summary>
        public bool IsOffer { get; set; }
        /// <summary>
        /// 不报价原因
        /// </summary>
        public string NoOfferReason { get; set; }
        /// <summary>
        /// 流程号Id
        /// </summary> 
        public long AuditFlowId { get; set; }
    }
}
