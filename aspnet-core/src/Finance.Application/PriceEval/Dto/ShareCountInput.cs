using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    public class ShareCountInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }


        /// <summary>
        /// 分摊数量
        /// </summary>
        public virtual decimal Count { get; set; }

        /// <summary>
        /// 分摊年数（年的数量）
        /// </summary>
       public virtual decimal YearCount { get; set; }
    }
}
