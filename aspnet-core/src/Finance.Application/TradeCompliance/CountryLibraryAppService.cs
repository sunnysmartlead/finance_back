using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Finance.BaseLibrary;
using Finance.Dto;
using Finance.TradeCompliance.Dto;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepository<FoundationLogs, long> _foundationLogs;

        public CountryLibraryAppService(IRepository<CountryLibrary, long> countryLibraryRepository, IRepository<FoundationLogs, long> foundationLogs)
        {
            _countryLibraryRepository = countryLibraryRepository;
            _foundationLogs = foundationLogs;
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
            await CreateLog("添加了贸易合规国家库1条",LogType.CountryLib);
        }

        /// <summary>
        /// 编辑国家
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task EditCountryLibrary(EditCountryLibraryDto input)
        {
            try {
                var entity = await _countryLibraryRepository.GetAsync(input.Id);
                if (entity == null)
                {
                    var prop = ObjectMapper.Map<CountryLibrary>(input);
                    await _countryLibraryRepository.InsertAsync(prop);
                }
                else
                {
                    entity.NationalType = input.NationalType;
                    entity.Country = input.Country;
                    entity.Rate = input.Rate;
                    await _countryLibraryRepository.UpdateAsync(entity);
                    await CreateLog("修改了贸易合规国家库1条", LogType.CountryLib);
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }

        /// <summary>
        /// 删除国家
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DeleteCountryLibrary(long id)
        {
            await _countryLibraryRepository.DeleteAsync(id);
            await CreateLog("删除了贸易合规国家库1条", LogType.CountryLib);
        }

        /// <summary>
        /// 获取国家库列表
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultDto<CountryLibraryDto>> PostDCountryLibraryList(PagedInputDto input)
        {

            try {
                List<CountryLibrary> list = await _countryLibraryRepository.GetAll().OrderByDescending(p => p.NationalType).PageBy(input).ToListAsync();

                var count = await _countryLibraryRepository.CountAsync();

                List<CountryLibraryDto> result = new List<CountryLibraryDto>();
                foreach (var info in list)
                {
                    CountryLibraryDto dto = new CountryLibraryDto();
                    dto.NationalType = info.NationalType;
                    dto.Country = info.Country;
                    dto.Rate = info.Rate;
                    result.Add(dto);
                }
                return new PagedResultDto<CountryLibraryDto>(count, result);
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }

        }
        /// <summary>
        /// 添加日志
        /// </summary>
        private async Task<bool> CreateLog(string Remark, LogType logType)
        {
            FoundationLogs entity = new FoundationLogs()
            {
                IsDeleted = false,
                DeletionTime = DateTime.Now,
                LastModificationTime = DateTime.Now,

            };
            if (AbpSession.UserId != null)
            {
                entity.LastModifierUserId = AbpSession.UserId.Value;
                entity.CreatorUserId = AbpSession.UserId.Value;
            }
            entity.Remark = Remark;
            entity.Type = logType;
            entity = await _foundationLogs.InsertAsync(entity);
            return true;
        }

    }
}
