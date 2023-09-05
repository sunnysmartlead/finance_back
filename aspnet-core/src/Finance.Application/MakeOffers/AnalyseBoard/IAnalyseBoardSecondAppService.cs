using System.Threading.Tasks;
using Finance.MakeOffers.AnalyseBoard.DTo;

namespace Finance.MakeOffers.AnalyseBoard;
/// <summary>
/// 报价分析看板二次开发api接口
/// </summary>
public interface IAnalyseBoardSecondAppService
{

    /// <summary>
    /// 查看报表分析看板  查看报价分析看板不含样品,查看报价分析看板含样品,查看报价分析看板仅含样品
    /// </summary>
    /// <param name="analyseBoardSecondInputDto"></param>
    /// <returns></returns>
    Task<AnalyseBoardSecondDto> PostStatementAnalysisBoardSecond(AnalyseBoardSecondInputDto analyseBoardSecondInputDto);
}