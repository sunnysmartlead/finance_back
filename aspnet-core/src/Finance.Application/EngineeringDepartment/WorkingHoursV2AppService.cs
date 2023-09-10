using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Finance.BaseLibrary;
using Finance.Dto;
using Finance.EngineeringDepartment.Dto;
using Finance.TradeCompliance;
using Finance.TradeCompliance.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment
{
    public class WorkingHoursV2AppService : ApplicationService
    {
        private readonly IRepository<FollowLineTangent, long> _followLineTangentRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogs;

        public WorkingHoursV2AppService(IRepository<FollowLineTangent, long> followLineTangentRepository, IRepository<FoundationLogs, long> foundationLogs)
        {
            _followLineTangentRepository=followLineTangentRepository;
            _foundationLogs=foundationLogs;
        }

        /// <summary>
        /// 添加国家
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task AddFollowLineTangent(WorkingHoursV2Dto input)
        {
            var entity = ObjectMapper.Map<FollowLineTangent>(input);

            await _followLineTangentRepository.InsertAsync(entity);
            await CreateLog("添加了跟线切线参数库1条", LogType.FollowLineTangent);
        }

        /// <summary>
        /// 编辑国家
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task EditFollowLineTangent(EditWorkingHoursV2Dto input)
        {
            try
            {
                var entity = await _followLineTangentRepository.GetAsync(input.Id);
                if (entity == null)
                {
                    var prop = ObjectMapper.Map<FollowLineTangent>(input);
                    await _followLineTangentRepository.InsertAsync(prop);
                }
                else
                {
                    entity.LaborHour = input.LaborHour;
                    entity.MachineHour = input.MachineHour;
                    entity.PerFollowUpQuantity = input.PerFollowUpQuantity;
                    await _followLineTangentRepository.UpdateAsync(entity);
                    await CreateLog("修改了跟线切线参数库1条", LogType.FollowLineTangent);
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
        public virtual async Task DeleteFollowLineTangent(long id)
        {
            await _followLineTangentRepository.DeleteAsync(id);
            await CreateLog("删除了跟线切线参数库1条", LogType.FollowLineTangent);
        }

        /// <summary>
        /// 获取国家库列表
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkingHoursV2Dto>> PostFollowLineTangentList(PagedInputDto input)
        {

            try
            {
                List<FollowLineTangent> list = await _followLineTangentRepository.GetAll().OrderByDescending(p => p.Year).PageBy(input).ToListAsync();

                var count = await _followLineTangentRepository.CountAsync();

                List<WorkingHoursV2Dto> result = new List<WorkingHoursV2Dto>();
                foreach (var info in list)
                {
                    WorkingHoursV2Dto dto = new WorkingHoursV2Dto();
                    dto.LaborHour = info.LaborHour;
                    dto.MachineHour = info.MachineHour;
                    dto.PerFollowUpQuantity = info.PerFollowUpQuantity;
                    result.Add(dto);
                }
                return new PagedResultDto<WorkingHoursV2Dto>(count, result);
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
