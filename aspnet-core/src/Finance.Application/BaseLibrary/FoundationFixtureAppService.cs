using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Spire.Pdf.General.Paper.Uof;
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
    public class FoundationFixtureAppService : ApplicationService
    {
        private readonly IRepository<FoundationFixture, long> _foundationFixtureRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;
        private readonly IRepository<FoundationFixtureItem, long> _foundationFoundationFixtureItemRepository;

        /// <summary>
        /// 日志类型
        /// </summary>
        private readonly int logType = 6;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="foundationFixtureRepository"></param>
        public FoundationFixtureAppService(
            IRepository<FoundationFixture, long> foundationFixtureRepository,
            IRepository<FoundationFixtureItem, long> foundationFoundationFixtureItemRepository,
            IRepository<User, long> userRepository,
            IRepository<FoundationLogs, long> foundationLogsRepository)
        {
            _foundationFixtureRepository = foundationFixtureRepository;
            _userRepository = userRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _foundationFoundationFixtureItemRepository = foundationFoundationFixtureItemRepository;
        }


        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> GetAsyncById(long id)
        {
            FoundationFixture entity = await _foundationFixtureRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<PagedResultDto<FoundationFixtureDto>> GetListAsync(GetFoundationFixturesInput input)
        {
            // 设置查询条件
            var query = this._foundationFixtureRepository.GetAll().Where(t => t.IsDeleted == false);
            // 获取总数
            var totalCount = query.Count();
            // 查询数据
            var list = query.Skip(input.PageIndex * input.MaxResultCount).Take(input.MaxResultCount).ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationFixture>, List<FoundationFixtureDto>>(list, new List<FoundationFixtureDto>());
            // 数据返回
            return new PagedResultDto<FoundationFixtureDto>(totalCount, dtos);
        }

        /// <summary>
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<FoundationFixtureDto>> GetListAllAsync(GetFoundationFixturesInput input)
        {
            // 设置查询条件
            var query = this._foundationFixtureRepository.GetAll().Where(t => t.IsDeleted == false);


            // 查询数据
            var list = query.ToList();
            //数据转换
            var dtos = ObjectMapper.Map<List<FoundationFixture>, List<FoundationFixtureDto>>(list, new List<FoundationFixtureDto>());
            foreach (var item in dtos)
            {
                var user = this._userRepository.GetAll().Where(u => u.Id == item.LastModifierUserId).ToList().FirstOrDefault();
                var FoundationDFixtureItemlist = this._foundationFoundationFixtureItemRepository.GetAll().Where(f => f.FoundationFixtureId == item.Id).ToList();

                //数据转换
                var dtosItem = ObjectMapper.Map<List<FoundationFixtureItem>, List<FoundationFixtureItemDto>>(FoundationDFixtureItemlist, new List<FoundationFixtureItemDto>());
                item.FixtureList = dtosItem;
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
        public virtual async Task<FoundationFixtureDto> GetEditorAsyncById(long id)
        {
            FoundationFixture entity = await _foundationFixtureRepository.GetAsync(id);
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }
    
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> CreateAsync(FoundationFixtureDto input)
        {
         

            var entity = ObjectMapper.Map<FoundationFixtureDto, FoundationFixture>(input, new FoundationFixture());
            entity.CreationTime = DateTime.Now;
            if (AbpSession.UserId != null)
            {
                entity.CreatorUserId = AbpSession.UserId.Value;
                entity.LastModificationTime = DateTime.Now;
                entity.LastModifierUserId = AbpSession.UserId.Value;
            }
            entity.LastModificationTime = DateTime.Now;
            entity = await this._foundationFixtureRepository.InsertAsync(entity);
            var foundationDevice = _foundationFixtureRepository.InsertAndGetId(entity);
            var result = ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity, new FoundationFixtureDto());
            if (input.FixtureList != null)
            {
                await _foundationFoundationFixtureItemRepository.DeleteAsync(t => t.FoundationFixtureId == result.Id);
                foreach (var deviceItem in input.FixtureList)
                {
                    var entityItem = ObjectMapper.Map<FoundationFixtureItemDto, FoundationFixtureItem>(deviceItem, new FoundationFixtureItem());

                    FoundationFixtureItem foundationFixtureItem = new FoundationFixtureItem();
                    foundationFixtureItem.FoundationFixtureId = foundationDevice;
                    foundationFixtureItem.CreationTime = DateTime.Now;
                    foundationFixtureItem.FixtureName = entityItem.FixtureName;
                    foundationFixtureItem.FixturePrice = entityItem.FixturePrice;
                    foundationFixtureItem.FixtureBusiness = entityItem.FixtureBusiness;
                    foundationFixtureItem.FixtureState = entityItem.FixtureState;
                    foundationFixtureItem.FixtureProvider = entityItem.FixtureProvider;
                    if (AbpSession.UserId != null)
                    {
                        foundationFixtureItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationFixtureItem.LastModificationTime = DateTime.Now;
                        foundationFixtureItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationFixtureItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationFixtureItemRepository.InsertAsync(foundationFixtureItem);
                    ObjectMapper.Map<FoundationFixtureItem, FoundationFixtureItemDto>(foundationFixtureItem, new FoundationFixtureItemDto());
                }
            }
            await this.CreateLog(" 创建治具项目1条");
            return result;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<FoundationFixtureDto> UpdateAsync(FoundationFixtureDto input)
        {
            FoundationFixture entity = await _foundationFixtureRepository.GetAsync(input.Id);
            entity = ObjectMapper.Map(input, entity);
            entity = await _foundationFixtureRepository.UpdateAsync(entity);
            if (input.FixtureList != null)
            {
                await _foundationFoundationFixtureItemRepository.DeleteAsync(t => t.FoundationFixtureId == entity.Id);
                foreach (var deviceItem in input.FixtureList)
                {
                    var entityItem = ObjectMapper.Map<FoundationFixtureItemDto, FoundationFixtureItem>(deviceItem, new FoundationFixtureItem());

                    FoundationFixtureItem foundationFixtureItem = new FoundationFixtureItem();
                    foundationFixtureItem.FoundationFixtureId = entity.Id;
                    foundationFixtureItem.CreationTime = DateTime.Now;
                    foundationFixtureItem.FixtureName = entityItem.FixtureName;
                    foundationFixtureItem.FixturePrice = entityItem.FixturePrice;
                    foundationFixtureItem.FixtureBusiness = entityItem.FixtureBusiness;
                    foundationFixtureItem.FixtureState = entityItem.FixtureState;
                    foundationFixtureItem.FixtureProvider = entityItem.FixtureProvider;
                    if (AbpSession.UserId != null)
                    {
                        foundationFixtureItem.CreatorUserId = AbpSession.UserId.Value;
                        foundationFixtureItem.LastModificationTime = DateTime.Now;
                        foundationFixtureItem.LastModifierUserId = AbpSession.UserId.Value;

                    }
                    foundationFixtureItem.LastModificationTime = DateTime.Now;
                    entityItem = await _foundationFoundationFixtureItemRepository.InsertAsync(foundationFixtureItem);
                    ObjectMapper.Map<FoundationFixtureItem, FoundationFixtureItemDto>(foundationFixtureItem, new FoundationFixtureItemDto());
                }
            }
            await this.CreateLog(" 编辑治具项目1条");
            return ObjectMapper.Map<FoundationFixture, FoundationFixtureDto>(entity,new FoundationFixtureDto());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(long id)
        {
            await _foundationFixtureRepository.DeleteAsync(s => s.Id == id);
            await this.CreateLog(" 删除治具项目1条");
        }


        /// <summary>
        /// 添加日志
        /// </summary>
        private async Task<bool> CreateLog(string Remark)
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
                entity.Remark = Remark;
                entity.Type = logType;


            }
            entity = await _foundationLogsRepository.InsertAsync(entity);
            return true;
        }
    }
}
