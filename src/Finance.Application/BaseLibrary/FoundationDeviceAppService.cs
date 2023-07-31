using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Microsoft.AspNetCore.Mvc;
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
    public class FoundationDeviceAppService : ApplicationService
    {
        private readonly IRepository<FoundationDevice, long> _foundationDeviceRepository;
        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly int logType = 4;


        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationDeviceItem, long> _foundationFoundationDeviceItemRepository;

        public FoundationDeviceAppService(
            IRepository<FoundationDevice, long> foundationDeviceRepository,
            IRepository<FoundationDeviceItem, long> foundationFoundationDeviceItemRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationDeviceRepository = foundationDeviceRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationDeviceItemRepository = foundationFoundationDeviceItemRepository;
        }



        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationDeviceDto> GetAsyncById(long id)
        {
            FoundationDevice entity = await _foundationDeviceRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity, new FoundationDeviceDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationDeviceDto>> GetListAsync(GetFoundationDevicesInput input)
        {
            // 设置查询条件
            var query = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationDevice>, List<FoundationDeviceDto>>(list, new List<FoundationDeviceDto>());
            // 数据返回
            return new PagedResultDto<FoundationDeviceDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationDeviceDto>> GetListAllAsync(GetFoundationDevicesInput input)
        {
            // 设置查询条件
            var query = this._foundationDeviceRepository.GetAll().Where(t => t.IsDeleted == false);
           

            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationDevice>, List<FoundationDeviceDto>>(list, new List<FoundationDeviceDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                var FoundationDeviceItemlist = this._foundationFoundationDeviceItemRepository.GetAll().Where(f => f.ProcessHoursEnterId == item.Id).ToList();
              
                //数据转换
                var dtosItem = ObjectMapper.Map<List<FoundationDeviceItem>, List<FoundationDeviceItemDto>>(FoundationDeviceItemlist, new List<FoundationDeviceItemDto>());
                item.DeviceList = dtosItem;
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
        public virtual async Task<FoundationDeviceDto> GetEditorAsyncById(long id)
        {
            FoundationDevice entity = await _foundationDeviceRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity,new FoundationDeviceDto());
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<FoundationDeviceDto> CreateAsync(FoundationDeviceDto input)
        {
            var entity = ObjectMapper.Map<FoundationDeviceDto, FoundationDevice>(input,new FoundationDevice());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await this._foundationDeviceRepository.InsertAsync(entity);
            var foundationDevice = _foundationDeviceRepository.InsertAndGetId(entity);
            var  result = ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity, new FoundationDeviceDto());
            if (input.DeviceList != null)
            {
                await _foundationFoundationDeviceItemRepository.DeleteAsync(t => t.ProcessHoursEnterId == result.Id);
                foreach (var deviceItem in input.DeviceList)
                {
                    var entityItem = ObjectMapper.Map<FoundationDeviceItemDto, FoundationDeviceItem>(deviceItem, new FoundationDeviceItem());

                    FoundationDeviceItem foundationDeviceItem =   new FoundationDeviceItem();
                    foundationDeviceItem.ProcessHoursEnterId = foundationDevice;
                    foundationDeviceItem.CreationTime = DateTime.Now;
                    foundationDeviceItem.DeviceNumber = entityItem.DeviceNumber;
                    foundationDeviceItem.DeviceName = entityItem.DeviceName;
                    foundationDeviceItem.DeviceStatus = entityItem.DeviceStatus;
                    foundationDeviceItem.DevicePrice = entityItem.DevicePrice;
                    foundationDeviceItem.DeviceProvider = entityItem.DeviceProvider;
                    if (AbpSession.UserId != null)
                    {
                        foundationDeviceItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationDeviceItem.LastModificationTime = DateTime.Now;
                        foundationDeviceItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationDeviceItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationDeviceItemRepository.InsertAsync(foundationDeviceItem);
                    ObjectMapper.Map<FoundationDeviceItem, FoundationDeviceItemDto>(foundationDeviceItem, new FoundationDeviceItemDto());
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
        public virtual async Task<FoundationDeviceDto> UpdateAsync(FoundationDeviceDto input)
        {
            FoundationDevice entity = await _foundationDeviceRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationDeviceRepository.UpdateAsync(entity);
            if (input.DeviceList != null)
            {
                await _foundationFoundationDeviceItemRepository.DeleteAsync(t => t.ProcessHoursEnterId == entity.Id);
                     foreach (var deviceItem in input.DeviceList)
                {
                    var entityItem = ObjectMapper.Map<FoundationDeviceItemDto, FoundationDeviceItem>(deviceItem, new FoundationDeviceItem());

                    FoundationDeviceItem foundationDeviceItem =   new FoundationDeviceItem();
                    foundationDeviceItem.ProcessHoursEnterId = entity.Id;
                    foundationDeviceItem.CreationTime = DateTime.Now;
                    foundationDeviceItem.DeviceNumber = entityItem.DeviceNumber;
                    foundationDeviceItem.DeviceName = entityItem.DeviceName;
                    foundationDeviceItem.DeviceStatus = entityItem.DeviceStatus;
                    foundationDeviceItem.DevicePrice = entityItem.DevicePrice;
                    foundationDeviceItem.DeviceProvider = entityItem.DeviceProvider;
                    if (AbpSession.UserId != null)
                    {
                        foundationDeviceItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationDeviceItem.LastModificationTime = DateTime.Now;
                        foundationDeviceItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationDeviceItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationDeviceItemRepository.InsertAsync(foundationDeviceItem);
                    ObjectMapper.Map<FoundationDeviceItem, FoundationDeviceItemDto>(foundationDeviceItem, new FoundationDeviceItemDto());
                }
            }
            return ObjectMapper.Map<FoundationDevice, FoundationDeviceDto>(entity,new FoundationDeviceDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationDeviceRepository.DeleteAsync(s => s.Id == id);
        }
    }
}
