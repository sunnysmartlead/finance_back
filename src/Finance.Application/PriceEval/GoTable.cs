using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 推移图
    /// </summary>
    public class GoTable
    {
        public virtual int Year { get; set; }

        public virtual decimal Value { get; set; }

    }
}
