using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Finance.Dto;
using Finance.Ext;
using Finance.Infrastructure.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure
{
    /// <summary>
    /// 财务字典表
    /// </summary>
    public class FinanceDictionaryAppService : ApplicationService
    {
        private readonly IRepository<FinanceDictionary, string> _financeDictionaryRepository;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="financeDictionaryRepository"></param>
        /// <param name="financeDictionaryDetailRepository"></param>
        public FinanceDictionaryAppService(IRepository<FinanceDictionary, string> financeDictionaryRepository, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository)
        {
            _financeDictionaryRepository = financeDictionaryRepository;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
        }

        /// <summary>
        /// 添加新字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task AddFinanceDictionary(AddFinanceDictionaryInput input)
        {
            var entity = ObjectMapper.Map<FinanceDictionary>(input);
            var guid = Guid.NewGuid();
            entity.Id = $"{guid:N}";
            await _financeDictionaryRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 编辑字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task EditFinanceDictionary(EditFinanceDictionaryInput input)
        {
            var entity = await _financeDictionaryRepository.GetAsync(input.Id);

            ObjectMapper.Map(input, entity);

            await _financeDictionaryRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DeleteFinanceDictionary(string id)
        {
            await _financeDictionaryRepository.DeleteAsync(id);
        }

        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<FinanceDictionaryListDto>> GetFinanceDictionaryList(GetFinanceDictionaryListInput input)
        {
            //定义列表查询
            var filter = _financeDictionaryRepository.GetAll()
                .WhereIf(!input.Id.IsNullOrEmpty(), p => p.Id == input.Id)
                .WhereIf(!input.DisplayName.IsNullOrEmpty(), p => p.DisplayName.Contains(input.DisplayName))
                .WhereIf(!input.Remark.IsNullOrEmpty(), p => p.Remark.Contains(input.Remark));

            //定义列表查询的排序和分页
            var pagedSorted = filter.OrderByDescending(p => p.Id).PageBy(input);

            //获取总数
            var count = await filter.CountAsync();

            //获取查询结果
            var result = await pagedSorted.ToListAsync();

            return new PagedResultDto<FinanceDictionaryListDto>(count, ObjectMapper.Map<List<FinanceDictionaryListDto>>(result));
        }

        ///// <summary>
        ///// 根据字典名（即字典Id）获取字典
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public virtual async Task<FinanceDictionaryListDto> GetFinanceDictionaryByName(string name)
        //{
        //    var entity = await _financeDictionaryRepository.FirstOrDefaultAsync(p => p.Id == name);
        //    return ObjectMapper.Map<FinanceDictionaryListDto>(entity);
        //}

        /// <summary>
        /// 根据字典Id获取字典（带明细）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<FinanceDictionaryAndDetailListDto> GetFinanceDictionaryAndDetailById(string id)
        {
            var entity = await _financeDictionaryRepository.FirstOrDefaultAsync(p => p.Id == id);
            var f = ObjectMapper.Map<FinanceDictionaryAndDetailListDto>(entity);

            if (entity is not null)
            {
                //定义列表查询
                var filter = _financeDictionaryDetailRepository.GetAll()
                    .Where(p => p.FinanceDictionaryId == entity.Id);

                //定义列表查询的排序（先根据Order倒序，再根据CreationTime倒序）
                var pagedSorted = filter.OrderByDescending(p => p.Order).OrderByDescending(p => p.CreationTime);


                //获取查询结果
                var result = await pagedSorted.ToListAsync();

                var d = ObjectMapper.Map<List<FinanceDictionaryDetailListDto>>(result);
                f.FinanceDictionaryDetailList = d;
            }
            return f;

        }

        /// <summary>
        /// 根据字典Id获取字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<FinanceDictionaryListDto> GetFinanceDictionaryById(string id)
        {
            var entity = await _financeDictionaryRepository.FirstOrDefaultAsync(p => p.Id == id);
            return ObjectMapper.Map<FinanceDictionaryListDto>(entity);
        }

        /// <summary>
        /// 添加字典明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task AddFinanceDictionaryDetail(AddFinanceDictionaryDetailInput input)
        {
            var entity = ObjectMapper.Map<FinanceDictionaryDetail>(input);
            var guid = Guid.NewGuid();
            entity.Id = $"{guid:N}";
            await _financeDictionaryDetailRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 编辑字典明细（字典明细的Id，是：【字典的Id-字典明细Name】,中间用【-】连接）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task EditFinanceDictionaryDetail(EditFinanceDictionaryDetailInput input)
        {
            var entity = await _financeDictionaryDetailRepository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _financeDictionaryDetailRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除字典明细（字典明细的Id，是：【字典的Id-字典明细Name】,中间用【-】连接）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DeleteFinanceDictionaryDetail(string id)
        {
            await _financeDictionaryDetailRepository.DeleteAsync(id);
        }

        /// <summary>
        /// 获取字典明细根据它的Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<FinanceDictionaryDetailListDto> GetFinanceDictionaryDetailById(string id)
        {
            var entity = await _financeDictionaryDetailRepository.GetAsync(id);
            return ObjectMapper.Map<FinanceDictionaryDetailListDto>(entity);
        }

        /// <summary>
        /// 获取字典明细列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<FinanceDictionaryDetailListDto>> GetFinanceDictionaryDetailList(GetFinanceDictionaryDetailListInput input)
        {
            //定义列表查询
            var filter = _financeDictionaryDetailRepository.GetAll()
                .Where(p => p.FinanceDictionaryId == input.FinanceDictionaryId)
                .WhereIf(!input.DisplayName.IsNullOrEmpty(), p => p.DisplayName.Contains(input.DisplayName))
                .WhereIf(!input.Remark.IsNullOrEmpty(), p => p.Remark.Contains(input.Remark));

            //定义列表查询的排序和分页（先根据Order倒序，再根据CreationTime倒序）
            var pagedSorted = filter.OrderByDescending(p => p.Order).OrderByDescending(p => p.CreationTime).PageBy(input);

            //获取总数
            var count = await filter.CountAsync();

            //获取查询结果
            var result = await pagedSorted.ToListAsync();

            return new PagedResultDto<FinanceDictionaryDetailListDto>(count, ObjectMapper.Map<List<FinanceDictionaryDetailListDto>>(result));

        }
    }
}
