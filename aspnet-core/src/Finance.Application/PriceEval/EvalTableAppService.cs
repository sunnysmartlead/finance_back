using Finance.PriceEval.Dto;
using Finance.PriceEval.Dto.EvalTable;
using Finance.ProductDevelopment;
using Finance.ProductDevelopment.Dto;
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
        private readonly ProductDevelopmentInputAppService _productDevelopmentInputAppService;
        private readonly PriceEvaluationAppService _priceEvaluationAppService;

        public EvalTableAppService(ProductDevelopmentInputAppService productDevelopmentInputAppService, PriceEvaluationAppService priceEvaluationAppService)
        {
            _productDevelopmentInputAppService = productDevelopmentInputAppService;
            _priceEvaluationAppService = priceEvaluationAppService;
        }

        /// <summary>
        /// 获取核价需求录入和产品开发部数据（GetPriceEvaluationStartData和PostProductDevelopmentInput接口的合并接口）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<PePdDto> GetPePd(PostProductDevelopmentInputDto input)
        {
            var priceEvaluationStartData = await _priceEvaluationAppService.GetPriceEvaluationStartData(input.AuditFlowId);
            var postProductDevelopment = await _productDevelopmentInputAppService.PostProductDevelopmentInput(input);
            return new PePdDto
            {
                PriceEvaluationStartInputResult = priceEvaluationStartData,
                PostProductDevelopmentInputDto = postProductDevelopment,
            };
        }


        /// <summary>
        /// 获取项目核价表（二开）（此接口仅供测试使用，不返回任何数据）
        /// </summary>
        /// <param name="input">获取项目核价表的接口参数输入</param>
        /// <returns>项目核价表</returns>
        public async virtual Task<ExcelPriceEvaluationTableDto> GetPriceEvaluationTable(GetPriceEvaluationTableInput input)
        {
            return new ExcelPriceEvaluationTableDto();
        }
    }
}
