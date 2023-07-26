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
    public class FoundationEmcAppService : ApplicationService
    {
        private readonly IRepository<FoundationEmc, long> _foundationEmcRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly int logType = 2;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationreliableRepository"></param>
        public FoundationEmcAppService(
            IRepository<FoundationEmc, long> foundationreliableRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationEmcRepository = foundationreliableRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
        }


        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationEmcDto> GetAsyncById(long id)
        {
            FoundationEmc entity = await _foundationEmcRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity,new FoundationEmcDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationEmcDto>> GetListAsync(GetFoundationEmcsInput input)
        {
            // 设置查询条件
            var query = this._foundationEmcRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationEmc>, List<FoundationEmcDto>>(list, new List<FoundationEmcDto>());
            // 数据返回
            return new PagedResultDto<FoundationEmcDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationEmcDto>> GetListAllAsync(GetFoundationEmcsInput input)
        {
            // 设置查询条件
            var query = this._foundationEmcRepository.GetAll().Where(t => t.IsDeleted == true);
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
            var dtos = ObjectMapper.Map<List<FoundationEmc>, List<FoundationEmcDto>>(list, new List<FoundationEmcDto>());
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
        public virtual async Task<FoundationEmcDto> GetEditorAsyncById(long id)
        {
            FoundationEmc entity = await _foundationEmcRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity,new FoundationEmcDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationEmcDto> CreateAsync(FoundationEmcDto input)
        {
        
            var entity = ObjectMapper.Map<FoundationEmcDto, FoundationEmc>(input, new FoundationEmc());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId  = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await _foundationEmcRepository.InsertAsync(entity);
            await this.CreateLog("add");
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity, new FoundationEmcDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationEmcDto> UpdateAsync(FoundationEmcDto input)
        {
            FoundationEmc entity = await _foundationEmcRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationEmcRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationEmc, FoundationEmcDto>(entity,new FoundationEmcDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationEmcRepository.DeleteAsync(s => s.Id == id);
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
