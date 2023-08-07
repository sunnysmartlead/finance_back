using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.ObjectMapping;
using Finance.Audit;
using Finance.Dto;
using Finance.EngineeringDepartment.Dto;
using Finance.Ext;
using Finance.Nre;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment
{
    /// <summary>
    /// 损耗率接口类
    /// </summary>
    public class LossRateAppService : FinanceAppServiceBase
    {
        private readonly IRepository<ElectronicBomInfo, long> _electronicBomInfoRepository;
        private readonly IRepository<StructureBomInfo, long> _structureBomInfoRepository;

        private readonly IRepository<LossRateInfo, long> _lossRateInfoRepository;
        private readonly IRepository<LossRateYearInfo, long> _lossRateYearInfoRepository;

        private readonly IRepository<ModelCount, long> _modelCountRepository;
        /// <summary>
        ///  零件是否全部录入 依据实体类
        /// </summary>
        private readonly IRepository<NreIsSubmit, long> _productIsSubmit;

        private readonly IObjectMapper _objectMapper;

        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="electronicBomInfoRepository"></param>
        /// <param name="structureBomInfoRepository"></param>
        /// <param name="lossRateInfoRepository"></param>
        /// <param name="lossRateYearInfoRepository"></param>
        /// <param name="objectMapper"></param>
        /// <param name="flowAppService"></param>
        /// <param name="modelCountRepository"></param>
        /// <param name="productIsSubmit"></param>
        public LossRateAppService(IRepository<ElectronicBomInfo, long> electronicBomInfoRepository, IRepository<StructureBomInfo, long> structureBomInfoRepository, IRepository<LossRateInfo, long> lossRateInfoRepository, IRepository<LossRateYearInfo, long> lossRateYearInfoRepository, IObjectMapper objectMapper, AuditFlowAppService flowAppService, IRepository<ModelCount, long> modelCountRepository, IRepository<NreIsSubmit, long> productIsSubmit)
        {
            _electronicBomInfoRepository = electronicBomInfoRepository;
            _structureBomInfoRepository = structureBomInfoRepository;
            _lossRateInfoRepository = lossRateInfoRepository;
            _lossRateYearInfoRepository = lossRateYearInfoRepository;
            _objectMapper = objectMapper;
            _flowAppService = flowAppService;
            _modelCountRepository = modelCountRepository;
            _productIsSubmit = productIsSubmit;
        }

        /// <summary>
        /// 对比新BOM中损耗率列表和旧的损耗率表，并更新损耗率表
        /// </summary>
        /// <param name="oldLossRateInfos"></param>
        /// <param name="newLossRateInfos"></param>
        /// <returns></returns>
        private async Task<List<LossRateDto>> CheckLossRateInfo(List<LossRateInfo> oldLossRateInfos, List<LossRateDto> newLossRateInfos)
        {
            List<LossRateDto> result = new();

            //旧的损耗率比对新损耗率
            foreach (var lossRateInfo in oldLossRateInfos)
            {
                bool isExsit = false;
                //是否还存在
                foreach (var newElecLossRateDto in newLossRateInfos)
                {
                    if (newElecLossRateDto.SuperType == lossRateInfo.SuperType && newElecLossRateDto.CategoryName == lossRateInfo.CategoryName)
                    {
                        isExsit = true;
                    }
                }

                //不存在则删除对应损耗率
                if (!isExsit)
                {
                    await _lossRateYearInfoRepository.HardDeleteAsync(p => lossRateInfo.Id.Equals(p.LossRateInfoId));
                    await _lossRateInfoRepository.HardDeleteAsync(lossRateInfo);
                }
                else
                {
                    List<LossRateYearInfo> lossRateYearInfos = await _lossRateYearInfoRepository.GetAll()
                                                                .Where(p => lossRateInfo.Id.Equals(p.LossRateInfoId)).ToListAsync();
                    if (lossRateYearInfos.Count > 0)
                    {
                        var lossRateYearDto = _objectMapper.Map<List<LossRateYearDto>>(lossRateYearInfos);
                        var lossRateDto = _objectMapper.Map<LossRateDto>(lossRateInfo);

                        lossRateDto.LossRateYearList = lossRateYearDto;
                        result.Add(lossRateDto);
                    }
                }
            }

            //新损耗率比对旧损耗率
            foreach (var newElecDto in newLossRateInfos)
            {
                bool isExsit = false;
                //是否已存在
                foreach (var lossRate in oldLossRateInfos)
                {
                    if (lossRate.SuperType == newElecDto.SuperType && lossRate.CategoryName == newElecDto.CategoryName)
                    {
                        isExsit = true;
                    }
                }
                //不存在，则是一个新增
                if (!isExsit)
                {
                    List<LossRateYearInfo> lossRateYearInfos = new();

                    var lossRateYearDto = _objectMapper.Map<List<LossRateYearDto>>(lossRateYearInfos);
                    newElecDto.LossRateYearList = lossRateYearDto;
                    result.Add(newElecDto);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取电子损耗率类型(新的第一个版本核价时使用)
        /// </summary>
        public async Task<List<LossRateDto>> GetElecLossRateType(LossRateDto input)
        {
            try
            {
                List<LossRateDto> bomTypeDtos = new();
                var electronicBomInfo = await _electronicBomInfoRepository.GetAll()
                        .Where(p => input.AuditFlowId.Equals(p.AuditFlowId))
                        .Where(p => input.ProductId.Equals(p.ProductId))
                        .Where(p => "是".Equals(p.IsInvolveItem)).ToListAsync();
                if (electronicBomInfo != null)
                {
                    int index = 1;          //序号存放位置
                    bool isInclude = false; //物料是否包含
                    foreach (var electronicBom in electronicBomInfo)
                    {
                        foreach (var ret in bomTypeDtos)//判断物料大类是否存在
                        {
                            if (ret.CategoryName == electronicBom.CategoryName)
                            {
                                isInclude = true;
                            }
                        }
                        if (!isInclude)//不存在则存入
                        {
                            LossRateDto dto = new()
                            {
                                AuditFlowId = electronicBom.AuditFlowId,
                                ProductId = electronicBom.ProductId,
                                SuperType = FinanceConsts.ElectronicName,
                                CategoryName = electronicBom.CategoryName
                            };
                            dto.IdNumber = index++;
                            bomTypeDtos.Add(dto);
                        }
                        isInclude = false;
                    }
                }

                List<LossRateInfo> lossRateInfos = await _lossRateInfoRepository.GetAll()
                                                .Where(p => input.AuditFlowId.Equals(p.AuditFlowId))
                                                .Where(p => input.ProductId.Equals(p.ProductId))
                                                .Where(p => p.SuperType.Equals(FinanceConsts.ElectronicName))
                                                .ToListAsync();
                return await this.CheckLossRateInfo(lossRateInfos, bomTypeDtos);
            }
            catch
            {
                throw new FriendlyException("获取电子料信息失败！");
            }
        }

        /// <summary>
        /// 展示电子损耗率(代入旧版本损耗率)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<LossRateDto>> GetElecOldLossRateInfo(LossRateDto input)
        {
            return await this.GetElecLossRateType(input);
        }

        /// <summary>
        /// 获取结构损耗率类型(新的第一个版本核价时使用)
        /// </summary>
        public async Task<List<LossRateDto>> GetStructLossRateType(LossRateDto input)
        {
            try
            {
                List<LossRateDto> bomTypeDtos = new();
                var structureBomInfo = await _structureBomInfoRepository.GetAll()
                        .Where(p => input.AuditFlowId.Equals(p.AuditFlowId))
                        .Where(p => input.ProductId.Equals(p.ProductId))
                        .Where(p => "是".Equals(p.IsInvolveItem)).ToListAsync();
                if (structureBomInfo != null)
                {
                    List<int> SuperTypeIndex = new();//超级大种类索引
                    List<string> names = new();//超级大种类名字保存
                    bool isInclude = false;

                    foreach (var structureBom in structureBomInfo)
                    {

                        foreach (var ret in bomTypeDtos)//判断物料大类是否存在
                        {
                            if (ret.CategoryName == structureBom.CategoryName)
                            {
                                isInclude = true;
                            }
                        }
                        if (!isInclude)
                        {

                            LossRateDto dto = new()
                            {
                                AuditFlowId = structureBom.AuditFlowId,
                                ProductId = structureBom.ProductId,
                                SuperType = structureBom.SuperTypeName,
                                CategoryName = structureBom.CategoryName,
                            };
                            if (!names.Contains(structureBom.SuperTypeName))
                            {
                                names.Add(structureBom.SuperTypeName);//插入新的超级大种类
                                SuperTypeIndex.Add(1);
                            }
                            int index = names.IndexOf(structureBom.SuperTypeName);//查找超级大种类所在位置的序号
                            dto.IdNumber = SuperTypeIndex[index]++;
                            bomTypeDtos.Add(dto);
                        }
                        isInclude = false;
                    }
                }

                List<LossRateInfo> lossRateInfos = await _lossRateInfoRepository.GetAll()
                                                .Where(p => input.AuditFlowId.Equals(p.AuditFlowId))
                                                .Where(p => input.ProductId.Equals(p.ProductId))
                                                .Where(p => !p.SuperType.Equals(FinanceConsts.ElectronicName))
                                                .ToListAsync();
                return await this.CheckLossRateInfo(lossRateInfos, bomTypeDtos);
            }
            catch
            {
                throw new FriendlyException("获取结构料信息失败！");
            }
        }

        /// <summary>
        /// 展示结构损耗率(代入旧版本损耗率)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<LossRateDto>> GetStructOldLossRateInfo(LossRateDto input)
        {
            return await this.GetStructLossRateType(input);
        }

        /// <summary>
        /// 存入损耗率
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        //[ParameterValidator]
        public async Task SaveLossRateInfo(List<LossRateDto> inputList)
        {
            LossRateDto firstLossRateDto = inputList.FirstOrDefault();
            if (firstLossRateDto.AuditFlowId == 0)
            {
                throw new FriendlyException("输入流程ID不能为0");
            }
            string lossRateInterface =  firstLossRateDto.SuperType.Equals(FinanceConsts.ElectronicName) ? AuditFlowConsts.AF_ElecLossRateInput : AuditFlowConsts.AF_StructLossRateInput;
            //查询核价需求导入时的零件信息
            var productIds = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == firstLossRateDto.AuditFlowId);
            bool isExsit = false;
            foreach (var productId in productIds)
            {
                if (productId.Id.Equals(firstLossRateDto.ProductId))
                {
                    isExsit = true;
                }
            }
            if (!isExsit)
            {
                throw new FriendlyException("输入的零件号不存在");
            }

            //查询已保存的Bom表中零件信息
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(firstLossRateDto.AuditFlowId) && p.SolutionId.Equals(firstLossRateDto.ProductId) && p.EnumSole.Equals(lossRateInterface));
            if (productIsSubmits.Count is not 0)
            {
                throw new FriendlyException(firstLossRateDto.ProductId + ":该零件id已经提交过了");
            }
            else
            {
                foreach (var lossRateDto in inputList)
                {
                    long lossRateInfoId = 0;
                    var lossRateInfo = _objectMapper.Map<LossRateInfo>(lossRateDto);
                    var lossRateInfoList = await _lossRateInfoRepository.GetAllListAsync(p => p.AuditFlowId == lossRateDto.AuditFlowId && p.ProductId == lossRateDto.ProductId && p.SuperType == lossRateInfo.SuperType && p.CategoryName == lossRateInfo.CategoryName);
                    if (lossRateInfoList.Count == 0)
                    {
                        lossRateInfoId = await _lossRateInfoRepository.InsertAndGetIdAsync(lossRateInfo);
                    }
                    else
                    {
                        lossRateInfoId = lossRateInfoList.FirstOrDefault().Id;
                    }

                    var lossRateYearInfos = _objectMapper.Map<List<LossRateYearInfo>>(lossRateDto.LossRateYearList);
                    foreach (var lossRateYearInfo in lossRateYearInfos)
                    {
                        LossRateYearInfo yearInfo;
                        var lossRateYearInfoList = await _lossRateYearInfoRepository.GetAllListAsync(p => p.AuditFlowId == lossRateInfo.AuditFlowId && p.LossRateInfoId == lossRateInfoId && p.Year == lossRateYearInfo.Year);
                        if (lossRateYearInfoList.Count > 0)
                        {
                            yearInfo = lossRateYearInfoList.FirstOrDefault();
                            yearInfo.Rate = lossRateYearInfo.Rate;
                        }
                        else
                        {
                            lossRateYearInfo.AuditFlowId = lossRateInfo.AuditFlowId;
                            lossRateYearInfo.LossRateInfoId = lossRateInfoId;
                            yearInfo = lossRateYearInfo;
                        }
                        await _lossRateYearInfoRepository.InsertOrUpdateAsync(yearInfo);
                    }
                }

                #region 录入完成之后
                await _productIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = firstLossRateDto.AuditFlowId, SolutionId = firstLossRateDto.ProductId, EnumSole = lossRateInterface });
                #endregion

                List<NreIsSubmit> allProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(firstLossRateDto.AuditFlowId) && p.EnumSole.Equals(lossRateInterface));
                //当前已保存的确认表中零件数目等于 核价需求导入时的零件数目
                if (productIds.Count == allProductIsSubmits.Count + 1)
                {
                    //执行跳转
                    if (AbpSession.UserId is null)
                    {
                        throw new FriendlyException("请先登录");
                    }

                    ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                    {
                        AuditFlowId = firstLossRateDto.AuditFlowId,
                        ProcessIdentifier = lossRateInterface,
                        UserId = AbpSession.UserId.Value,
                        Opinion = OPINIONTYPE.Submit_Agreee,
                    });
                }
            }
        }

        /// <summary>
        /// 损耗率 退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task ClearLossRateInfoState(long Id, string lossRateInterface)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(lossRateInterface));
            foreach (NreIsSubmit item in productIsSubmits)
            {
                await _productIsSubmit.HardDeleteAsync(item);
            }
        }


    }
}
