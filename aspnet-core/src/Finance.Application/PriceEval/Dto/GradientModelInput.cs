using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class GradientModelInput
    {
        /// <summary>
        /// 序号
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// 客户零件号
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// 子项目代码
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 产品大类
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public virtual List<GradientModelYearInput> GradientModelYear { get; set; }

    }

    public class GradientModelYearInput 
    {
        /// <summary>
        /// 年份
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType YearType { get; set; }

        /// <summary>
        /// 梯度走量
        /// </summary>
        public virtual decimal Count { get; set; }
    }
}
