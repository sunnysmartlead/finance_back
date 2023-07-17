using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.EvalTable
{
    public class EvalTableDto
    {
        /// <summary>
        /// 核价表的年份，为0表示全生命周期
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// 核价表标题
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        public virtual int InputCount { get; set; }

        /// <summary>
        /// 需求量
        /// </summary>
        public virtual int RequiredCount { get; set; }

        /// <summary>
        /// 物料（SuperType，超级大种类，区分结构料、电子料、SMT外协等信息）
        /// </summary>
        public virtual List<Material> Material { get; set; }


    }
}
