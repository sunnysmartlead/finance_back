using Finance.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.DTo
{
    /// <summary>
    /// 年份维度对比 动态单价表计算 交互类
    /// </summary>
    public class YearProductBoardProcessDto
    {
        /// <summary>
        /// 流程表id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossMargin { get; set; }
        /// <summary>
        /// 动态单价表计算 交互类
        /// </summary>
        public List<DynamicProductBoardModel> ProductBoards { get; set; }
    }
    /// <summary>
    /// 年份维度对比 动态单价表计算 交互类
    /// </summary>
    public class YearSomeProductBoardProcessDto
    {
        /// <summary>
        /// 流程表id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossMargin { get; set; }
        /// <summary>
        /// 动态单价表计算 交互类
        /// </summary>
        public DynamicProductBoardModel ProductBoard { get; set; }
    }
    /// <summary>
    /// 动态单价表计算 交互类
    /// </summary>
    public class ProductBoardProcessDto
    {
        /// <summary>
        /// 流程表id
        /// </summary>
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 动态单价表计算 交互类
        /// </summary>
        public List<DynamicProductBoardModel> ProductBoards { get; set; }
    }   
    /// <summary>
    /// 动态单价表计算 模型
    /// </summary>
    public class DynamicProductBoardModel    
    {       
        /// <summary>
        /// 模组数量主键
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 本次报价-单价
        /// </summary>
        public decimal UnitPrice { get; set; }
    } 
    /// <summary>
    /// 动态单价表计算 交互类
    /// </summary>
    public class ProductBoardGrossMarginDto: ResultDto
    {
        /// <summary>
        /// 本次報價 整套毛利率
        /// </summary>
        public decimal AllGrossMargin { get; set; }
        /// <summary>
        /// 动态单价表计算 模型
        /// </summary>
        public List<ProductBoardGrossMarginModel> ProductBoardGrosses { get; set; }
    }
    /// <summary>
    /// 动态单价表计算 模型
    /// </summary>
    public class ProductBoardGrossMarginModel: DynamicProductBoardModel
    {
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossMargin { get; set; }
    }
}
