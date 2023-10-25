using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.ObjectMapping;
using Abp.UI;
using Finance.Audit;
using Finance.DemandApplyAudit;
using Finance.Ext;
using Finance.Nre;
using Finance.PriceEval;
using Finance.ProductDevelopment.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Finance.ProductDevelopment
{
    /// <summary>
    /// 结构BOM录入接口类
    /// </summary>

    public class StructionBomAppService : FinanceAppServiceBase
    {
        private readonly ILogger<StructionBomAppService> _logger;
        private readonly IRepository<StructureBomInfo, long> _structureBomInfoRepository;
        private readonly IRepository<StructureBomInfoBak, long> _structureBomInfoBakRepository;
        private readonly IRepository<ModelCount, long> _modelCountRepository;
        private readonly IRepository<StructBomDifferent, long> _structBomDifferentRepository;
        private readonly IRepository<Solution, long> _solutionTableRepository;
        /// <summary>
        ///  零件是否全部录入 依据实体类
        /// </summary>
        private readonly IRepository<NreIsSubmit, long> _productIsSubmit;

        /// <summary>
        /// 界面流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;
        private readonly ProductDevelopmentInputAppService _productDevelopmentInputAppService;
        private readonly IObjectMapper _objectMapper;

        private readonly WorkflowInstanceAppService _workflowInstanceAppService;
        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;
        public StructionBomAppService(ILogger<StructionBomAppService> logger, IRepository<StructureBomInfo, long> structureBomInfoRepository, IRepository<StructureBomInfoBak, long> structureBomInfoBakRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<StructBomDifferent, long> structBomDifferentRepository, IRepository<Solution, long> solutionTableRepository, IRepository<NreIsSubmit, long> productIsSubmit, AuditFlowAppService flowAppService, ProductDevelopmentInputAppService productDevelopmentInputAppService, IObjectMapper objectMapper, WorkflowInstanceAppService workflowInstanceAppService, IRepository<Solution, long> resourceSchemeTable)
        {
            _logger = logger;
            _structureBomInfoRepository = structureBomInfoRepository;
            _structureBomInfoBakRepository = structureBomInfoBakRepository;
            _modelCountRepository = modelCountRepository;
            _structBomDifferentRepository = structBomDifferentRepository;
            _solutionTableRepository = solutionTableRepository;
            _productIsSubmit = productIsSubmit;
            _flowAppService = flowAppService;
            _productDevelopmentInputAppService = productDevelopmentInputAppService;
            _objectMapper = objectMapper;
            _workflowInstanceAppService = workflowInstanceAppService;
            _resourceSchemeTable = resourceSchemeTable;
        }




        /// <summary>
        /// 上传后读取结构bom接口
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<ProductDevelopmentInputDto> LoadExcel(IFormFile file)
        {
            //打开上传文件的输入流
            Stream stream = file.OpenReadStream();

            //根据文件流创建excel数据结构
            IWorkbook workbook = WorkbookFactory.Create(stream);
            stream.Close();

            //尝试获取第一个sheet
            var sheet = workbook.GetSheetAt(0);

            List<StructureBomDto> list = new List<StructureBomDto>();
            ProductDevelopmentInputDto result = new ProductDevelopmentInputDto();

            //判断是否获取到 sheet
            if (sheet != null)
            {
                //最后一列的标号
                int lastRowNum = sheet.LastRowNum;
                //从第三行开始获取
                string superType = "";

                List<string> superTypes = new List<string> { FinanceConsts.ElectronicName, FinanceConsts.StructuralName, FinanceConsts.GlueMaterialName, FinanceConsts.SMTOutSourceName, FinanceConsts.PackingMaterialName };

                string daLei = "";
                for (int i = 1; i < lastRowNum + 1; i++)
                {
                    var row = sheet.GetRow(i);
                    StructureBomDto dto = new StructureBomDto();
                    bool isMatch = false;
                    for (int j = 0; j < 15; j++)
                    {
                        //输出
                        if (null == row.GetCell(j) || string.IsNullOrEmpty(row.GetCell(j).ToString()))
                        {
                            if (j > 1)
                            {
                                throw new FriendlyException("第" + (i + 1) + "行第" + (j + 1) + "列未进行有效填录，请检查！");
                            }
                        }
                        else
                        {
                            try
                            {
                                if (j == 0)
                                {
                                    isMatch = Regex.IsMatch(row.GetCell(j).ToString(), @"^\d+$");
                                    if (isMatch)
                                    {
                                        dto = DataToList(dto, row.GetCell(j), j);
                                    }
                                    else
                                    {
                                        superType = row.GetCell(j).ToString();
                                        if (!superTypes.Contains(superType))
                                        {
                                            throw new FriendlyException("结构Bom中超级大种类不包含：" + superType + "，请检查！");
                                        }

                                        break;
                                    }
                                }
                                else if (j == 1)
                                {
                                    daLei = row.GetCell(j).ToString();
                                }
                                else
                                {
                                    dto = DataToList(dto, row.GetCell(j), j);
                                    if (j == 5 && dto.IsInvolveItem.Equals("是"))
                                    {
                                        if (dto.AssemblyQuantity <= 0)
                                        {
                                            throw new FriendlyException("第" + (i + 1) + "行是否涉及填了“是”，装配数量不能再填0,请检查！");
                                        }
                                    }
                                    if (j == 5 && dto.IsInvolveItem.Equals("否"))
                                    {
                                        if (dto.AssemblyQuantity != 0)
                                        {
                                            throw new FriendlyException("第" + (i + 1) + "行是否涉及填了“否”，装配数量必须填0,请检查！");
                                        }
                                    }

                                }
                            }
                            catch
                            {
                                throw new FriendlyException("第" + (i + 1) + "行第" + (j + 1) + "列数据格式错误，请检查！");
                            }
                        }
                    }

                    if (isMatch)
                    {
                        dto.SuperTypeName = superType;
                        dto.CategoryName = daLei;
                        list.Add(dto);
                    }
                }
            }

            result.StructureBomDtos = list;
            result.IsSuccess = true;
            return result;

        }

        /// <summary>
        /// 列数据转换成指定的表值
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="data"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static StructureBomDto DataToList(StructureBomDto dto, object data, int column)
        {
            if (data == null) { return null; }

            if (column == 0)
            {
                dto.IdNumber = int.Parse(data.ToString());
            }
            else if (column == 2)
            {
                dto.TypeName = data.ToString();
            }
            else if (column == 3)
            {
                dto.IsInvolveItem = data.ToString();
            }
            else if (column == 4)
            {
                dto.SapItemNum = Regex.Replace(data.ToString(), @"\s", "");
            }
            else if (column == 5)
            {
                dto.DrawingNumName = data.ToString();
            }
            else if (column == 6)
            {
                dto.AssemblyQuantity = double.Parse(data.ToString());
            }
            else if (column == 7)
            {
                dto.OverallDimensionSize = data.ToString();
            }
            else if (column == 8)
            {
                dto.MaterialName = data.ToString();
            }
            else if (column == 9)
            {
                dto.WeightNumber = double.Parse(data.ToString());
            }
            else if (column == 10)
            {
                dto.MoldingProcess = data.ToString();
            }
            else if (column == 11)
            {
                dto.IsNewMouldProduct = data.ToString();
            }
            else if (column == 12)
            {
                dto.SecondaryProcessingMethod = data.ToString();
            }
            else if (column == 13)
            {
                dto.SurfaceTreatmentMethod = data.ToString();
            }
            else if (column == 14)
            {
                dto.DimensionalAccuracyRemark = data.ToString();
            }
            return dto;
        }
        /// <summary>
        /// 总的方案
        /// </summary>
        internal async Task<List<SolutionModel>> TotalSolution(long auditFlowId)
        {
            List<Solution> result = await _resourceSchemeTable.GetAllListAsync(p => auditFlowId == p.AuditFlowId);
            result = result.OrderBy(p => p.ModuleName).ToList();
            List<SolutionModel> partModel = (from a in result
                                             select new SolutionModel
                                             {
                                                 SolutionId = a.Id,
                                                 SolutionName = a.SolutionName,
                                                 ProductId = a.Productld,
                                             }).ToList();
            return partModel;
        }
        /// <summary>
        /// 接收前端数据存入本地接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        public async Task SaveStructionBom(ProductDevelopmentInputDto dto)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(dto.AuditFlowId) && p.SolutionId.Equals(dto.SolutionId) && p.EnumSole.Equals(AuditFlowConsts.AF_StructBomImport));


            if (productIsSubmits.Count is not 0)
            {
                throw new FriendlyException(dto.SolutionId + ":该零件方案id已经提交过了");
            }
            else
            {
                await _productDevelopmentInputAppService.SaveProductDevelopmentInput(dto);
                //查询总方案
                List<SolutionModel> solutionId = await TotalSolution(dto.AuditFlowId);
                //var solutionTable =  _solutionTableRepository.GetAll().Where(p => p.Id == dto.SolutionId).FirstOrDefault();


                List<StructureBomDto> structureBomDtos = dto.StructureBomDtos;
                long AuditFlowId = dto.AuditFlowId;
                long SolutionId = dto.SolutionId;
                long ProductId = dto.ProductId;
                //long ProductId = solutionTable.ProductId//dto.ProductId;
                structureBomDtos.ForEach(bomInfo =>
                {
                    bomInfo.AuditFlowId = AuditFlowId;
                    bomInfo.SolutionId = SolutionId;
                    bomInfo.ProductId = ProductId;
                });

                //要保存的bom表list
                var listBak = _objectMapper.Map<List<StructureBomInfoBak>>(structureBomDtos);
                if (listBak.Count > 0)
                {
                    await _structureBomInfoBakRepository.HardDeleteAsync(p => p.AuditFlowId == dto.AuditFlowId && p.SolutionId == dto.SolutionId);
                    _structureBomInfoBakRepository.GetDbContext().Set<StructureBomInfoBak>().AddRange(listBak);
                    _structureBomInfoBakRepository.GetDbContext().SaveChanges();
                }
                else
                {
                    throw new FriendlyException(dto.SolutionId + ":该零件BOM没有上传!");
                }

                var bomInfoByProductIds = await _structureBomInfoRepository.GetAllListAsync(p => p.AuditFlowId == dto.AuditFlowId && p.SolutionId == dto.SolutionId);
                if (bomInfoByProductIds.Count == 0)
                {
                    foreach (var item in structureBomDtos)
                    {
                        StructureBomInfo structureBomInfo = new()
                        {
                            AuditFlowId = item.AuditFlowId,
                            ProductId = item.ProductId,
                            SolutionId = item.SolutionId,
                            SuperTypeName = item.SuperTypeName,
                            CategoryName = item.CategoryName,
                            TypeName = item.TypeName,
                            IsInvolveItem = item.IsInvolveItem,
                            SapItemNum = item.SapItemNum,
                            DrawingNumName = item.DrawingNumName,
                            AssemblyQuantity = item.AssemblyQuantity,
                            OverallDimensionSize = item.OverallDimensionSize,
                            MaterialName = item.MaterialName,
                            WeightNumber = item.WeightNumber,
                            MoldingProcess = item.MoldingProcess,
                            IsNewMouldProduct = item.IsNewMouldProduct,
                            SecondaryProcessingMethod = item.SecondaryProcessingMethod,
                            SurfaceTreatmentMethod = item.SurfaceTreatmentMethod,
                            DimensionalAccuracyRemark = item.DimensionalAccuracyRemark,
                        };
                        await _structureBomInfoRepository.InsertAsync(structureBomInfo);
                    }
                }

                #region 录入完成之后

                //为提交操作才执行插库、流转工作流操作
                if (dto.Opinion == FinanceConsts.Done)
                {
                    await _productIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = dto.AuditFlowId, SolutionId = dto.SolutionId, EnumSole = AuditFlowConsts.AF_StructBomImport });

                    List<NreIsSubmit> allProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(dto.AuditFlowId) && p.EnumSole.Equals(AuditFlowConsts.AF_StructBomImport));
                    //当前已保存的确认表中零件数目等于 核价需求导入时的零件数目
                    if (solutionId.Count == allProductIsSubmits.Count + 1)
                    {
                        //嵌入工作流
                        await _workflowInstanceAppService.SubmitNodeInterfece(new SubmitNodeInput
                        {
                            NodeInstanceId = dto.NodeInstanceId,
                            FinanceDictionaryDetailId = FinanceConsts.Done,//这个方法没有保存机制，把所以的输入都视作提交 dto.Opinion,
                            Comment = dto.Comment,
                        });
                    }
                }
                #endregion

            }
        }

        /// <summary>
        /// 结构BOM录入 退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task ClearStructBomImportState(long Id)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(AuditFlowConsts.AF_StructBomImport));
            foreach (NreIsSubmit item in productIsSubmits)
            {
                await _productIsSubmit.HardDeleteAsync(item);
            }
        }

        /// <summary>
        /// 更新结构Bom表和差异表数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task UpdateStructionBomDiffAndStructionBom(long auditFlowId)
        {
            //查询核价需求导入时的零件信息
            var solutionIds = await _solutionTableRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);

            //把备份表中数据插入到原表中
            List<StructureBomInfo> list;
            foreach (var solutionId in solutionIds)
            {
                //已经保存到备份bom表里的数据
                var bakList = await _structureBomInfoBakRepository.GetAllListAsync(p => p.AuditFlowId == solutionId.AuditFlowId && p.SolutionId == solutionId.Id);
                list = _objectMapper.Map<List<StructureBomInfo>>(bakList);

                var bomInfoByProductIds = await _structureBomInfoRepository.GetAllListAsync(p => p.AuditFlowId == solutionId.AuditFlowId && p.SolutionId == solutionId.Id);

                foreach (var item in list)
                {
                    if (bomInfoByProductIds.Count != 0)
                    {
                        //根据前面7项检查出来是否存在
                        var exsitBomInfos = await _structureBomInfoRepository.GetAllListAsync(p => p.AuditFlowId == solutionId.AuditFlowId && p.SolutionId == solutionId.Id && p.SuperTypeName == item.SuperTypeName
                                                                                    && p.CategoryName == item.CategoryName && p.TypeName == item.TypeName && p.IsInvolveItem == item.IsInvolveItem
                                                                                    && p.SapItemNum == item.SapItemNum && p.DrawingNumName == item.DrawingNumName);
                        //如果不存在，则是一个新增
                        if (exsitBomInfos.Count == 0)
                        {
                            StructureBomInfo structureBomInfo = new()
                            {
                                AuditFlowId = item.AuditFlowId,
                                ProductId = item.ProductId,
                                SolutionId = item.SolutionId,
                                SuperTypeName = item.SuperTypeName,
                                CategoryName = item.CategoryName,
                                TypeName = item.TypeName,
                                IsInvolveItem = item.IsInvolveItem,
                                SapItemNum = item.SapItemNum,
                                DrawingNumName = item.DrawingNumName,
                                AssemblyQuantity = item.AssemblyQuantity,
                                OverallDimensionSize = item.OverallDimensionSize,
                                MaterialName = item.MaterialName,
                                WeightNumber = item.WeightNumber,
                                MoldingProcess = item.MoldingProcess,
                                IsNewMouldProduct = item.IsNewMouldProduct,
                                SecondaryProcessingMethod = item.SecondaryProcessingMethod,
                                SurfaceTreatmentMethod = item.SurfaceTreatmentMethod,
                                DimensionalAccuracyRemark = item.DimensionalAccuracyRemark,
                            };
                            long itemId = await _structureBomInfoRepository.InsertAndGetIdAsync(structureBomInfo);
                            StructBomDifferent structBomDifferent = new()
                            {
                                AuditFlowId = item.AuditFlowId,
                                ProductId = item.ProductId,
                                SolutionId = item.SolutionId,
                                StructureId = itemId,
                                ModifyTypeValue = MODIFYTYPE.ADDNEWDATA,
                            };
                            await _structBomDifferentRepository.InsertAsync(structBomDifferent);
                        }
                        else
                        {
                            foreach (var bominfo in exsitBomInfos)
                            {
                                //如果前面7项相同，但后面8项有不同，则是一个修改
                                if (bominfo.AssemblyQuantity != item.AssemblyQuantity || bominfo.OverallDimensionSize != item.OverallDimensionSize || bominfo.MaterialName != item.MaterialName
                                    || bominfo.WeightNumber != item.WeightNumber || bominfo.MoldingProcess != item.MoldingProcess || bominfo.IsNewMouldProduct != item.IsNewMouldProduct
                                    || bominfo.SecondaryProcessingMethod != item.SecondaryProcessingMethod || bominfo.SurfaceTreatmentMethod != item.SurfaceTreatmentMethod || bominfo.DimensionalAccuracyRemark != item.DimensionalAccuracyRemark)
                                {
                                    bominfo.AssemblyQuantity = item.AssemblyQuantity;
                                    bominfo.OverallDimensionSize = item.OverallDimensionSize;
                                    bominfo.MaterialName = item.MaterialName;
                                    bominfo.WeightNumber = item.WeightNumber;
                                    bominfo.MoldingProcess = item.MoldingProcess;
                                    bominfo.IsNewMouldProduct = item.IsNewMouldProduct;
                                    bominfo.SecondaryProcessingMethod = item.SecondaryProcessingMethod;
                                    bominfo.SurfaceTreatmentMethod = item.SurfaceTreatmentMethod;
                                    bominfo.DimensionalAccuracyRemark = item.DimensionalAccuracyRemark;

                                    await _structureBomInfoRepository.UpdateAsync(bominfo);
                                    StructBomDifferent structBomDifferent = new()
                                    {
                                        AuditFlowId = bominfo.AuditFlowId,
                                        ProductId = bominfo.ProductId,
                                        SolutionId = item.SolutionId,
                                        StructureId = bominfo.Id,
                                        ModifyTypeValue = MODIFYTYPE.MODIFYNEWDATA,
                                    };
                                    await _structBomDifferentRepository.InsertAsync(structBomDifferent);
                                }
                            }
                        }
                    }
                    else
                    {
                        StructureBomInfo structureBomInfo = new()
                        {
                            AuditFlowId = item.AuditFlowId,
                            ProductId = item.ProductId,
                            SolutionId = item.SolutionId,
                            SuperTypeName = item.SuperTypeName,
                            CategoryName = item.CategoryName,
                            TypeName = item.TypeName,
                            IsInvolveItem = item.IsInvolveItem,
                            SapItemNum = item.SapItemNum,
                            DrawingNumName = item.DrawingNumName,
                            AssemblyQuantity = item.AssemblyQuantity,
                            OverallDimensionSize = item.OverallDimensionSize,
                            MaterialName = item.MaterialName,
                            WeightNumber = item.WeightNumber,
                            MoldingProcess = item.MoldingProcess,
                            IsNewMouldProduct = item.IsNewMouldProduct,
                            SecondaryProcessingMethod = item.SecondaryProcessingMethod,
                            SurfaceTreatmentMethod = item.SurfaceTreatmentMethod,
                            DimensionalAccuracyRemark = item.DimensionalAccuracyRemark,
                        };
                        await _structureBomInfoRepository.InsertAsync(structureBomInfo);
                    }
                }

                if (bomInfoByProductIds.Count != 0)
                {
                    //检查旧BOM表中是否有删除的记录
                    foreach (var bomInfoItem in bomInfoByProductIds)
                    {
                        var bomInfoItemMore = list.FirstOrDefault(p => p.SuperTypeName == bomInfoItem.SuperTypeName && p.CategoryName == bomInfoItem.CategoryName && p.TypeName == bomInfoItem.TypeName
                                                        && p.IsInvolveItem == bomInfoItem.IsInvolveItem && p.SapItemNum == bomInfoItem.SapItemNum && p.DrawingNumName == bomInfoItem.DrawingNumName);
                        if (bomInfoItemMore == null)
                        {
                            StructBomDifferent structBomDifferent = new()
                            {
                                AuditFlowId = bomInfoItem.AuditFlowId,
                                ProductId = bomInfoItem.ProductId,
                                SolutionId = bomInfoItem.SolutionId,
                                StructureId = bomInfoItem.Id,
                                ModifyTypeValue = MODIFYTYPE.DELNEWDATA,
                            };
                            await _structBomDifferentRepository.InsertAsync(structBomDifferent);

                            await _structureBomInfoRepository.HardDeleteAsync(bomInfoItem);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 清除结构Bom备份表和差异化表数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task ClearStructionBomBak(long auditFlowId)
        {
            await _structureBomInfoBakRepository.HardDeleteAsync(p => p.AuditFlowId == auditFlowId);
            await _structBomDifferentRepository.HardDeleteAsync(p => p.AuditFlowId == auditFlowId);
        }

        /// <summary>
        /// 清除结构Bom差异化表数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task ClearStructionBomDiff(long auditFlowId)
        {
            await _structBomDifferentRepository.HardDeleteAsync(p => p.AuditFlowId == auditFlowId);
        }

        /// <summary>
        ///清除结构Bom表数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task ClearStructionBom(long auditFlowId)
        {
            await _structureBomInfoRepository.HardDeleteAsync(p => p.AuditFlowId == auditFlowId);
        }

        /// <summary>
        /// 跳转函数
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        private async Task InterfaceJump(long flowId)
        {
            if (AbpSession.UserId is null)
            {
                throw new FriendlyException("请先登录");
            }

            await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
            {
                AuditFlowId = flowId,
                ProcessIdentifier = AuditFlowConsts.AF_StructBomImport,
                UserId = AbpSession.UserId.Value,
                Opinion = OPINIONTYPE.Submit_Agreee
            });
        }

        /// <summary>
        /// 最终提交后展示结构bom接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<StructureBomInfo>> FindStructureBomByProcess(ProductDevelopmentInputDto dto)
        {
            long AuditFlowId = dto.AuditFlowId;
            long ProductId = dto.ProductId;
            long SolutionId = dto.SolutionId;

            var dataBak = _structureBomInfoBakRepository.GetAll()
                .Where(p => AuditFlowId.Equals(p.AuditFlowId))
                .Where(p => SolutionId.Equals(p.SolutionId)).OrderBy(p => p.Id);

            List<StructureBomInfoBak> resultBak = await dataBak.ToListAsync();
            List<StructureBomInfo> result = _objectMapper.Map<List<StructureBomInfo>>(resultBak);
            if (result.Count == 0)
            {
                var data = _structureBomInfoRepository.GetAll()
                .Where(p => AuditFlowId.Equals(p.AuditFlowId))
                .Where(p => SolutionId.Equals(p.SolutionId)).OrderBy(p => p.Id);

                result = await data.ToListAsync();
            }

            return result;
        }


        ///// <summary>
        /////  
        ///// </summary>
        ///// <param name="FileName"></param>
        ///// <returns></returns>
        ///// <exception cref="UserFriendlyException"></exception>
        //public IActionResult PostProductDepartmentDownloadExcel(string FileName = "NRE产品部EMC电性能实验费模版下载")
        //{
        //    try
        //    {
        //        string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\NRE产品部EMC电性能实验费模版.xlsx";
        //        return new FileStreamResult(File.OpenRead(templatePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //        {
        //            FileDownloadName = $"{FileName}.xlsx"
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        throw new UserFriendlyException(e.Message);
        //    }
        //}

    }
}
