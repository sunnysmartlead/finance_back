using Finance.PriceEval.Dto;
using Finance.PriceEval.Dto.EvalTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 二开核价服务
    /// </summary>
    public class EvalTableAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 获取项目核价表（二开）
        /// </summary>
        /// <param name="input">获取项目核价表的接口参数输入</param>
        /// <returns>项目核价表</returns>
        public async virtual Task<ExcelPriceEvaluationTableDto> GetPriceEvaluationTable(GetPriceEvaluationTableInput input)
        {
            return new ExcelPriceEvaluationTableDto();
        }
    }
}
