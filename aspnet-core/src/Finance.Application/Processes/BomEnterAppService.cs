using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Finance.Authorization.Users;
using Finance.BaseLibrary;
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
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<BomEnterTotal, long> _bomEnterTotalRepository;
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="bomEnterRepository"></param>
        public BomEnterAppService(
            IRepository<BomEnterTotal, long> foundationFoundationWorkingHourItemRepository,
            IRepository<User, long> userRepository,
            IRepository<BomEnter, long> bomEnterRepository)
        {
            _bomEnterRepository = bomEnterRepository;
            _bomEnterTotalRepository = foundationFoundationWorkingHourItemRepository;
            _userRepository = userRepository;
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
        /// 列表-无分页功能
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>结果</returns>
        public virtual async Task<List<BomEnterDto>> GetListAllAsync(GetBomEntersInput input)
        {

            try
            {
            var query = (from a in _bomEnterRepository.GetAllList(p =>
         p.IsDeleted == false && p.SolutionId == input.SolutionId && p.AuditFlowId == input.AuditFlowId
         ).Select(c => c.Classification).Distinct()
                             select a).ToList();
                var dtos = ObjectMapper.Map<List<String>, List<String>>(query, new List<String>());
                List<BomEnterDto> logisticscostResponseList = new List<BomEnterDto>();
                foreach (var item in dtos)
                {
                    BomEnterDto logisticscostResponse = new BomEnterDto();

                    var queryItem = this._bomEnterRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId && t.SolutionId == input.SolutionId && t.Classification == item).ToList();
                    List<BomEnterDto> bomEnterDto =   new List<BomEnterDto>();
                    foreach (var dtosItem in queryItem)
                    {
                        BomEnterDto bomEnterDto1 = new BomEnterDto();
                        bomEnterDto1.Year = dtosItem.Year;
                        bomEnterDto1.TotalCost = dtosItem.TotalCost;
                        bomEnterDto1.Remark = dtosItem.Remark;
                        bomEnterDto1.IndirectSummary = dtosItem.IndirectSummary;
                        bomEnterDto1.IndirectManufacturingCosts = dtosItem.IndirectManufacturingCosts;
                        bomEnterDto1.IndirectLaborPrice = dtosItem.IndirectLaborPrice;
                        bomEnterDto1.IndirectDepreciation = dtosItem.IndirectDepreciation;
                        bomEnterDto1.DirectSummary = dtosItem.DirectSummary;
                        bomEnterDto1.DirectManufacturingCosts = dtosItem.DirectManufacturingCosts;
                        bomEnterDto1.DirectLineChangeCost = dtosItem.DirectLineChangeCost;
                        bomEnterDto1.DirectLaborPrice = dtosItem.DirectLaborPrice;
                        bomEnterDto1.DirectDepreciation = dtosItem.DirectDepreciation;
                        bomEnterDto.Add(bomEnterDto1);
                    }

                 

                    var queryItemTotal = this._bomEnterTotalRepository.GetAll().Where(t => t.IsDeleted == false && t.AuditFlowId == input.AuditFlowId /*&& t.ProductId == input.SolutionId*/ && t.Classification == item).ToList();
                    List<BomEnterTotalDto> ListbomEnterDto = new List<BomEnterTotalDto>();
                    foreach (var dtosItemTotal in queryItemTotal)
                    {
                        BomEnterTotalDto bomEnterTotal = new BomEnterTotalDto();
                        bomEnterTotal.Year = dtosItemTotal.Year;
                        bomEnterTotal.Remark= dtosItemTotal.Remark;
                        bomEnterTotal.TotalCost = dtosItemTotal.TotalCost;
                        ListbomEnterDto.Add(bomEnterTotal);
                    }
                    logisticscostResponse.ListBomEnter = bomEnterDto;
                    logisticscostResponse.ListBomEnterTotal = ListbomEnterDto;
                    logisticscostResponse.Classification = item;
                    logisticscostResponseList.Add(logisticscostResponse);
                }

                // 数据返回
                return logisticscostResponseList;
            }
            catch (Exception)
            {

                throw;
            }
         
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
        public virtual async Task CreateAsync(BomEnterDto input)
        {
            await _bomEnterRepository.DeleteAsync(s => s.SolutionId == input.SolutionId && s.AuditFlowId == input.AuditFlowId);
            //await _bomEnterTotalRepository.DeleteAsync(s => s.ProductId == input.SolutionId && s.AuditFlowId == input.AuditFlowId);
            List<BomEnterDto> LogisticscostList = input.ListBomEnter;
        
                foreach (var item1 in input.ListBomEnter)
                {
                    BomEnter bomEnter = new BomEnter();
                    bomEnter.SolutionId = input.SolutionId;
                    bomEnter.AuditFlowId = input.AuditFlowId;
                    bomEnter.Classification = input.Classification;
                    bomEnter.CreationTime = DateTime.Now;
                    bomEnter.DirectDepreciation = item1.DirectDepreciation;
                    bomEnter.DirectLaborPrice = item1.DirectLaborPrice;
                    bomEnter.DirectLineChangeCost = item1.DirectLineChangeCost;
                    bomEnter.DirectManufacturingCosts = item1.DirectManufacturingCosts;
                    bomEnter.DirectSummary = item1.DirectSummary;
                    bomEnter.IndirectDepreciation = item1.IndirectDepreciation;
                    bomEnter.IndirectLaborPrice = item1.IndirectLaborPrice;
                    bomEnter.IndirectManufacturingCosts = item1.IndirectManufacturingCosts;
                    bomEnter.IndirectSummary = item1.IndirectSummary;
                    bomEnter.TotalCost = item1.TotalCost;
                    bomEnter.Year = item1.Year;
                    bomEnter.Remark = item1.Remark;
                    if (AbpSession.UserId != null)
                    {
                        bomEnter.CreatorUserId = AbpSession.UserId.Value;
                        bomEnter.LastModificationTime = DateTime.Now;
                        bomEnter.LastModifierUserId = AbpSession.UserId.Value;
                    }
                    bomEnter.LastModificationTime = DateTime.Now;
                     await _bomEnterRepository.InsertAsync(bomEnter);
                }
                foreach (var item1 in input.ListBomEnterTotal)
                {
                    BomEnterTotal bomEnterTotal = new BomEnterTotal();
                    //bomEnterTotal.ProductId = input.SolutionId;
                    bomEnterTotal.AuditFlowId = input.AuditFlowId;
                    bomEnterTotal.Classification = input.Classification;
                    bomEnterTotal.CreationTime = DateTime.Now;
                    bomEnterTotal.TotalCost = item1.TotalCost;
                    bomEnterTotal.Remark = item1.Remark;
                    bomEnterTotal.Year = item1.Year;
                    bomEnterTotal.Remark = item1.Remark;
                    if (AbpSession.UserId != null)
                    {
                        bomEnterTotal.CreatorUserId = AbpSession.UserId.Value;
                        bomEnterTotal.LastModificationTime = DateTime.Now;
                        bomEnterTotal.LastModifierUserId = AbpSession.UserId.Value;
                    }
                    bomEnterTotal.LastModificationTime = DateTime.Now;
                    await _bomEnterTotalRepository.InsertAsync(bomEnterTotal);
                }
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
