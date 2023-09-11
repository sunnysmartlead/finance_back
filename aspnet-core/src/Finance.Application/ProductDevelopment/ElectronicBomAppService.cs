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
using Finance.PriceEval.Dto;
using Finance.ProductDevelopment.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Finance.WorkFlows;
using Finance.WorkFlows.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Finance.ProductDevelopment
{
    /// <summary>
    /// 电子BOM录入接口类
    /// </summary>
    public class ElectronicBomAppService : FinanceAppServiceBase
    {
        private readonly ILogger<ElectronicBomAppService> _logger;
        private readonly IRepository<ElectronicBomInfo, long> _electronicBomInfoRepository;
        private readonly IRepository<ElectronicBomInfoBak, long> _electronicBomInfoBakRepository;
        private readonly IRepository<ModelCount, long> _modelCountRepository;
        private readonly IRepository<ElecBomDifferent, long> _elecBomDifferentRepository;
        private readonly IRepository<PriceEvaluation, long> _priceEvaluationRepository;
        private readonly IRepository<ProductInformation, long> _productInformationRepository;
        private readonly IRepository<BoardInfo, long> _boardInfoRepository;
        private readonly IRepository<Solution, long> _solutionTableRepository;
        /// <summary>
        ///  零件是否全部录入 依据实体类
        /// </summary>
        private readonly IRepository<NreIsSubmit, long> _productIsSubmit;



        /// <summary>
        /// 界面流转服务
        /// </summary>
        private readonly AuditFlowAppService _flowAppService;
        private readonly IObjectMapper _objectMapper;

        private readonly WorkflowInstanceAppService _workflowInstanceAppService;

        /// <summary>
        /// 营销部审核中方案表
        /// </summary>
        public readonly IRepository<Solution, long> _resourceSchemeTable;
        public ElectronicBomAppService(ILogger<ElectronicBomAppService> logger, IRepository<ElectronicBomInfo, long> electronicBomInfoRepository, IRepository<ElectronicBomInfoBak, long> electronicBomInfoBakRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<ElecBomDifferent, long> elecBomDifferentRepository, IRepository<PriceEvaluation, long> priceEvaluationRepository, IRepository<ProductInformation, long> productInformationRepository, IRepository<BoardInfo, long> boardInfoRepository, IRepository<Solution, long> solutionTableRepository, IRepository<NreIsSubmit, long> productIsSubmit, AuditFlowAppService flowAppService, IObjectMapper objectMapper, WorkflowInstanceAppService workflowInstanceAppService, IRepository<Solution, long> resourceSchemeTable)
        {
            _logger = logger;
            _electronicBomInfoRepository = electronicBomInfoRepository;
            _electronicBomInfoBakRepository = electronicBomInfoBakRepository;
            _modelCountRepository = modelCountRepository;
            _elecBomDifferentRepository = elecBomDifferentRepository;
            _priceEvaluationRepository = priceEvaluationRepository;
            _productInformationRepository = productInformationRepository;
            _boardInfoRepository = boardInfoRepository;
            _solutionTableRepository = solutionTableRepository;
            _productIsSubmit = productIsSubmit;
            _flowAppService = flowAppService;
            _objectMapper = objectMapper;
            _workflowInstanceAppService = workflowInstanceAppService;
            _resourceSchemeTable = resourceSchemeTable;
        }







        /// <summary>
        /// 上传后读取电子bom接口
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<ProductDevelopmentInputDto> UploadExcel(IFormFile file)
        {
            //打开上传文件的输入流
            Stream stream = file.OpenReadStream();

            //根据文件流创建excel数据结构
            IWorkbook workbook = WorkbookFactory.Create(stream);
            stream.Close();

            //尝试获取第一个sheet
            var sheet = workbook.GetSheetAt(0);

            List<ElectronicBomDto> list = new List<ElectronicBomDto>();
            ProductDevelopmentInputDto result = new ProductDevelopmentInputDto();

            //判断是否获取到 sheet
            if (sheet != null)
            {
                //最后一行的标号
                int lastRowNum = sheet.LastRowNum;

                //从第三行开始获取
                string daLei = "";
                for (int i = 2; i < lastRowNum + 1; i++)
                {
                    var row = sheet.GetRow(i);

                    ElectronicBomDto dto = new ElectronicBomDto();
                    for (int j = 0; j < 7; j++)
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
                                    daLei = row.GetCell(j).ToString();
                                }
                                else
                                {
                                    dto = DataToList(dto, row.GetCell(j), j);
                                }
                            }
                            catch
                            {
                                throw new FriendlyException("第" + (i + 1) + "行第" + (j + 1) + "列数据格式错误，请检查！");
                            }
                        }
                    }
                    dto.CategoryName = daLei;
                    list.Add(dto);
                }
            }

            result.ElectronicBomDtos = list;
            result.IsSuccess = true;
            return result;

        }

        /// <summary>
        /// 列数据转换成指定的表值
        /// </summary>
        /// <param name="bomInfo"></param>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static ElectronicBomDto DataToList(ElectronicBomDto bomInfo, object data, int index)
        {
            if (data == null) { return null; }

            if (index == 1)
            {
                bomInfo.TypeName = data.ToString();
            }
            else if (index == 2)
            {
                bomInfo.IsInvolveItem = data.ToString();
            }
            else if (index == 3)
            {
                bomInfo.SapItemNum = Regex.Replace(data.ToString(), @"\s", "");
            }
            else if (index == 4)
            {
                bomInfo.SapItemName = data.ToString();
            }
            else if (index == 5)
            {
                bomInfo.AssemblyQuantity = double.Parse(data.ToString());
            }
            else if (index == 6)
            {
                bomInfo.EncapsulationSize = data.ToString();
            }



            return bomInfo;
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

        public async Task SaveElectronicBom(ProductDevelopmentInputDto dto)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(dto.AuditFlowId) && p.SolutionId.Equals(dto.SolutionId) && p.EnumSole.Equals(AuditFlowConsts.AF_ElectronicBomImport));
            if (productIsSubmits.Count is not 0)
            {
                throw new FriendlyException(dto.SolutionId + ":该零件方案id已经提交过了");
            }
            else
            {
                //查询总方案
                List<SolutionModel> solutionId = await TotalSolution(dto.AuditFlowId);

                List<ElectronicBomDto> electronicBomDtos = dto.ElectronicBomDtos;
                long AuditFlowId = dto.AuditFlowId;
                long SolutionId = dto.SolutionId;
                long ProductId = dto.ProductId;
                electronicBomDtos.ForEach(bomInfo =>
                {
                    bomInfo.AuditFlowId = AuditFlowId;
                    bomInfo.SolutionId = SolutionId;
                    bomInfo.ProductId = ProductId;
                });

                //要保存的bom表list
                var listBak = _objectMapper.Map<List<ElectronicBomInfoBak>>(electronicBomDtos);
                if (listBak.Count > 0)
                {
                    await _electronicBomInfoBakRepository.HardDeleteAsync(p => p.AuditFlowId == dto.AuditFlowId && p.SolutionId == dto.SolutionId);
                    _electronicBomInfoBakRepository.GetDbContext().Set<ElectronicBomInfoBak>().AddRange(listBak);
                    _electronicBomInfoBakRepository.GetDbContext().SaveChanges();
                }
                else
                {
                    throw new FriendlyException(dto.SolutionId + ":该零件方案BOM没有上传!");
                }

                var bomInfoByProductIds = await _electronicBomInfoRepository.GetAllListAsync(p => p.AuditFlowId == dto.AuditFlowId && p.SolutionId == dto.SolutionId);
                if (bomInfoByProductIds.Count == 0)
                {
                    foreach (var item in electronicBomDtos)
                    {
                        ElectronicBomInfo electronicBomInfo = new()
                        {
                            AuditFlowId = item.AuditFlowId,
                            SolutionId = item.SolutionId,
                            ProductId = item.ProductId,
                            CategoryName = item.CategoryName,
                            TypeName = item.TypeName,
                            IsInvolveItem = item.IsInvolveItem,
                            SapItemName = item.SapItemName,
                            SapItemNum = item.SapItemNum,
                            AssemblyQuantity = item.AssemblyQuantity,
                            EncapsulationSize = item.EncapsulationSize,
                        };
                        await _electronicBomInfoRepository.InsertAsync(electronicBomInfo);
                    }
                }

                #region 录入完成之后
                await _productIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = dto.AuditFlowId, SolutionId = dto.SolutionId, EnumSole = AuditFlowConsts.AF_ElectronicBomImport });
                #endregion

                List<NreIsSubmit> allProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(dto.AuditFlowId) && p.EnumSole.Equals(AuditFlowConsts.AF_ElectronicBomImport));
                //当前已保存的bom表中零件数目等于 核价需求导入时的零件数目
                if (solutionId.Count <= allProductIsSubmits.Count + 1)
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
        }


        /// <summary>
        /// 接收前端拼版数据存入本地接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        public async Task SaveBoard(ProductDevelopmentInputDto dto)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(dto.AuditFlowId) && p.SolutionId.Equals(dto.SolutionId) && p.EnumSole.Equals(AuditFlowConsts.AF_ElectronicBomImport));
            if (productIsSubmits.Count is not 0)
            {
                throw new FriendlyException(dto.SolutionId + ":该零件方案id已经提交过了");
            }
            else
            {
                //查询核价需求导入时的零件信息
                var productIds = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == dto.AuditFlowId);

                List<BoardDto> boardDtos = dto.BoardDtos;
                long AuditFlowId = dto.AuditFlowId;
                long SolutionId = dto.SolutionId;
                long ProductId = dto.ProductId;
                boardDtos.ForEach(bomInfo =>
                {
                    bomInfo.AuditFlowId = AuditFlowId;
                    bomInfo.SolutionId = SolutionId;
                    bomInfo.ProductId = ProductId;
                });


                var boardInfoByProductIds = await _boardInfoRepository.GetAllListAsync(p => p.AuditFlowId == dto.AuditFlowId && p.SolutionId == dto.SolutionId);
                if (boardInfoByProductIds.Count == 0)
                {
                    foreach (var item in boardDtos)
                    {
                        BoardInfo boardInfo = new()
                        {
                            AuditFlowId = item.AuditFlowId,
                            SolutionId = item.SolutionId,
                            ProductId = item.ProductId,
                            BoardId = item.BoardId,
                            BoardName = item.BoardName,
                            BoardLenth = item.BoardLenth,
                            BoardWidth = item.BoardWidth,
                            BoardSquare = item.BoardSquare,
                            StoneQuantity = item.StoneQuantity,
                        };
                        await _boardInfoRepository.InsertAsync(boardInfo);
                    }
                }
            }
        }


        /// <summary>
        /// 电子BOM录入 退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task ClearElecBomImportState(long Id)
        {
            List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(AuditFlowConsts.AF_ElectronicBomImport));
            foreach (NreIsSubmit item in productIsSubmits)
            {
                await _productIsSubmit.HardDeleteAsync(item);
            }
        }

        /// <summary>
        /// 更新电子Bom表和差异表数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task UpdateElecBomDiffAndElecBom(long auditFlowId)
        {
            //查询核价需求导入时的零件信息
            var solutionIds = await _solutionTableRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);

            //把备份表中数据插入到原表中
            List<ElectronicBomInfo> list;
            foreach (var solutionId in solutionIds)
            {
                //已经保存到备份bom表里的数据
                var bakList = await _electronicBomInfoBakRepository.GetAllListAsync(p => p.AuditFlowId == solutionId.AuditFlowId && p.SolutionId == solutionId.Id);
                list = _objectMapper.Map<List<ElectronicBomInfo>>(bakList);

                var bomInfoByProductIds = await _electronicBomInfoRepository.GetAllListAsync(p => p.AuditFlowId == solutionId.AuditFlowId && p.SolutionId == solutionId.Id);

                foreach (var item in list)
                {
                    if (bomInfoByProductIds.Count != 0)
                    {
                        //根据前面7项检查出来是否存在
                        var exsitBomInfos = await _electronicBomInfoRepository.GetAllListAsync(p => p.AuditFlowId == solutionId.AuditFlowId && p.SolutionId == solutionId.Id && p.CategoryName == item.CategoryName && p.TypeName == item.TypeName
                                                                                    && p.IsInvolveItem == item.IsInvolveItem && p.SapItemName == item.SapItemName && p.SapItemNum == item.SapItemNum);
                        //如果不存在，则是一个新增
                        if (exsitBomInfos.Count == 0)
                        {
                            ElectronicBomInfo electronicBomInfo = new()
                            {
                                AuditFlowId = item.AuditFlowId,
                                ProductId = item.ProductId,
                                SolutionId = solutionId.Id,
                                CategoryName = item.CategoryName,
                                TypeName = item.TypeName,
                                IsInvolveItem = item.IsInvolveItem,
                                SapItemName = item.SapItemName,
                                SapItemNum = item.SapItemNum,
                                AssemblyQuantity = item.AssemblyQuantity,
                                EncapsulationSize = item.EncapsulationSize,
                            };
                            long itemId = await _electronicBomInfoRepository.InsertAndGetIdAsync(electronicBomInfo);
                            ElecBomDifferent elecBomDifferent = new()
                            {
                                AuditFlowId = item.AuditFlowId,
                                ProductId = item.ProductId,
                                SolutionId = solutionId.Id,
                                ElectronicId = itemId,
                                ModifyTypeValue = MODIFYTYPE.ADDNEWDATA,
                            };
                            await _elecBomDifferentRepository.InsertAsync(elecBomDifferent);
                        }
                        else
                        {
                            foreach (var bominfo in exsitBomInfos)
                            {
                                //如果前面7项相同，但后面两项有不同，则是一个修改
                                if (bominfo.AssemblyQuantity != item.AssemblyQuantity || bominfo.EncapsulationSize != item.EncapsulationSize)
                                {
                                    bominfo.AssemblyQuantity = item.AssemblyQuantity;
                                    bominfo.EncapsulationSize = item.EncapsulationSize;
                                    await _electronicBomInfoRepository.UpdateAsync(bominfo);
                                    ElecBomDifferent elecBomDifferent = new()
                                    {
                                        AuditFlowId = bominfo.AuditFlowId,
                                        ProductId = bominfo.ProductId,
                                        SolutionId = solutionId.Id,
                                        ElectronicId = bominfo.Id,
                                        ModifyTypeValue = MODIFYTYPE.MODIFYNEWDATA,
                                    };
                                    await _elecBomDifferentRepository.InsertAsync(elecBomDifferent);
                                }
                            }
                        }
                    }
                    else
                    {
                        ElectronicBomInfo electronicBomInfo = new()
                        {
                            AuditFlowId = item.AuditFlowId,
                            ProductId = item.ProductId,
                            SolutionId = item.SolutionId,
                            CategoryName = item.CategoryName,
                            TypeName = item.TypeName,
                            IsInvolveItem = item.IsInvolveItem,
                            SapItemName = item.SapItemName,
                            SapItemNum = item.SapItemNum,
                            AssemblyQuantity = item.AssemblyQuantity,
                            EncapsulationSize = item.EncapsulationSize,
                        };
                        await _electronicBomInfoRepository.InsertAsync(electronicBomInfo);
                    }
                }

                if (bomInfoByProductIds.Count != 0)
                {
                    //检查旧BOM表中是否有删除的记录
                    foreach (var bomInfoItem in bomInfoByProductIds)
                    {
                        var bomInfoItemMore = list.FirstOrDefault(p => p.CategoryName == bomInfoItem.CategoryName && p.TypeName == bomInfoItem.TypeName
                                                        && p.IsInvolveItem == bomInfoItem.IsInvolveItem && p.SapItemName == bomInfoItem.SapItemName && p.SapItemNum == bomInfoItem.SapItemNum);
                        if (bomInfoItemMore == null)
                        {
                            ElecBomDifferent elecBomDifferent = new()
                            {
                                AuditFlowId = bomInfoItem.AuditFlowId,
                                ProductId = bomInfoItem.ProductId,
                                SolutionId = bomInfoItem.SolutionId,
                                ElectronicId = bomInfoItem.Id,
                                ModifyTypeValue = MODIFYTYPE.DELNEWDATA,
                            };
                            await _elecBomDifferentRepository.InsertAsync(elecBomDifferent);

                            await _electronicBomInfoRepository.HardDeleteAsync(bomInfoItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清除电子Bom备份表和差异化表数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task ClearElectronicBomBak(long auditFlowId)
        {
            await _electronicBomInfoBakRepository.HardDeleteAsync(p => p.AuditFlowId == auditFlowId);
            await _elecBomDifferentRepository.HardDeleteAsync(p => p.AuditFlowId == auditFlowId);
        }

        /// <summary>
        /// 清除电子Bom差异化表数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task ClearElectronicBomDiff(long auditFlowId)
        {
            await _elecBomDifferentRepository.HardDeleteAsync(p => p.AuditFlowId == auditFlowId);
        }


        /// <summary>
        ///清除电子Bom表数据
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task ClearElectronicBom(long auditFlowId)
        {
            await _electronicBomInfoRepository.HardDeleteAsync(p => p.AuditFlowId == auditFlowId);
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
                ProcessIdentifier = AuditFlowConsts.AF_ElectronicBomImport,
                UserId = AbpSession.UserId.Value,
                Opinion = OPINIONTYPE.Submit_Agreee
            });
        }

        /// <summary>
        /// 最终提交后展示电子bom接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<ElectronicBomInfo>> FindElectronicBomByProcess(ProductDevelopmentInputDto dto)
        {
            long AuditFlowId = dto.AuditFlowId;
            long SolutionId = dto.SolutionId;
            var dataBak = _electronicBomInfoBakRepository.GetAll()
                .Where(p => AuditFlowId.Equals(p.AuditFlowId))
                .Where(p => SolutionId.Equals(p.SolutionId)).OrderBy(p => p.Id);

            List<ElectronicBomInfoBak> resultBak = await dataBak.ToListAsync();
            List<ElectronicBomInfo> result = _objectMapper.Map<List<ElectronicBomInfo>>(resultBak);
            if (result.Count == 0)
            {
                var data = _electronicBomInfoRepository.GetAll()
                .Where(p => AuditFlowId.Equals(p.AuditFlowId))
                .Where(p => SolutionId.Equals(p.SolutionId)).OrderBy(p => p.Id);

                result = await data.ToListAsync();
            }

            return result;
        }

        /// <summary>
        /// 营销部录入界面中客户供应/指定的情况
        /// </summary>
        public async Task<List<CreateColumnFormatProductInformationDto>> GetProductionControl(long auditFlowId)
        {
            var productInformation = await _productInformationRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            List<CreateColumnFormatProductInformationDto> productInformationDto = ObjectMapper.Map<List<CreateColumnFormatProductInformationDto>>(productInformation);
            return productInformationDto;
        }


        /// <summary>
        /// 根据流程号方案号查询拼版列表接口
        /// </summary>
        public async Task<List<BoardDto>> GetBoardInfomation(long auditFlowId, long SolutionId)
        {
            var boardtInformation = await _boardInfoRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId && p.SolutionId == SolutionId);
            List<BoardDto> boardDto = ObjectMapper.Map<List<BoardDto>>(boardtInformation);
            return boardDto;
        }

    }
}
