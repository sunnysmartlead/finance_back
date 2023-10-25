using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 车型下的模组数量
    /// </summary>
    public class CreateCarModelCountDto
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
        /// 序号（正序排序，从1开始）
        /// </summary>
        [Required]
        public virtual long Order { get; set; }

        /// <summary>
        /// 客户零件号
        /// </summary>
        public virtual string PartNumber { get; set; }

        /// <summary>
        /// 子项目代码
        /// </summary>
        public virtual string Code { get; set; }


        /// <summary>
        /// 产品（产品名称从这里取）（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 产品大类（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string ProductType { get; set; }

        /// <summary>
        /// 像素
        /// </summary>
        [Required]
        public virtual string Pixel { get; set; }

        /// <summary>
        /// 我司角色
        /// </summary>
        public virtual string OurRole { get; set; }

        /// <summary>
        /// 市场份额（%）
        /// </summary>
        [Required]
        public virtual decimal MarketShare { get; set; }

        /// <summary>
        /// 模组搭载率
        /// </summary>
        [Required]
        public virtual decimal ModuleCarryingRate { get; set; }

        /// <summary>
        /// 单车产品数量
        /// </summary>
        [Required]
        public virtual int SingleCarProductsQuantity { get; set; }
 
        /// <summary>
        /// 年份
        /// </summary>
        public virtual List<CreateCarModelCountYearDto> ModelCountYearList { get; set; }
    }

    /// <summary>
    /// 车型下的模组数量年份
    /// </summary>
    public class CreateCarModelCountYearDto 
    {
        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        [Range(FinanceConsts.MinYear, FinanceConsts.MaxYear)]
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        [Required]
        public virtual YearType UpDown { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        [Range(0, long.MaxValue)]
        public virtual decimal Quantity { get; set; }
    }
}
