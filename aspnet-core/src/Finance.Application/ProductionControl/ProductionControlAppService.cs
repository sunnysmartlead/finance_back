using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Finance.Audit;
using Finance.Dto;
using Finance.Ext;
using Finance.Nre;
using Finance.PriceEval;
using Finance.ProductionControl.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProductionControl
{
    /// <summary>
    /// 物流成本接口类
    /// </summary>
    public class ProductionControlAppService : FinanceAppServiceBase
    {
        private readonly IRepository<ProductionControlInfo, long> _productionControlInfoRepository;

        private readonly IRepository<Pcs, long> _pcsRepository;

        private readonly IRepository<PcsYear, long> _pcsYearRepository;
        
        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;

        private readonly IRepository<ModelCount, long> _modelCountRepository;
        /// <summary>
        ///  零件是否全部录入 依据实体类
        /// </summary>
        private readonly IRepository<NreIsSubmit, long> _productIsSubmit;


        /// <summary>
        /// 流程流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="productionControlInfoRepository"></param>
        /// <param name="pcsRepository"></param>
        /// <param name="pcsYearRepository"></param>
        /// <param name="priceEvaluationRepository"></param>
        /// <param name="flowAppService"></param>
        /// <param name="modelCountRepository"></param>
        /// <param name="productIsSubmit"></param>
        public ProductionControlAppService(IRepository<ProductionControlInfo, long> productionControlInfoRepository, IRepository<Pcs, long> pcsRepository, IRepository<PcsYear, long> pcsYearRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, AuditFlowAppService flowAppService, IRepository<ModelCount, long> modelCountRepository, IRepository<NreIsSubmit, long> productIsSubmit)
        {
            _productionControlInfoRepository = productionControlInfoRepository;
            _pcsRepository = pcsRepository;
            _pcsYearRepository = pcsYearRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _flowAppService = flowAppService;
            _modelCountRepository = modelCountRepository;
            _productIsSubmit = productIsSubmit;
        }



        /// <summary>
        /// 获取年份表
        /// </summary>
        public async Task<List<int>> GetAllYearsFrom(long auditFlowId)
        {
            List<int> result = new List<int>();
            List<Pcs> data = await _pcsRepository.GetAll().Where(p => auditFlowId.Equals(p.AuditFlowId)).ToListAsync();

            if (data.Count > 0) {
                List<PcsYear> pcsYearsInfo = await _pcsYearRepository.GetAllListAsync(p => p.PcsId == data.FirstOrDefault().Id);
                if (pcsYearsInfo.Count > 0)
                {
                    foreach (PcsYear pcsYear in pcsYearsInfo)
                    {
                        result.Add(pcsYear.Year);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 保存物流成本
        /// </summary>
        [ParameterValidator]
        public async Task SavaProductionControl(ProductControlInputDto dto)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(dto.AuditFlowId) && p.SolutionId.Equals(dto.ProductId) && p.EnumSole.Equals(AuditFlowConsts.AF_LogisticsCostInput));
            if (productIsSubmits.Count is not 0)
            {
                throw new FriendlyException(dto.ProductId + ":该零件id已经提交过了");
            }
            else
            {
                //查询核价需求导入时的零件信息
                var productIds = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == dto.AuditFlowId);
                List<ProductionControlInfo> infoList = dto.InfoList;

                long ProductId = dto.ProductId;
                long AuditFlowId = dto.AuditFlowId;
                await _productionControlInfoRepository.HardDeleteAsync(s => s.AuditFlowId.Equals(AuditFlowId) && s.ProductId.Equals(ProductId));

                infoList.ForEach(info =>
                {
                    info.AuditFlowId = AuditFlowId;
                    info.ProductId = ProductId;

                });
                for (int i = 0; i < infoList.Count; i++)
                {
                    await _productionControlInfoRepository.InsertOrUpdateAsync(infoList[i]);
                }

                #region 录入完成之后
                await _productIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = dto.AuditFlowId, SolutionId = dto.ProductId, EnumSole = AuditFlowConsts.AF_LogisticsCostInput });
                #endregion

                List<NreIsSubmit> allProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(dto.AuditFlowId) && p.EnumSole.Equals(AuditFlowConsts.AF_LogisticsCostInput));
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
                        AuditFlowId = dto.AuditFlowId,
                        ProcessIdentifier = AuditFlowConsts.AF_LogisticsCostInput,
                        UserId = AbpSession.UserId.Value,
                        Opinion = OPINIONTYPE.Submit_Agreee
                    });
                }
            }
        }

        /// <summary>
        /// 物流成本录入 退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task ClearLogisticsCostInputState(long Id)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(AuditFlowConsts.AF_LogisticsCostInput));
            foreach (NreIsSubmit item in productIsSubmits)
            {
                await _productIsSubmit.HardDeleteAsync(item);
            }
        }

        /// <summary>
        /// 获取物流成本
        /// </summary>
        public async Task<ProductControlInputDto> GetProductionControl(ProductInfoDto dto)
        {
            ProductControlInputDto  productControlInputDto = new();

            var productionControlInfoList =  await _productionControlInfoRepository.GetAllListAsync(p => p.AuditFlowId == dto.AuditFlowId && p.ProductId == dto.ProductId);

            productControlInputDto.AuditFlowId = dto.AuditFlowId;
            productControlInputDto.ProductId = dto.ProductId;
            productControlInputDto.InfoList = productionControlInfoList;
            return productControlInputDto;
        }
    }
}
