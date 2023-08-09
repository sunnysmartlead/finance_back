using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 核价看板产品选择下拉框返回值（返回的Id，为ProductId，即模组/产品/零件的Id）
    /// </summary>
    public class ModelCountSelectListDto : EntityDto<long>
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public virtual string ProductName { get; set; }
    }


    /// <summary>
    /// 获取产品投入量Dto
    /// </summary>
    public class ModelCountInputCountEditDto : EntityDto<long>
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public virtual string ProductName { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        public virtual int? InputCount { get; set; }

        /// <summary>
        /// 年份（生成核价表的年份）
        /// </summary>
        public virtual int? Year { get; set; }
    }
}
