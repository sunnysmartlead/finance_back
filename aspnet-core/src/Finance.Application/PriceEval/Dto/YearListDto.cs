using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 核价看板产品选择下拉框下拉数据接口 返回的年份信息  Id是年份值，为【2022、2023、2024】。Name为年份显示名称，为【全生命周期、2022年、2023年、2024年】
    /// </summary>
    public class YearListDto:EntityDto<int>
    {
        /// <summary>
        /// 年份名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 年份类型
        /// </summary>
        public virtual YearType UpDown { get; set; }
    }
}
