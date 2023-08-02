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
    public class FoundationWorkingHourAppService : ApplicationService
    {
        private readonly IRepository<FoundationWorkingHour, long> _foundationWorkingHourRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationWorkingHourItem, long> _foundationFoundationWorkingHourItemRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationWorkingHourRepository"></param>
        public FoundationWorkingHourAppService(
            IRepository<FoundationWorkingHourItem, long> foundationFoundationWorkingHourItemRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository,
            IRepository<FoundationWorkingHour, long> foundationWorkingHourRepository)
        {
            _foundationWorkingHourRepository = foundationWorkingHourRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationWorkingHourItemRepository = foundationFoundationWorkingHourItemRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> GetAsyncById(long id)
        {
            FoundationWorkingHour entity = await _foundationWorkingHourRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity,new FoundationWorkingHourDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationWorkingHourDto>> GetListAsync(GetFoundationWorkingHoursInput input)
        {
            // 设置查询条件
            var query = this._foundationWorkingHourRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationWorkingHour>, List<FoundationWorkingHourDto>>(list, new List<FoundationWorkingHourDto>());
            // 数据返回
            return new PagedResultDto<FoundationWorkingHourDto>(totalCount, dtos);
        }


        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationWorkingHourDto>> GetListAllAsync(GetFoundationWorkingHoursInput input)
        {
            // 设置查询条件
            var query = this._foundationWorkingHourRepository.GetAll().Where(t => t.IsDeleted == false);
            if (!string.IsNullOrEmpty(input.ProcessName))
            {
                query = query.Where(t => t.ProcessName.Contains(input.ProcessName));
            }

            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationWorkingHour>, List<FoundationWorkingHourDto>>(list, new List<FoundationWorkingHourDto>());
            foreach (var item in dtos)
            {
                var FoundationDeviceItemlist = this._foundationFoundationWorkingHourItemRepository.GetAll().Where(f => f.FoundationWorkingHourId == item.Id).ToList();
                var dtosItem = ObjectMapper.Map<List<FoundationWorkingHourItem>, List<FoundationWorkingHourItemDto>>(FoundationDeviceItemlist, new List<FoundationWorkingHourItemDto>());
                item.ListFoundationWorkingHour = dtosItem;
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
        public virtual async Task<FoundationWorkingHourDto> GetEditorAsyncById(long id)
        {
            FoundationWorkingHour entity = await _foundationWorkingHourRepository.GetAsync(id);

            return ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity,new FoundationWorkingHourDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> CreateAsync(FoundationWorkingHourDto input)
        {
            var entity = ObjectMapper.Map<FoundationWorkingHourDto, FoundationWorkingHour>(input,new FoundationWorkingHour());

            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await this._foundationWorkingHourRepository.InsertAsync(entity);
            var foundationDevice = _foundationWorkingHourRepository.InsertAndGetId(entity);
            var result = ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity, new FoundationWorkingHourDto());
            if (input.ListFoundationWorkingHour != null)
            {
                await _foundationFoundationWorkingHourItemRepository.DeleteAsync(t => t.FoundationWorkingHourId == result.Id);
                foreach (var deviceItem in input.ListFoundationWorkingHour)
                {
                    var entityItem = ObjectMapper.Map<FoundationWorkingHourItemDto, FoundationWorkingHourItem>(deviceItem, new FoundationWorkingHourItem());

                    FoundationWorkingHourItem foundationWorkingHourItem = new FoundationWorkingHourItem();
                    foundationWorkingHourItem.FoundationWorkingHourId = foundationDevice;
                    foundationWorkingHourItem.CreationTime = DateTime.Now;
                    foundationWorkingHourItem.Year = entityItem.Year;
                    foundationWorkingHourItem.MachineHour = entityItem.MachineHour;
                    foundationWorkingHourItem.NumberPersonnel = entityItem.NumberPersonnel;
                    foundationWorkingHourItem.LaborHour = entityItem.LaborHour;
                    if (AbpSession.UserId != null)
                    {
                        foundationWorkingHourItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                        foundationWorkingHourItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationWorkingHourItemRepository.InsertAsync(foundationWorkingHourItem);
                    ObjectMapper.Map<FoundationWorkingHourItem, FoundationFixtureItemDto>(foundationWorkingHourItem, new FoundationFixtureItemDto());
                }
            }
            return result;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationWorkingHourDto> UpdateAsync(FoundationWorkingHourDto input)
        {
        
            FoundationWorkingHour entity = await _foundationWorkingHourRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationWorkingHourRepository.UpdateAsync(entity);
            var result = ObjectMapper.Map<FoundationWorkingHour, FoundationWorkingHourDto>(entity, new FoundationWorkingHourDto());

            if (input.ListFoundationWorkingHour != null)
            {
                await _foundationFoundationWorkingHourItemRepository.DeleteAsync(t => t.FoundationWorkingHourId == result.Id);
                foreach (var deviceItem in input.ListFoundationWorkingHour)
                {
                    var entityItem = ObjectMapper.Map<FoundationWorkingHourItemDto, FoundationWorkingHourItem>(deviceItem, new FoundationWorkingHourItem());

                    FoundationWorkingHourItem foundationWorkingHourItem = new FoundationWorkingHourItem();
                    foundationWorkingHourItem.FoundationWorkingHourId = entity.Id;
                    foundationWorkingHourItem.CreationTime = DateTime.Now;
                    foundationWorkingHourItem.Year = entityItem.Year;
                    foundationWorkingHourItem.MachineHour = entityItem.MachineHour;
                    foundationWorkingHourItem.NumberPersonnel = entityItem.NumberPersonnel;
                    foundationWorkingHourItem.LaborHour = entityItem.LaborHour;
                    if (AbpSession.UserId != null)
                    {
                        foundationWorkingHourItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                        foundationWorkingHourItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationWorkingHourItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationWorkingHourItemRepository.InsertAsync(foundationWorkingHourItem);
                    ObjectMapper.Map<FoundationWorkingHourItem, FoundationFixtureItemDto>(foundationWorkingHourItem, new FoundationFixtureItemDto());
                }
            }
            return result ;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationWorkingHourRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
