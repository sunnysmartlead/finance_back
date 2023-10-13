using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 创建 模组数量 的参数
    /// </summary>
    public class CreateModelCountDto
    {
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
        /// 产品大类名（ProductType这个键对应的值）——应杨工要求：公式工序界面需要用到而添加
        /// </summary>
        public virtual string ProductTypeName { get; set; }

        /// <summary>
        /// 像素
        /// </summary>
        [Required]
        public virtual string Pixel { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Required]
        public virtual List<CreateModelCountYearDto> ModelCountYearList { get; set; }

        /// <summary>
        /// 合计数量（模组数量合计专用，单零件全产品全年份合计数量）
        /// </summary>
        [Required]
        public virtual decimal SumQuantity { get; set; }
    }

    /// <summary>
    /// 创建 终端走量年份 的参数
    /// </summary>
    public class CreateModelCountYearDto
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
