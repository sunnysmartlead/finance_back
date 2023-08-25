using Abp.Application.Services.Dto;
using Finance.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TradeCompliance.Dto
{
    /// <summary>
    /// 贸易合规 国家输入参数Dto
    /// </summary>
    public class CountryLibraryDto
    {
     
        /// <summary>
        /// 国家类型
        /// </summary>
        public string NationalType { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
        public string Rate { get; set; }

    }

    public class EditCountryLibraryDto : EntityDto<long>
    {

        /// <summary>
        /// 国家类型
        /// </summary>
        public string NationalType { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
        public string Rate { get; set; }

    }



    /// <summary>
    /// 获取字典列表方法的参数输入
    /// </summary>
    public class GetCountryLibraryDtoListInput : PagedInputDto
    {
        /// <summary>
        /// 国家类型
        /// </summary>
        public string NationalType { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
        public string Rate { get; set; }
    }
}
