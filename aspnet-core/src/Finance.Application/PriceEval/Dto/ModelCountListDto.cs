using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 模组数量Dto
    /// </summary>
    public class ModelCountListDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// 审批流程主表ID
        /// </summary>
        public virtual long AuditFlowId { get; set; }

        /// <summary>
        /// 核价表主键
        /// </summary>
        public virtual long PriceEvaluationId { get; set; }

        /// <summary>
        /// 序号（正序排序，从1开始）
        /// </summary>
        public virtual long Order { get; set; }

        /// <summary>
        /// 客户零件号
        /// </summary>
        public virtual string PartNumber { get; set; }


        /// <summary>
        /// 产品（产品名称从这里取）（字典明细表主键）
        /// </summary>
        public virtual string Product { get; set; }

        /// <summary>
        /// 产品小类（字典明细表主键）
        /// </summary>
        public virtual string ProductType { get; set; }

        /// <summary>
        /// 市场份额（%）
        /// </summary>
        public virtual decimal MarketShare { get; set; }

        /// <summary>
        /// 模组搭载率
        /// </summary>
        public virtual decimal ModuleCarryingRate { get; set; }

        /// <summary>
        /// 单车产品数量
        /// </summary>
        public virtual int SingleCarProductsQuantity { get; set; }

        /// <summary>
        /// 模组总量
        /// </summary>
        public virtual int ModelTotal { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        public virtual int? InputCount { get; set; }

        /// <summary>
        /// 年份（生成核价表的年份）
        /// </summary>
        public virtual int? Year { get; set; }

        /// <summary>
        /// 核价表Json
        /// </summary>
        public virtual string TableJson { get; set; }

        /// <summary>
        /// 核价表Json（全生命周期）
        /// </summary>
        public virtual string TableAllJson { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual IList<ModelCountYearListDto> ModelCountYearListDto { get; set; }
    }
}
