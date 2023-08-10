using Abp.Application.Services.Dto;
using Finance.FinanceMaintain;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitPriceLibraryAppService
    {
        /// <summary>
        /// 查询单价库信息
        /// </summary>
        /// <param name="input"></param>
        Task<PagedResultDto<UInitPriceFormModel>> GetGainUInitPriceForm(QueryInteractionClass input);
        /// <summary>
        /// 添加/修改单价库信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task PostUInitPriceForm(IFormFile file);
        /// <summary>
        /// 查询毛利率方案(查询依据 GrossMarginName)
        /// </summary>
        /// <returns></returns>
        Task<PagedResultDto<GrossMarginDto>> GetGrossMargin(GrossMarginInputDto input);
        /// <summary>
        /// 添加/修改毛利率方案 有则修改无则添加
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        Task PostGrossMargin(BacktrackGrossMarginDto price);
        /// <summary>
        /// 删除毛利率方案
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task DeleteGrossMargin(long Id);
        /// <summary>
        /// 添加/修改汇率
        /// </summary>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        Task PostExchangeRate(ExchangeRateDto exchangeRate);
        /// <summary>
        /// 查询汇率
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ExchangeRateDto>> GetExchangeRate(ExchangeRateInputDto input);
        /// <summary>
        /// 删除汇率
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task DeleteExchangeRate(long Id);
    }
}
