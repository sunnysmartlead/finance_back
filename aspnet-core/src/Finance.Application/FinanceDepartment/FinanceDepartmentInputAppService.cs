using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.ObjectMapping;
using Finance.FinanceDepartment.Dto;
using Finance.FinanceParameter;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.FinanceDepartment
{
    /// <summary>
    /// 财务部所有接口
    /// </summary>
    public class FinanceDepartmentInputAppService : ApplicationService
    {
        private readonly IRepository<RateEntryInfo, long> _rateEntryInfoRepository;
        private readonly IRepository<QualityRatioEntryInfo, long> _qualityCostProportionEntryInfoRepository;
        private readonly IRepository<QualityRatioYearInfo, long> _qualityCostProportionYearInfoRepository;
        private readonly IRepository<ManufacturingCostInfo, long> _manufacturingCostInfoRepository;
        private readonly IObjectMapper _objectMapper;

        public FinanceDepartmentInputAppService(IRepository<RateEntryInfo, long> rateEntryInfoRepository, IRepository<QualityRatioEntryInfo, long> qualityCostProportionEntryInfoRepository, IRepository<QualityRatioYearInfo, long> qualityCostProportionYearInfoRepository, IRepository<ManufacturingCostInfo, long> manufacturingCostInfoRepository, IObjectMapper objectMapper)
        {
            _rateEntryInfoRepository=rateEntryInfoRepository;
            _qualityCostProportionEntryInfoRepository=qualityCostProportionEntryInfoRepository;
            _qualityCostProportionYearInfoRepository=qualityCostProportionYearInfoRepository;
            _manufacturingCostInfoRepository=manufacturingCostInfoRepository;
            _objectMapper=objectMapper;
        }



        /// <summary>
        /// 获取费率录入
        /// </summary>
        /// <returns></returns>
        public async Task<RateEntryDto> GetRateEntry()
        {
            List<RateEntryInfo> result = await _rateEntryInfoRepository.GetAll().ToListAsync();

            RateEntryDto dto = new RateEntryDto();
            dto.rateEntryInfos = result;

            return dto;
        }

        /// <summary>
        /// 保存费率
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task SaveRateEntryInput(RateEntryDto dto)
        {

            List<RateEntryInfo> rateEntryInfos = dto.rateEntryInfos;
            await _rateEntryInfoRepository.HardDeleteAsync(s => s.Id>0);
            rateEntryInfos.ForEach(async info =>
            {
               await _rateEntryInfoRepository.InsertOrUpdateAsync(info);
            });

        }

        /// <summary>
        /// 获取质量成本比例录入
        /// </summary>
        /// <returns></returns>
        public async Task<List<QualityCostDto>> GetQualityCost()
        {
            List<QualityCostDto> retqualityCostDto = new();

            List<QualityRatioEntryInfo> qualityCostProportionEntryInfos = await _qualityCostProportionEntryInfoRepository.GetAll().ToListAsync();
            foreach (var qualityCostProportionEntryInfo in qualityCostProportionEntryInfos)
            {
                List<QualityRatioYearInfo> qualityCostProportionYearInfos = await _qualityCostProportionYearInfoRepository.GetAll()
                                                            .Where(p => qualityCostProportionEntryInfo.Id.Equals(p.QualityCostId)).ToListAsync();
                if (qualityCostProportionYearInfos.Count > 0)
                {
                    var qualityCostProportionYearrDto = _objectMapper.Map<List<QualityCostYearDto>>(qualityCostProportionYearInfos);
                    var qualityCostDto = _objectMapper.Map<QualityCostDto>(qualityCostProportionEntryInfo);

                    qualityCostDto.QualityCostYearList = qualityCostProportionYearrDto;
                    retqualityCostDto.Add(qualityCostDto);
                }
            }
            return retqualityCostDto;
        }

        /// <summary>
        /// 保存质量成本比例录入
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        public async Task SaveQualityCost(QualityCostProportionEntryDto input)
        {
            List<QualityCostDto> qualityCostDtoList = input.QualityCostList;
            foreach (var qualityCostDto in qualityCostDtoList)
            {
                long qualityCostInfoId = 0;
                QualityRatioEntryInfo qualityCostInfo = _objectMapper.Map<QualityRatioEntryInfo>(qualityCostDto);
                var infos = await _qualityCostProportionEntryInfoRepository.GetAllListAsync(p => p.Category.Equals(qualityCostInfo.Category) && p.IsFirst == qualityCostInfo.IsFirst);
                if (infos.Count > 0)
                {
                    qualityCostInfoId = (infos.FirstOrDefault()).Id;
                }
                else
                {
                    qualityCostInfoId = await _qualityCostProportionEntryInfoRepository.InsertAndGetIdAsync(qualityCostInfo);
                }
                var qualityCostYearInfos = _objectMapper.Map<List<QualityRatioYearInfo>>(qualityCostDto.QualityCostYearList);
                foreach (var qualityCostYearInfo in qualityCostYearInfos)
                {
                    var yearinfos = await _qualityCostProportionYearInfoRepository.GetAllListAsync(p => p.QualityCostId == qualityCostInfoId && p.Year == qualityCostYearInfo.Year);
                    if (yearinfos.Count > 0)
                    {
                        yearinfos.FirstOrDefault().Rate = qualityCostYearInfo.Rate;
                        await _qualityCostProportionYearInfoRepository.UpdateAsync(yearinfos.FirstOrDefault());
                    }
                    else
                    {
                        qualityCostYearInfo.QualityCostId = qualityCostInfoId;
                        await _qualityCostProportionYearInfoRepository.InsertAsync(qualityCostYearInfo);
                    }
                }
            }
        }




        /// <summary>
        /// 获取制造成本里计算字段参数维护
        /// </summary>
        /// <returns></returns>
        public async Task<ManufacturingCostDto> GetManufacturingCost()
        {
            List<ManufacturingCostInfo> result = await _manufacturingCostInfoRepository.GetAll().ToListAsync();

            ManufacturingCostDto dto = new ManufacturingCostDto();
            dto.ManufacturingCosts = result;

            return dto;
        }

        /// <summary>
        /// 保存制造成本里计算字段参数维护
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task SaveManufacturingCost(ManufacturingCostDto dto)
        {
            List<ManufacturingCostInfo> result = dto.ManufacturingCosts;
            await _manufacturingCostInfoRepository.HardDeleteAsync(s => s.Id>0);
            result.ForEach(async manufacturingCost =>
            {
                await _manufacturingCostInfoRepository.InsertOrUpdateAsync(manufacturingCost);
            });

        }
    }
}
