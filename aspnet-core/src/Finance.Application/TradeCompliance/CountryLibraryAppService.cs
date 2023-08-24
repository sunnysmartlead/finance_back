using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.TradeCompliance.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TradeCompliance
{
    /// <summary>
    /// 贸易合规 国家库增删改查
    /// </summary>
    public class CountryLibraryAppService : ApplicationService
    {
        private readonly IRepository<CountryLibrary, long> _countryLibraryRepository;

        public CountryLibraryAppService(IRepository<CountryLibrary, long> countryLibraryRepository)
        {
            _countryLibraryRepository=countryLibraryRepository;
        }

        /// <summary>
        /// 添加国家
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task AddCountryLibrary(CountryLibraryDto input)
        {
            var entity = ObjectMapper.Map<CountryLibrary>(input);
    
            await _countryLibraryRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 编辑国家
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task EditCountryLibrary(EditCountryLibraryDto input)
        {
            var entity = await _countryLibraryRepository.GetAsync(input.Id);

            ObjectMapper.Map(input, entity);

            await _countryLibraryRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除国家
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DeleteCountryLibrary(long id)
        {
            await _countryLibraryRepository.DeleteAsync(id);
        }

        /// <summary>
        /// 获取国家列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<GetCountryLibraryDtoListInput>> GetFinanceDictionaryList(GetCountryLibraryDtoListInput input)
        {
            ////定义列表查询
            //var filter = _countryLibraryRepository.GetAll()
            //    .WhereIf(!input.NationalType.IsNullOrEmpty(), p => p.NationalType.Contains(input.NationalType))
            //    .WhereIf(!input.Country.IsNullOrEmpty(), p => p.Country.Contains(input.Country));

            ////定义列表查询的排序和分页
            //var pagedSorted = filter.OrderByDescending(p => p.Id).PageBy(input);

            ////获取总数
            //var count = await filter.CountAsync();

            ////获取查询结果
            //var result = await pagedSorted.ToListAsync();

            //return new PagedResultDto<FinanceDictionaryListDto>(count, ObjectMapper.Map<List<FinanceDictionaryListDto>>(result));
            return null;
        }

    }
}
