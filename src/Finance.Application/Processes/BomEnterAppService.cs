using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Finance.Processes
{
    /// <summary>
    /// 管理
    /// </summary>
    public class BomEnterAppService : ApplicationService
    {
        private readonly IRepository<BomEnter, long> _bomEnterRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="bomEnterRepository"></param>
        public BomEnterAppService(
            IRepository<BomEnter, long> bomEnterRepository)
        {
            _bomEnterRepository = bomEnterRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<BomEnterDto> GetByIdAsync(long id)
        {
            BomEnter entity = await _bomEnterRepository.GetAsync(id);

            return ObjectMapper.Map<BomEnter, BomEnterDto>(entity,new BomEnterDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<BomEnterDto>> GetListAsync(GetBomEntersInput input)
        {
            // 设置查询条件
            var query = this._bomEnterRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<BomEnter>, List<BomEnterDto>>(list, new List<BomEnterDto>());
            // 数据返回
            return new PagedResultDto<BomEnterDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<BomEnterDto> GetEditorByIdAsync(long id)
        {
            BomEnter entity = await _bomEnterRepository.GetAsync(id);

            return ObjectMapper.Map<BomEnter, BomEnterDto>(entity,new BomEnterDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<BomEnterDto> CreateAsync(BomEnterDto input)
        {
            var entity = ObjectMapper.Map<BomEnterDto, BomEnter>(input,new BomEnter());
            entity = await _bomEnterRepository.InsertAsync(entity);
            return ObjectMapper.Map<BomEnter, BomEnterDto>(entity,new BomEnterDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<BomEnterDto> UpdateAsync(BomEnterDto input)
        {
            BomEnter entity = await _bomEnterRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _bomEnterRepository.UpdateAsync(entity);
            return ObjectMapper.Map<BomEnter, BomEnterDto>(entity,new BomEnterDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _bomEnterRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
