using Finance.MakeOffers.AnalyseBoard.DTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard
{
    /// <summary>
    /// 报价分析看板api接口
    /// </summary>
    public interface IAnalyseBoardAppService
    {
        /// <summary>
        /// 查看报表分析看板
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<AnalyseBoardDto> GetStatementAnalysisBoard(long Id, long? GrossMarginId);
        /// <summary>
        /// 查看年份维度对比
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="GrossMargin"></param>
        /// <returns></returns>
        Task<List<YearDimensionalityComparisonDto>> PostYearDimensionalityComparison(YearProductBoardProcessDto yearProductBoardProcessDto);
        /// <summary>
        /// 报价分析看板 动态动态单价表单的计算
        /// </summary>
        /// <param name="productBoardDtos"></param>
        /// <returns></returns>
        Task<ProductBoardGrossMarginDto> PostCalculateFullGrossMargin(ProductBoardProcessDto productBoardDtos);
        /// <summary>
        /// 报价审核表 审批
        /// </summary>
        /// <param name="quotationListDto"></param>
        /// <returns></returns>
        Task PostAuditQuotationList(AuditQuotationListDto quotationListDto);
    }
}
