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
    public class BomEnterTotalAppService : ApplicationService
    {
        private readonly IRepository<BomEnterTotal, long> _bomEnterTotalRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="bomEnterTotalRepository"></param>
        public BomEnterTotalAppService(
            IRepository<BomEnterTotal, long> bomEnterTotalRepository)
        {
            _bomEnterTotalRepository = bomEnterTotalRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<BomEnterTotalDto> GetByIdAsync(long id)
        {
            BomEnterTotal entity = await _bomEnterTotalRepository.GetAsync(id);

            return ObjectMapper.Map<BomEnterTotal, BomEnterTotalDto>(entity,new BomEnterTotalDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<BomEnterTotalDto>> GetListAsync(GetBomEnterTotalsInput input)
        {
            // 设置查询条件
            var query = this._bomEnterTotalRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<BomEnterTotal>, List<BomEnterTotalDto>>(list, new List<BomEnterTotalDto>());
            // 数据返回
            return new PagedResultDto<BomEnterTotalDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<BomEnterTotalDto> GetEditorByIdAsync(long id)
        {
            BomEnterTotal entity = await _bomEnterTotalRepository.GetAsync(id);

            return ObjectMapper.Map<BomEnterTotal, BomEnterTotalDto>(entity,new BomEnterTotalDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<BomEnterTotalDto> CreateAsync(BomEnterTotalDto input)
        {
            var entity = ObjectMapper.Map<BomEnterTotalDto, BomEnterTotal>(input,new BomEnterTotal());
            entity = await _bomEnterTotalRepository.InsertAsync(entity);
            return ObjectMapper.Map<BomEnterTotal, BomEnterTotalDto>(entity,new BomEnterTotalDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<BomEnterTotalDto> UpdateAsync(BomEnterTotalDto input)
        {
            BomEnterTotal entity = await _bomEnterTotalRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _bomEnterTotalRepository.UpdateAsync(entity);
            return ObjectMapper.Map<BomEnterTotal, BomEnterTotalDto>(entity,new BomEnterTotalDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _bomEnterTotalRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
