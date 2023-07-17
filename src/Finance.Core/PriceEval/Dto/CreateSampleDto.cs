using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class CreateSampleDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// 样品阶段名称（从字典明细表取值，FinanceDictionaryId是【SampleName】）
        /// </summary>
        public virtual string Name { get; set; }


        /// <summary>
        /// 需求量
        /// </summary>
        public virtual decimal Pcs { get; set; }
    }
}
