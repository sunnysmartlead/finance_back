using Finance.Dto;
using Finance.MakeOffers.AnalyseBoard.Method;
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
    /// 报价分析看板返回实体类  交互类
    /// </summary>
    public class AnalyseBoardDto: ResultDto
    {
        /// <summary>
        /// Nre 营销部录入初始值 
        /// </summary>
        public List<ReturnSalesDepartmentDto> NRE { get; set; }
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
 
    }
    /// <summary>
    /// 动态单价表 模型 +全部模组全生命周期 返利后销售收入与收入  和 销售毛利
    /// </summary>
    public class ProductBoardDtoOrEvery: DynamicUnitPrice
    {
        /// <summary>
        /// 每个模组全生命周期  返利后销售收入与收入 总和
        /// </summary>
        public decimal AllRebateSalesRevenueFull { get; set; }
        /// <summary>
        /// 每个模组全生命周期  销售毛利 总和
        /// </summary>
        public decimal AllTradingProfitFull { get; set; }
        /// <summary>
        /// 动态单价表目标价内部的单价
        /// </summary>
        public List<DynamicProductBoardModel> InteriorUnitPrice { get; set; }
        /// <summary>
        /// 动态单价表目标价客户的单价
        /// </summary>
        public List<DynamicProductBoardModel> ClientUnitPrice { get; set; }
        /// <summary>
        /// 动态单价表本次报价的单价
        /// </summary>
        public List<DynamicProductBoardModel> OfferUnitPrice { get; set; }
    }
    /// <summary>
    /// 动态单价表 模型
    /// </summary>
    public class DynamicUnitPrice
    {
        private decimal alInteriorGrossMargin;
        /// <summary>
        /// 目标价(内部)整套毛利率
        /// </summary>
        public decimal AllInteriorGrossMargin
        {
            get { return alInteriorGrossMargin.GetDecimal(2); }   // get 方法
            set { alInteriorGrossMargin = value; }  // set 方法
        }
        private decimal allClientGrossMargin;
        /// <summary>
        /// 目标价(客户)整套毛利率
        /// </summary>
        public decimal AllClientGrossMargin
        {
            get { return allClientGrossMargin.GetDecimal(2); }   // get 方法
            set { allClientGrossMargin = value; }  // set 方法
        }
        private decimal allOfferGrossMargin;
        /// <summary>
        /// 本次报价整套毛利率
        /// </summary>
        public decimal AllOfferGrossMargin
        {
            get { return allOfferGrossMargin.GetDecimal(2); }   // get 方法
            set { allOfferGrossMargin = value; }  // set 方法
        }
        /// <summary>
        /// 动态单价表 模型
        /// </summary>
        public List<ProductBoardModel> ProductBoard { get; set; }
    }
}
