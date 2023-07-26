using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 管理
    /// </summary>
    public class FoundationreliableAppService : ApplicationService
    {
        private readonly IRepository<Foundationreliable, long> _foundationreliableRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly int logType = 1;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationreliableRepository"></param>
        public FoundationreliableAppService(
            IRepository<Foundationreliable, long> foundationreliableRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationreliableRepository = foundationreliableRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationreliableDto> GetAsyncById(long id)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(id);

            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 列表-带分页
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationreliableDto>> GetListByPagingAsync(GetFoundationreliablesInput input)
        {
            // 设置查询条件
            var query = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<Foundationreliable>, List<FoundationreliableDto>>(list, new List<FoundationreliableDto>());
            // 数据返回
            return new PagedResultDto<FoundationreliableDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationreliableDto>> GetListAllAsync(GetFoundationreliablesInput input)
        {
            // 设置查询条件
            var query = this._foundationreliableRepository.GetAll().Where(t => t.IsDeleted == true);
            if (!string.IsNullOrEmpty(input.Name))
            {
                query = query.Where(t => t.Name.Contains(input.Name));
            }
            if (!string.IsNullOrEmpty(input.Classification))
            {
                query = query.Where(t => t.Classification.Contains(input.Classification));
            }
            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<Foundationreliable>, List<FoundationreliableDto>>(list, new List<FoundationreliableDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.CreatorUserId).ToList().FirstOrDefault();
                if (user != null)
                {
                    item.LastModifierUserName = user.Name;
                }
            }
            // 数据返回
            return dtos;
        }

        /// <summary>
        /// 获取修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationreliableDto> GetEditorAsyncById(long id)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(id);

            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationreliableDto> CreateAsync(FoundationreliableDto input)
        {
            var entity = ObjectMapper.Map<FoundationreliableDto, Foundationreliable>(input, new Foundationreliable());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId= AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await _foundationreliableRepository.InsertAsync(entity);
            await this.CreateLog("add");
            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationreliableDto> UpdateAsync(FoundationreliableDto input)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map<FoundationreliableDto, Foundationreliable>(input, entity);
            entity.LastModificationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity = await _foundationreliableRepository.UpdateAsync(entity);
            await this.CreateLog("");
            return ObjectMapper.Map<Foundationreliable, FoundationreliableDto>(entity, new FoundationreliableDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            Foundationreliable entity = await _foundationreliableRepository.GetAsync(id);
            entity.DeletionTime = DateTime.Now;
            entity.IsDeleted = true;
            if (AbpSession.UserId != null)
            {
                entity.DeleterUserId = AbpSession.UserId.Value;
            }
            entity = await _foundationreliableRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        private async Task<bool> CreateLog(string type)
        {
            FoundationLogs entity = new FoundationLogs()
            {
                IsDeleted = false,
                DeletionTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                Remark = "test",
                Type = logType,
                Version = "001"
            };
            if (AbpSession.UserId != null)
            {
                entity.LastModifierUserId = AbpSession.UserId.Value;
                if ("add".Equals(type))
                {
                    entity.CreatorUserId = AbpSession.UserId.Value;
                    entity.CreationTime = DateTime.Now;
                }
            }
            var maxId = this._foundationLogsRepository.GetAll().Max(t => t.Id);
            entity.Id = maxId + 1;
            entity = await _foundationLogsRepository.InsertAsync(entity);
            return true;
        }
    }
}
