﻿using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Finance.Audit;
using Finance.DemandApplyAudit;
using Finance.Dto;
using Finance.Ext;
using Finance.Infrastructure;
using Finance.PriceEval;
using Finance.ProductDevelopment.Dto;
using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProductDevelopment
{
    /// <summary>
    /// 产品开发部输入接口类
    /// </summary>
    public class ProductDevelopmentInputAppService : FinanceAppServiceBase
    {
        private readonly ILogger<ProductDevelopmentInput> _logger;
        private readonly IRepository<ProductDevelopmentInput, long> _productDevelopmentInputRepository;
        private readonly IRepository<ModelCount, long> _modelCountRepository;
        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;
        private readonly IRepository<FileManagement, long> _fileManagementRepository;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        private readonly IRepository<Solution, long> _solutionTableRepository;
        private readonly IObjectMapper _objectMapper;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;

        public ProductDevelopmentInputAppService(ILogger<ProductDevelopmentInput> logger, IRepository<ProductDevelopmentInput, long> productDevelopmentInputRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<FileManagement, long> fileManagementRepository, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, IRepository<Solution, long> solutionTableRepository, IObjectMapper objectMapper, IRepository<Solution, long> resourceSchemeTable)
        {
            _logger = logger;
            _productDevelopmentInputRepository = productDevelopmentInputRepository;
            _modelCountRepository = modelCountRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _fileManagementRepository = fileManagementRepository;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _solutionTableRepository = solutionTableRepository;
            _objectMapper = objectMapper;
            _resourceSchemeTable = resourceSchemeTable;
        }

        #region 快速核报价

        /// <summary>
        /// 物流基础信息表快速核报价
        /// </summary>
        /// <param name="AuditFlowId"></param>
        /// <param name="QuoteAuditFlowId"></param>
        /// <param name="solutionIdAndQuoteSolutionIds"></param>
        /// <returns></returns>
        internal async Task FastPostProductDevelopmentCopy(long AuditFlowId, long QuoteAuditFlowId, List<SolutionIdAndQuoteSolutionId> solutionIdAndQuoteSolutionIds)
        {
            foreach (var item in solutionIdAndQuoteSolutionIds)
            {
                List<ProductDevelopmentInput> productDevelopments = await _productDevelopmentInputRepository.GetAllListAsync(p => p.AuditFlowId.Equals(QuoteAuditFlowId) && p.SolutionId.Equals(item.QuoteSolutionId));
                productDevelopments.Select(p => { p.AuditFlowId = AuditFlowId; p.Id = 0; p.SolutionId = item.NewSolutionId; return p; }).ToList();
                await _productDevelopmentInputRepository.BulkInsertAsync(productDevelopments);
            }
        }

        #endregion


        /// <summary>
        /// 保存产品开发部界面包装基础输入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<ProductDevelopmentInput> SaveProductDevelopmentInput(ProductDevelopmentInputDto input)
        {

            if (input.Picture3DFileId == 0 && input.Opinion == FinanceConsts.Done)
            {
                throw new FriendlyException("3D爆炸图必选上传");
                //input.Picture3DFileId = "0";
            }
            ProductDevelopmentInput entity;
            var productInput = await _productDevelopmentInputRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.SolutionId == input.SolutionId);

            if (productInput.Count > 0)
            {
                entity = productInput.FirstOrDefault();
            }
            else
            {
                entity = new();
                entity.ProductId = input.ProductId;
                entity.SolutionId = input.SolutionId;
                entity.AuditFlowId = input.AuditFlowId;
            }
            entity.Picture3DFileId = Convert.ToInt64(input.Picture3DFileId);
            entity.Pixel = input.Pixel.IsNullOrEmpty() ? 0 : Convert.ToDouble(input.Pixel);
            entity.FOV = input.FOV.IsNullOrEmpty() ? 0 : Convert.ToDouble(input.FOV);
            #region 3.14新增
            entity.HFOV = input.HFOV;
            entity.VFOV = input.VFOV;
            entity.DFOV = input.DFOV;
            #endregion
            entity.OuterPackagingLength = input.OuterPackagingLength.IsNullOrEmpty() ? 0 : Convert.ToDouble(input.OuterPackagingLength);
            entity.OuterPackagingWidth = input.OuterPackagingWidth.IsNullOrEmpty() ? 0 : Convert.ToDouble(input.OuterPackagingWidth);
            entity.OuterPackagingHeight = input.OuterPackagingHeight.IsNullOrEmpty() ? 0 : Convert.ToDouble(input.OuterPackagingHeight);
            entity.SingleProductWeight = input.SingleProductWeight.IsNullOrEmpty() ? 0 : Convert.ToDouble(input.SingleProductWeight);
            entity.SingleBoxQuantity = input.SingleBoxQuantity.IsNullOrEmpty() ? 0 : Convert.ToInt32(input.SingleBoxQuantity);
            entity.InnerPackagingLength = input.InnerPackagingLength.IsNullOrEmpty() ? 0 : Convert.ToDouble(input.InnerPackagingLength);
            entity.InnerPackagingWidth = input.InnerPackagingWidth.IsNullOrEmpty() ? 0 : Convert.ToDouble(input.InnerPackagingWidth);
            entity.InnerPackagingHeight = input.InnerPackagingHeight.IsNullOrEmpty() ? 0 : Convert.ToDouble(input.InnerPackagingHeight);
            entity.IsHit = input.IsHit;
            entity.BoxesPerPallet = input.BoxesPerPallet.IsNullOrEmpty() ? 0 : Convert.ToInt32(input.BoxesPerPallet);
            entity.QuantityPerBox = input.QuantityPerBox.IsNullOrEmpty() ? 0 : Convert.ToInt32(input.QuantityPerBox);
            entity.Remarks = input.Remarks;

            ProductDevelopmentInput entityRet = await _productDevelopmentInputRepository.InsertOrUpdateAsync(entity);

            return entityRet;

        }

        /// <summary>
        /// 返回产品开发部录入信息
        /// </summary>
        public async Task<PostProductDevelopmentInputDto> PostProductDevelopmentInput(PostProductDevelopmentInputDto dto)
        {

            //ProductDevelopmentInput data = _objectMapper.Map<ProductDevelopmentInput>(dto);

            List<ProductDevelopmentInput> data = await _productDevelopmentInputRepository.GetAll()
                    .Where(p => dto.AuditFlowId.Equals(p.AuditFlowId))
                    .Where(p => dto.SolutionId.Equals(p.SolutionId)).ToListAsync();
            PostProductDevelopmentInputDto result = new PostProductDevelopmentInputDto();
            if (data.Count == 1)
            {
                result.Picture3DFileId = data.FirstOrDefault().Picture3DFileId.ToString();
                result.Pixel = data.FirstOrDefault().Pixel.ToString();
                result.FOV = data.FirstOrDefault().FOV.ToString();
                #region 3.14新增
                result.HFOV = data.FirstOrDefault().HFOV;
                result.VFOV = data.FirstOrDefault().VFOV;
                result.DFOV = data.FirstOrDefault().DFOV;
                #endregion
                result.OuterPackagingLength = data.FirstOrDefault().OuterPackagingLength.ToString();
                result.OuterPackagingWidth = data.FirstOrDefault().OuterPackagingWidth.ToString();
                result.OuterPackagingHeight = data.FirstOrDefault().OuterPackagingHeight.ToString();
                result.SingleProductWeight = data.FirstOrDefault().SingleProductWeight.ToString();
                result.SingleBoxQuantity = data.FirstOrDefault().SingleBoxQuantity.ToString();
                result.InnerPackagingLength = data.FirstOrDefault().InnerPackagingLength.ToString();
                result.InnerPackagingWidth = data.FirstOrDefault().InnerPackagingWidth.ToString();
                result.InnerPackagingHeight = data.FirstOrDefault().InnerPackagingHeight.ToString();
                result.IsHit = data.FirstOrDefault().IsHit;
                result.BoxesPerPallet = data.FirstOrDefault().BoxesPerPallet.ToString();
                result.QuantityPerBox = data.FirstOrDefault()?.QuantityPerBox.ToString();
                result.Remarks = data[0].Remarks;
                var productInformation = await _priceEvaluationRepository.GetAllListAsync(p => p.AuditFlowId == dto.AuditFlowId);
                result.ShippingType = _financeDictionaryDetailRepository.FirstOrDefault(p => p.Id == productInformation.FirstOrDefault().ShippingType).DisplayName;
                result.PackagingType = _financeDictionaryDetailRepository.FirstOrDefault(p => p.Id == productInformation.FirstOrDefault().PackagingType).DisplayName;
                result.PlaceOfDelivery = productInformation.FirstOrDefault()?.PlaceOfDelivery;
            }
            return result;

        }

        /// <summary>
        /// 根据流程号获取方案号
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async Task<List<Solution>> GetProductByAuditFlowId(long auditFlowId)
        {
            try
            {
                List<Solution> resultNew = new List<Solution>();
                List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => auditFlowId == p.AuditFlowId);
                List<string> strings = result.Select(p => p.ModuleName).Distinct().ToList();
                strings.ForEach(m =>
                {

                    resultNew.AddRange(result.Where(p => p.ModuleName == m));
                });
                return resultNew;
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 客户特殊性需求与SORId
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        public async Task<SorDto> GetSorByAuditFlowId(long auditFlowId)
        {

            //PriceEvaluation priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
            //return priceEvaluation;
            SorDto SorDto = new();
            try
            {
                var priceInfo = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
                string CustomerSpecialRequest = priceInfo.CustomerSpecialRequest;
                //long SorFileId = long.Parse(priceInfo.SorFile);
                long SorFileId = JsonConvert.DeserializeObject<List<long>>(priceInfo.SorFile).FirstOrDefault();
                var fileName = await _fileManagementRepository.GetAllListAsync(p => p.Id == SorFileId);
                if (fileName.Count > 0)
                {
                    SorDto.AuditFlowId = auditFlowId;
                    SorDto.SorFileName = fileName.FirstOrDefault().Name;
                    SorDto.SorFileId = SorFileId;
                    SorDto.CustomerSpecialRequest = CustomerSpecialRequest;
                    SorDto.IsSuccess = true;
                    return SorDto;
                }
                else
                {
                    throw new FriendlyException("文件找不到");
                }
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
    }
}
