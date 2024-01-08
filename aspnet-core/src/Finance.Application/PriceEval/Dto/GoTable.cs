using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }


        public virtual decimal Value { get; set; }

    }

    public class FileNameMemoryStream : MemoryStream 
    {
        public virtual string FileName { get; set; }
    }
}
