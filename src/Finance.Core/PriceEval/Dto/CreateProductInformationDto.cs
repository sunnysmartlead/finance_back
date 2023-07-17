using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 创建产品信息Dto
    /// </summary>
    public class CreateProductInformationDto
    {
        /// <summary>
        /// 类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        [Required]
        public virtual string TypeSelect { get; set; }

        /// <summary>
        /// 零件名称
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// 核心部件
        /// </summary>
        [Required]
        public virtual string CoreName { get; set; }

        /// <summary>
        /// 品牌/型号
        /// </summary>
        public virtual string BrandModel { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public virtual decimal? Price { get; set; }

        /// <summary>
        /// 制程
        /// </summary>
        [Required]
        public virtual string ProductionProcess { get; set; }
    }
}
