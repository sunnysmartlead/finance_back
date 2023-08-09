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
    public class FoundationHardwareAppService : ApplicationService
    {
        private readonly IRepository<FoundationHardware, long> _foundationHardwareRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationHardwareItem, long> _foundationFoundationHardwareItemRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationHardwareRepository"></param>
        public FoundationHardwareAppService(
             IRepository<FoundationHardwareItem, long> foundationFoundationHardwareItemRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository,
            IRepository<FoundationHardware, long> foundationHardwareRepository)
        {
            _foundationHardwareRepository = foundationHardwareRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationHardwareItemRepository = foundationFoundationHardwareItemRepository;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> GetAsyncById(long id)
        {
            FoundationHardware entity = await _foundationHardwareRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationHardwareDto>> GetListAsync(GetFoundationHardwaresInput input)
        {
            // 设置查询条件
            var query = this._foundationHardwareRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationHardware>, List<FoundationHardwareDto>>(list, new List<FoundationHardwareDto>());
            // 数据返回
            return new PagedResultDto<FoundationHardwareDto>(totalCount, dtos);

        }
        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationHardwareDto>> GetListAllAsync(GetFoundationHardwaresInput input)
        {
            // 设置查询条件
            var query = this._foundationHardwareRepository.GetAll().Where(t => t.IsDeleted == false);


            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationHardware>, List<FoundationHardwareDto>>(list, new List<FoundationHardwareDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                var FoundationDeviceItemlist = this._foundationFoundationHardwareItemRepository.GetAll().Where(f => f.FoundationHardwareId == item.Id).ToList();

                //数据转换
                var dtosItem = ObjectMapper.Map<List<FoundationHardwareItem>, List<FoundationHardwareItemDto>>(FoundationDeviceItemlist, new List<FoundationHardwareItemDto>());
                item.ListHardware = dtosItem;
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
        public virtual async Task<FoundationHardwareDto> GetEditorAsyncById(long id)
        {
            FoundationHardware entity = await _foundationHardwareRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationHardwareDto> CreateAsync(FoundationHardwareDto input)
        {

            var entity = ObjectMapper.Map<FoundationHardwareDto, FoundationHardware>(input, new FoundationHardware());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await this._foundationHardwareRepository.InsertAsync(entity);
            var foundationDevice = _foundationHardwareRepository.InsertAndGetId(entity);
            var result = ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity, new FoundationHardwareDto());
            if (input.ListHardware != null)
            {
                await _foundationFoundationHardwareItemRepository.DeleteAsync(t => t.FoundationHardwareId == result.Id);
                foreach (var deviceItem in input.ListHardware)
                {
                    var entityItem = ObjectMapper.Map<FoundationHardwareItemDto, FoundationHardwareItem>(deviceItem, new FoundationHardwareItem());

                    FoundationHardwareItem foundationHardwareItem = new FoundationHardwareItem();
                    foundationHardwareItem.Id = foundationDevice;
                    foundationHardwareItem.CreationTime = DateTime.Now;
                    foundationHardwareItem.HardwareName = entityItem.HardwareName;
                    foundationHardwareItem.HardwarePrice = entityItem.HardwarePrice;
                    foundationHardwareItem.HardwareBusiness = entityItem.HardwareBusiness;
                    foundationHardwareItem.HardwareState = entityItem.HardwareState;
                    if (AbpSession.UserId != null)
                    {
                        foundationHardwareItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationHardwareItem.LastModificationTime = DateTime.Now;
                        foundationHardwareItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationHardwareItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationHardwareItemRepository.InsertAsync(foundationHardwareItem);
                    ObjectMapper.Map<FoundationHardwareItem, FoundationHardwareItemDto>(foundationHardwareItem, new FoundationHardwareItemDto());


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
        public virtual async Task<FoundationHardwareDto> UpdateAsync(FoundationHardwareDto input)
        {
            FoundationHardware entity = await _foundationHardwareRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationHardwareRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity,new FoundationHardwareDto());

            if (input.ListHardware != null)
            {
                await _foundationFoundationHardwareItemRepository.DeleteAsync(t => t.FoundationHardwareId == entity.Id);
                foreach (var deviceItem in input.ListHardware)
                {
                    var entityItem = ObjectMapper.Map<FoundationHardwareItemDto, FoundationHardwareItem>(deviceItem, new FoundationHardwareItem());

                    FoundationHardwareItem foundationHardwareItem = new FoundationHardwareItem();
                    foundationHardwareItem.Id = entity.Id;
                    foundationHardwareItem.CreationTime = DateTime.Now;
                    foundationHardwareItem.HardwareName = entityItem.HardwareName;
                    foundationHardwareItem.HardwarePrice = entityItem.HardwarePrice;
                    foundationHardwareItem.HardwareBusiness = entityItem.HardwareBusiness;
                    foundationHardwareItem.HardwareState = entityItem.HardwareState;
                    if (AbpSession.UserId != null)
                    {
                        foundationHardwareItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationHardwareItem.LastModificationTime = DateTime.Now;
                        foundationHardwareItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationHardwareItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationHardwareItemRepository.InsertAsync(foundationHardwareItem);
                    ObjectMapper.Map<FoundationHardwareItem, FoundationHardwareItemDto>(foundationHardwareItem, new FoundationHardwareItemDto());
                }
            }
            return ObjectMapper.Map<FoundationHardware, FoundationHardwareDto>(entity, new FoundationHardwareDto()); ;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationHardwareRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
