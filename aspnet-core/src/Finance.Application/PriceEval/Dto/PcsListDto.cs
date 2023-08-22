using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 终端走量（PCS）
    /// </summary>
    public class PcsListDto //: FullAuditedEntityDto<long>
    {
        /// <summary>
        /// 车厂
        /// </summary>
        public virtual string CarFactory { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        public virtual string CarModel { get; set; }

        /// <summary>
        /// 相关年份
        /// </summary>
        public virtual IList<PcsYearListDto> PcsYear { get; set; }

    }

    /// <summary>
    /// 终端走量年份
    /// </summary>
    public class PcsYearListDto //: EntityDto<long>
    {
        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public virtual decimal Quantity { get; set; }
    }
}
