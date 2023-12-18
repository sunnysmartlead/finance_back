using Finance.ProductDevelopment.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class PePdDto
    {
        /// <summary>
        /// 核价需求录入数据
        /// </summary>
        public PriceEvaluationStartInputResult PriceEvaluationStartInputResult { get; set; }

        /// <summary>
        /// 产品开发部录入信息
        /// </summary>
        public PostProductDevelopmentInputDto PostProductDevelopmentInputDto { get; set; }
    }
}
