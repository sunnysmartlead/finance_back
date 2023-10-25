using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Finance.Audit;
using Finance.BaseLibrary;
using Finance.DemandApplyAudit;
using Finance.Entering;
using Finance.FinanceMaintain;
using Finance.Infrastructure;
using Finance.Infrastructure.Dto;
using Finance.PriceEval;
using Finance.PriceEval.Dto;
using Finance.PropertyDepartment.Entering.Model;
using Finance.TradeCompliance.Dto;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.TradeCompliance
{
    /// <summary>
    /// 贸易合规后端API接口类
    /// </summary>
    public class TradeComplianceAppService : ApplicationService
    {
        private readonly IRepository<TradeComplianceCheck, long> _tradeComplianceCheckRepository;
        private readonly IRepository<ProductMaterialInfo, long> _productMaterialInfoRepository;
        private readonly IRepository<EnteringElectronic, long> _enteringElectronicRepository;
        private readonly IRepository<StructureElectronic, long> _structureElectronicRepository;
        private readonly IRepository<PriceEvaluation, long> _priceEvalRepository;
        private readonly IRepository<ModelCount, long> _modelCountRepository;
        private readonly IRepository<ModelCountYear, long> _modelCountYearRepository;
        private readonly IRepository<FinanceDictionaryDetail, string> _financeDictionaryDetailRepository;
        private readonly PriceEvaluationGetAppService _priceEvaluationGetAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly IRepository<Solution, long> _solutionTableRepository;
        private readonly IRepository<CountryLibrary, long> _countryLibraryRepository;
        private readonly IRepository<FoundationLogs, long> _foundationLogsRepository;

        private readonly IRepository<Gradient, long> _gradient;

        public TradeComplianceAppService(IRepository<TradeComplianceCheck, long> tradeComplianceCheckRepository, IRepository<ProductMaterialInfo, long> productMaterialInfoRepository, IRepository<EnteringElectronic, long> enteringElectronicRepository, IRepository<StructureElectronic, long> structureElectronicRepository, IRepository<PriceEvaluation, long> priceEvalRepository, IRepository<ModelCount, long> modelCountRepository, IRepository<ModelCountYear, long> modelCountYearRepository, IRepository<FinanceDictionaryDetail, string> financeDictionaryDetailRepository, PriceEvaluationGetAppService priceEvaluationGetAppService, IObjectMapper objectMapper, IRepository<Solution, long> solutionTableRepository, IRepository<CountryLibrary, long> countryLibraryRepository, IRepository<FoundationLogs, long> foundationLogsRepository, IRepository<Gradient, long> gradient)
        {
            _tradeComplianceCheckRepository = tradeComplianceCheckRepository;
            _productMaterialInfoRepository = productMaterialInfoRepository;
            _enteringElectronicRepository = enteringElectronicRepository;
            _structureElectronicRepository = structureElectronicRepository;
            _priceEvalRepository = priceEvalRepository;
            _modelCountRepository = modelCountRepository;
            _modelCountYearRepository = modelCountYearRepository;
            _financeDictionaryDetailRepository = financeDictionaryDetailRepository;
            _priceEvaluationGetAppService = priceEvaluationGetAppService;
            _objectMapper = objectMapper;
            _solutionTableRepository = solutionTableRepository;
            _countryLibraryRepository = countryLibraryRepository;
            _foundationLogsRepository = foundationLogsRepository;
            _gradient = gradient;
        }





        /// <summary>
        /// 获取 第一个页面最初的年份
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        private async Task<(int, YearType)> GetFristSopYear(long auditFlowId)
        {
            //List<ModelCountYear> modelCountYears = await _modelCountYearRepository.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            //List<int> yearList = modelCountYears.Select(p => p.Year).Distinct().ToList();
            //int sopYear = yearList.Min();
            //return sopYear;

            var list = await _modelCountYearRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            var sopYear = list.MinBy(p => p.Year);
            return sopYear.UpDown == YearType.Year ? (sopYear.Year, YearType.Year) : (sopYear.Year, YearType.FirstHalf);
        }

        /// <summary>
        ///最小的梯度
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        internal async Task<long> GetMinGradient(long auditFlowId)
        {
            List<Gradient> gradients = await _gradient.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId));
            List<long> gradientList = gradients.Select(p => p.Id).Distinct().ToList();
            long GradientId = gradientList.Min();
            return GradientId;
        }

        /// <summary>
        /// 获取贸易合规界面数据(计算得出)
        /// </summary>
        public virtual async Task<TradeComplianceCheckDto> GetTradeComplianceCheckByCalc(TradeComplianceInputDto input)
        {
            TradeComplianceCheckDto tradeComplianceCheckDto = new();

            //var productInfos = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.Id == input.ProductId);
            var solutionInfos = await _solutionTableRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.Id == input.SolutionId);
            if (solutionInfos.Count > 0)
            {
                var tradeComplianceList = await _tradeComplianceCheckRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.SolutionId == input.SolutionId);
                if (tradeComplianceList.Count > 0)
                {
                    tradeComplianceCheckDto.TradeComplianceCheck = tradeComplianceList.FirstOrDefault();
                }
                else
                {
                    tradeComplianceCheckDto.TradeComplianceCheck = new();
                }
                tradeComplianceCheckDto.TradeComplianceCheck.AuditFlowId = input.AuditFlowId;
                tradeComplianceCheckDto.TradeComplianceCheck.SolutionId = input.SolutionId;
                tradeComplianceCheckDto.TradeComplianceCheck.ProductName = solutionInfos.FirstOrDefault().Product;

                //取产品大类
                var productInfos = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.Id == solutionInfos.FirstOrDefault().Productld);
                tradeComplianceCheckDto.TradeComplianceCheck.ProductType = _financeDictionaryDetailRepository.FirstOrDefault(p => p.Id == productInfos.FirstOrDefault().ProductType).DisplayName;

                //取最终出口地国家
                var countryList = (from a in await _priceEvalRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId)
                                   join b in await _countryLibraryRepository.GetAllListAsync() on a.CountryLibraryId equals b.Id
                                   select b.Country).ToList();

                tradeComplianceCheckDto.TradeComplianceCheck.Country = countryList.Count > 0 ? countryList.FirstOrDefault() : null;




                //存入部分合规信息生成ID
                var tradeComplianceCheckId = await _tradeComplianceCheckRepository.InsertOrUpdateAndGetIdAsync(tradeComplianceCheckDto.TradeComplianceCheck);

                decimal sumEccns = 0;
                decimal sumPending = 0;
                var sopYear = await GetFristSopYear(input.AuditFlowId);
                GetPriceEvaluationTableInput priceTableByPart = new()
                {
                    AuditFlowId = input.AuditFlowId,
                    SolutionId = input.SolutionId,
                    InputCount = 0,
                    Year = sopYear.Item1,
                    GradientId = await GetMinGradient(input.AuditFlowId),
                    UpDown = sopYear.Item2
                };
                var priceTable = await _priceEvaluationGetAppService.GetPriceEvaluationTable(priceTableByPart);//取核价表数据
                tradeComplianceCheckDto.TradeComplianceCheck.ProductFairValue = priceTable.TotalCost * 1.1m;//产品公允价=核价表成本合计*1.1


                var countryIdList = (from a in await _priceEvalRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId)
                                   join b in await _countryLibraryRepository.GetAllListAsync() on a.CountryLibraryId equals b.Id
                                   select b.Id).ToList();
                tradeComplianceCheckDto.TradeComplianceCheck.CountryLibraryId = countryIdList.Count > 0 ? countryIdList.FirstOrDefault() : 0;

                var countries = await _countryLibraryRepository.GetAllListAsync(p => p.Id.Equals(tradeComplianceCheckDto.TradeComplianceCheck.CountryLibraryId));
                decimal rate = 0;//取国家库的比例

                if (countries.Count > 0)
                {
                    rate = countries.FirstOrDefault().Rate;
                }
                //取产品组成物料
                tradeComplianceCheckDto.ProductMaterialInfos = new();
                foreach (var material in priceTable.Material)
                {
                    ProductMaterialInfo materialinfo;
                    var materialList = await _productMaterialInfoRepository.GetAllListAsync(p => p.TradeComplianceCheckId == tradeComplianceCheckId && p.MaterialCode == material.Sap && p.MaterialName == material.TypeName);
                    if (materialList.Count > 0)
                    {
                        materialinfo = materialList.FirstOrDefault();
                    }
                    else
                    {
                        materialinfo = new();
                        materialinfo.AuditFlowId = input.AuditFlowId;
                        materialinfo.TradeComplianceCheckId = tradeComplianceCheckId;

                        materialinfo.MaterialCode = material.Sap;
                        materialinfo.MaterialName = material.TypeName;
                    }
                    materialinfo.MaterialIdInBom = material.Id;
                    materialinfo.MaterialDetailName = material.MaterialName;

                    materialinfo.Count = material.AssemblyCount;
                    materialinfo.UnitPrice = material.MaterialPriceCyn;
                    materialinfo.Amount = material.TotalMoneyCyn;

                    tradeComplianceCheckDto.ProductMaterialInfos.Add(materialinfo);

                    string MaterialIdPrefix = materialinfo.MaterialIdInBom[..1];
                    if (MaterialIdPrefix.Equals(PriceEvaluationGetAppService.ElectronicBomName))
                    {
                        long elecId = long.Parse(materialinfo.MaterialIdInBom.Remove(0, 1));
                        //查询是否涉及
                        EnteringElectronic enteringElectronic = await _enteringElectronicRepository.FirstOrDefaultAsync(p => p.ElectronicId.Equals(elecId) && p.AuditFlowId == input.AuditFlowId && p.SolutionId == input.SolutionId);
                        materialinfo.ControlStateType = enteringElectronic.MaterialControlStatus;
                    }
                    else
                    {
                        long structId = long.Parse(materialinfo.MaterialIdInBom.Remove(0, 1));
                        StructureElectronic structureElectronic = await _structureElectronicRepository.FirstOrDefaultAsync(p => p.StructureId.Equals(structId) && p.AuditFlowId == input.AuditFlowId && p.SolutionId == input.SolutionId);
                        materialinfo.ControlStateType = structureElectronic.MaterialControlStatus;
                    }



                    if (materialinfo.ControlStateType == FinanceConsts.EccnCode_Eccn || (rate == (decimal)0.1 && materialinfo.ControlStateType == FinanceConsts.EccnCode_Ear99))
                    {
                        sumEccns += materialinfo.Amount;
                    }
                    if (materialinfo.ControlStateType == FinanceConsts.EccnCode_Pending)
                    {
                        sumPending += materialinfo.Amount;
                    }
                    await _productMaterialInfoRepository.InsertOrUpdateAsync(materialinfo);
                }
                tradeComplianceCheckDto.TradeComplianceCheck.EccnPricePercent = sumEccns / tradeComplianceCheckDto.TradeComplianceCheck.ProductFairValue;
                tradeComplianceCheckDto.TradeComplianceCheck.PendingPricePercent = sumPending / tradeComplianceCheckDto.TradeComplianceCheck.ProductFairValue;
                tradeComplianceCheckDto.TradeComplianceCheck.AmountPricePercent = tradeComplianceCheckDto.TradeComplianceCheck.EccnPricePercent + tradeComplianceCheckDto.TradeComplianceCheck.PendingPricePercent;


                if (tradeComplianceCheckDto.TradeComplianceCheck.AmountPricePercent > rate)
                {
                    tradeComplianceCheckDto.TradeComplianceCheck.AnalysisConclusion = GeneralDefinition.TRADE_COMPLIANCE_NOT_OK;
                }
                else
                {
                    tradeComplianceCheckDto.TradeComplianceCheck.AnalysisConclusion = GeneralDefinition.TRADE_COMPLIANCE_OK;
                }
                await _tradeComplianceCheckRepository.InsertOrUpdateAsync(tradeComplianceCheckDto.TradeComplianceCheck);
            }
            else
            {
                throw new FriendlyException("获取零件信息失败，请检查零件信息");
            }
            return tradeComplianceCheckDto;
        }

        public virtual async Task<bool> IsProductsTradeComplianceOK(long flowId)
        {
            bool isOk = true;
            var solutionIdList = await _solutionTableRepository.GetAllListAsync(p => p.AuditFlowId == flowId);
            foreach (var solution in solutionIdList)
            {
                TradeComplianceInputDto tradeComplianceInput = new()
                {
                    AuditFlowId = flowId,
                    SolutionId = solution.Id
                };
                var tradeComplianceCheck = await GetTradeComplianceCheckByCalc(tradeComplianceInput);
                if (tradeComplianceCheck.TradeComplianceCheck.AnalysisConclusion == GeneralDefinition.TRADE_COMPLIANCE_NOT_OK)
                {
                    isOk = false;
                }
            }
            return isOk;
        }

        /// <summary>
        /// 获取贸易合规界面数据(从数据库获取)
        /// </summary>
        public virtual async Task<TradeComplianceCheckDto> GetTradeComplianceCheckFromDateBase(TradeComplianceInputDto input)
        {
            var tradeComplianceCheckList = await _tradeComplianceCheckRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.SolutionId == input.SolutionId);
            if (tradeComplianceCheckList.Count > 0)
            {
                TradeComplianceCheckDto tradeComplianceCheckDto = new();
                tradeComplianceCheckDto.TradeComplianceCheck = tradeComplianceCheckList.FirstOrDefault();
                tradeComplianceCheckDto.ProductMaterialInfos = new();
                var productMaterialList = await _productMaterialInfoRepository.GetAllListAsync(p => p.AuditFlowId == input.AuditFlowId && p.TradeComplianceCheckId == tradeComplianceCheckDto.TradeComplianceCheck.Id);
                if (productMaterialList.Count == 0)
                {
                    throw new FriendlyException("产品组成物料信息表未查询到对应方案信息，请检查！");
                }
                else 
                {
                    foreach (var productMaterial in productMaterialList)
                    {
                        tradeComplianceCheckDto.ProductMaterialInfos.Add(productMaterial);
                    }
                    return tradeComplianceCheckDto;
                }
                
            }
            else
            {
                throw new FriendlyException("贸易合规数据未正式进库，请检查信息！");

            }
        }

        /// <summary>
        /// 是否贸易合规，合规返回true，不合规返回false
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> GetIsTradeCompliance(long auditFlowId)
        {
            var tradeComplianceCheckList = await _tradeComplianceCheckRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            foreach (var tradeComplianceCheck in tradeComplianceCheckList)
            {
                if (tradeComplianceCheck.AnalysisConclusion.Equals(GeneralDefinition.TRADE_COMPLIANCE_NOT_OK))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  贸易合规EXCLE下载
        /// </summary>
        /// <param name="laboratoryFeeModels"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task<FileResult> PostExportOfTradeForm(TradeComplianceInputDto input)
        {
     
            var TradeTable = await GetTradeComplianceCheckByCalc(input);

            //创建Workbook
            XSSFWorkbook workbook = new XSSFWorkbook();
            //创建一个sheet
            workbook.CreateSheet("sheet1");


            ISheet sheet = workbook.GetSheetAt(0);//获取sheet

            //创建头部
            IRow row000 = sheet.CreateRow(0);
            row000.CreateCell(0).SetCellValue("");
            row000.CreateCell(1).SetCellValue("");
            row000.CreateCell(2).SetCellValue("");
            row000.CreateCell(3).SetCellValue("");
            row000.CreateCell(4).SetCellValue("");
            row000.CreateCell(5).SetCellValue("");
            row000.CreateCell(6).SetCellValue("");
            row000.CreateCell(7).SetCellValue("表单编号：");
            row000.CreateCell(8).SetCellValue("没有");

            IRow row001 = sheet.CreateRow(1);
            row001.CreateCell(0).SetCellValue("产品识别分析表（由系统自动生成）");

            IRow row002 = sheet.CreateRow(2);
            row002.CreateCell(0).SetCellValue("产品名称：");
            row002.CreateCell(2).SetCellValue(TradeTable.TradeComplianceCheck.ProductName);
            row002.CreateCell(6).SetCellValue("最终出口地国家：");
            row002.CreateCell(7).SetCellValue(TradeTable.TradeComplianceCheck.Country);

            IRow row003 = sheet.CreateRow(3);
            row003.CreateCell(0).SetCellValue("产品小类：");
            row003.CreateCell(2).SetCellValue(TradeTable.TradeComplianceCheck.ProductType);
            row003.CreateCell(6).SetCellValue("产品公允售价 ：");
            row003.CreateCell(7).SetCellValue(TradeTable.TradeComplianceCheck.ProductFairValue.ToString());

            IRow row004 = sheet.CreateRow(4);
            row004.CreateCell(0).SetCellValue("产品组成物料");
            //合并单元格
            sheet.AddMergedRegion(new CellRangeAddress( 4, 4+ TradeTable.ProductMaterialInfos.Count+3, 0, 0));

            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 8));
            sheet.AddMergedRegion(new CellRangeAddress(2, 2, 7, 8));
            sheet.AddMergedRegion(new CellRangeAddress(3, 3, 7, 8));


            row004.CreateCell(1).SetCellValue("序号");
            row004.CreateCell(2).SetCellValue("物料编码");
            row004.CreateCell(3).SetCellValue("物料种类");
            row004.CreateCell(4).SetCellValue("材料名称");
            row004.CreateCell(5).SetCellValue("装配数量");
            row004.CreateCell(6).SetCellValue("单价");
            row004.CreateCell(7).SetCellValue("金额");
            row004.CreateCell(8).SetCellValue("物料管制状态分类");


            for (int n = 0; n < TradeTable.ProductMaterialInfos.Count; n++)
            {
                IRow row00n = sheet.CreateRow(4+1+n);

                row00n.CreateCell(1).SetCellValue(TradeTable.ProductMaterialInfos[n].MaterialIdInBom);
                row00n.CreateCell(2).SetCellValue(TradeTable.ProductMaterialInfos[n].MaterialCode);
                row00n.CreateCell(3).SetCellValue(TradeTable.ProductMaterialInfos[n].MaterialName);
                row00n.CreateCell(4).SetCellValue(TradeTable.ProductMaterialInfos[n].MaterialDetailName);
                row00n.CreateCell(5).SetCellValue(TradeTable.ProductMaterialInfos[n].Count);
                row00n.CreateCell(6).SetCellValue(TradeTable.ProductMaterialInfos[n].UnitPrice.ToString());
                row00n.CreateCell(7).SetCellValue(TradeTable.ProductMaterialInfos[n].Amount.ToString());
                row00n.CreateCell(8).SetCellValue(TradeTable.ProductMaterialInfos[n].ControlStateType);

            }

            IRow rowAfterN1= sheet.CreateRow(4 + TradeTable.ProductMaterialInfos.Count + 1);
            rowAfterN1.CreateCell(1).SetCellValue("ECCN成分价值占比");
            sheet.AddMergedRegion(new CellRangeAddress(4 + TradeTable.ProductMaterialInfos.Count + 1, 4 + TradeTable.ProductMaterialInfos.Count + 1, 1, 4));
            sheet.AddMergedRegion(new CellRangeAddress(4 + TradeTable.ProductMaterialInfos.Count + 1, 4 + TradeTable.ProductMaterialInfos.Count + 1, 5, 8));
            rowAfterN1.CreateCell(5).SetCellValue(TradeTable.TradeComplianceCheck.EccnPricePercent.ToString());

            IRow rowAfterN2 = sheet.CreateRow(4 + TradeTable.ProductMaterialInfos.Count + 2);
            rowAfterN2.CreateCell(1).SetCellValue("待定成分价值占比");
            sheet.AddMergedRegion(new CellRangeAddress(4 + TradeTable.ProductMaterialInfos.Count + 2, 4 + TradeTable.ProductMaterialInfos.Count + 2, 1, 4));
            sheet.AddMergedRegion(new CellRangeAddress(4 + TradeTable.ProductMaterialInfos.Count + 2, 4 + TradeTable.ProductMaterialInfos.Count + 2, 5, 8));
            rowAfterN2.CreateCell(5).SetCellValue(TradeTable.TradeComplianceCheck.PendingPricePercent.ToString());

            IRow rowAfterN3 = sheet.CreateRow(4 + TradeTable.ProductMaterialInfos.Count + 3);
            rowAfterN3.CreateCell(1).SetCellValue("合计价值占比");
            sheet.AddMergedRegion(new CellRangeAddress(4 + TradeTable.ProductMaterialInfos.Count + 3, 4 + TradeTable.ProductMaterialInfos.Count + 3, 1, 4));
            sheet.AddMergedRegion(new CellRangeAddress(4 + TradeTable.ProductMaterialInfos.Count + 3, 4 + TradeTable.ProductMaterialInfos.Count + 3, 5, 8));
            rowAfterN3.CreateCell(5).SetCellValue(TradeTable.TradeComplianceCheck.AmountPricePercent.ToString());

            IRow rowAfterN4 = sheet.CreateRow(4 + TradeTable.ProductMaterialInfos.Count + 4);
            rowAfterN4.CreateCell(0).SetCellValue("分析结论");
            sheet.AddMergedRegion(new CellRangeAddress(4 + TradeTable.ProductMaterialInfos.Count + 4, 4 + TradeTable.ProductMaterialInfos.Count + 4, 1, 8));
            rowAfterN4.CreateCell(1).SetCellValue(TradeTable.TradeComplianceCheck.AnalysisConclusion.ToString());

            IRow rowAfterN5 = sheet.CreateRow(4 + TradeTable.ProductMaterialInfos.Count + 5);
            rowAfterN5.CreateCell(0).SetCellValue("做成/日期");
            sheet.AddMergedRegion(new CellRangeAddress(4 + TradeTable.ProductMaterialInfos.Count + 5, 4 + TradeTable.ProductMaterialInfos.Count + 5, 1, 8));
            rowAfterN5.CreateCell(1).SetCellValue(TradeTable.TradeComplianceCheck.CreationTime.ToString());

            //创建  占比  样式和列宽度
            XSSFCellStyle zhanbiStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            zhanbiStyle.Alignment = HorizontalAlignment.Center; // 居中
            sheet.GetRow(4 + TradeTable.ProductMaterialInfos.Count + 1).GetCell(1).CellStyle = zhanbiStyle;
            sheet.GetRow(4 + TradeTable.ProductMaterialInfos.Count + 2).GetCell(1).CellStyle = zhanbiStyle;
            sheet.GetRow(4 + TradeTable.ProductMaterialInfos.Count + 3).GetCell(1).CellStyle = zhanbiStyle;





            //创建头部样式和列宽度
            XSSFCellStyle titleStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            titleStyle.Alignment = HorizontalAlignment.Center; // 居中
            IFont titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            titleFont.FontHeightInPoints = 12;
            titleFont.Color = HSSFColor.Black.Index;//设置字体颜色
            titleStyle.SetFont(titleFont);
            sheet.GetRow(1).GetCell(0).CellStyle = titleStyle;

            //边框
            //XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            //cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            //cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            //cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            //cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

            //for (int i =0; i< 4 + TradeTable.ProductMaterialInfos.Count + 5;i++) {
            //    for (int j = 0;j < 8; j++)
            //    {
            //        sheet.GetRow(i).GetCell(j).CellStyle = cellStyle;
            //    }
            //}


            //列宽
            sheet.SetColumnWidth(0, 4000);
            sheet.SetColumnWidth(1, 5000);
            sheet.SetColumnWidth(2, 5500);
            sheet.SetColumnWidth(3, 6000);
            sheet.SetColumnWidth(4, 12000);
            sheet.SetColumnWidth(5, 2000);
            sheet.SetColumnWidth(6, 4000);
            sheet.SetColumnWidth(7, 5000);
            sheet.SetColumnWidth(8, 5000);


            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);

            Byte[] btye2 = ms.ToArray();
            FileContentResult fileContent = new FileContentResult(btye2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = "aaa.xlsx" };

            return fileContent;


        }

    }

}
