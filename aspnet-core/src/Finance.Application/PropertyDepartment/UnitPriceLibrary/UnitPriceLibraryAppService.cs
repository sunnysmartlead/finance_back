using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper;
using Finance.BaseLibrary;
using Finance.Dto;
using Finance.EngineeringDepartment;
using Finance.EntityFrameworkCore;
using Finance.Ext;
using Finance.FinanceMaintain;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Utilities;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.PropertyDepartment.UnitPriceLibrary
{
    /// <summary>
    /// 
    /// </summary>  
    public class UnitPriceLibraryAppService : ApplicationService, IUnitPriceLibraryAppService
    {
        /// <summary>
        /// 日志类型-毛利率
        /// </summary>
        private readonly LogType GrossProfitMarginType = LogType.GrossProfitMargin;
        /// <summary>
        /// 日志类型-汇率
        /// </summary>
        private readonly LogType ExchangeRateType = LogType.ExchangeRate;
        /// <summary>
        /// 日志类型-损耗率
        /// </summary>
        private readonly LogType LossRateType = LogType.LossRate;
        /// <summary>
        /// 日志类型-质量成本比例
        /// </summary>
        private readonly LogType QualityCostRatioType = LogType.QualityCostRatio;
        /// <summary>
        /// 基础单价库实体类
        /// </summary>
        private readonly IRepository<UInitPriceForm, long> _configUInitPriceForm;
        /// <summary>
        ///  财务维护 毛利率方案
        /// </summary>
        private readonly IRepository<GrossMarginForm, long> _configGrossMarginForm;
        /// <summary>
        ///  财务维护 汇率表
        /// </summary>
        private readonly IRepository<ExchangeRate, long> _configExchangeRate;
        /// <summary>
        /// 数据上下文提供者
        /// </summary>
        private readonly IDbContextProvider<FinanceDbContext> _provider;
        /// <summary>
        /// 共用物料库
        /// </summary>
        private readonly IRepository<SharedMaterialWarehouse, long> _sharedMaterialWarehouse;
        /// <summary>
        /// 基础库--日志表
        /// </summary>
        private readonly IRepository<FoundationLogs, long> _foundationLogs;
        /// <summary>
        /// 损耗率表
        /// </summary>
        private readonly IRepository<LossRateInfo, long> _lossRateInfo;
        /// <summary>
        /// 损耗率年份
        /// </summary>
        private readonly IRepository<LossRateYearInfo, long> _lossRateYearInfo;
        /// <summary>
        /// 质量成本比例
        /// </summary>
        private readonly IRepository<QualityCostRatio, long> _qualityCostRatio;
        /// <summary>
        /// 质量成本比例年份
        /// </summary>
        private readonly IRepository<QualityCostRatioYear, long> _qualityCostRatioYear;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configUInitPriceForm"></param>
        /// <param name="configGrossMarginForm"></param>
        /// <param name="configExchangeRate"></param>
        /// <param name="provider"></param>
        /// <param name="sharedMaterialWarehouse"></param>
        /// <param name="foundationLogs"></param>
        /// <param name="lossRateInfo"></param>
        /// <param name="lossRateYearInfo"></param>
        /// <param name="qualityCostRatio"></param>
        /// <param name="qualityCostRatioYear"></param>
        public UnitPriceLibraryAppService(IRepository<UInitPriceForm, long> configUInitPriceForm,
            IRepository<GrossMarginForm, long> configGrossMarginForm,
            IRepository<ExchangeRate, long> configExchangeRate,
            IDbContextProvider<FinanceDbContext> provider,
            IRepository<SharedMaterialWarehouse, long> sharedMaterialWarehouse,
            IRepository<FoundationLogs, long> foundationLogs,
            IRepository<LossRateInfo, long> lossRateInfo,
            IRepository<LossRateYearInfo, long> lossRateYearInfo,
            IRepository<QualityCostRatio, long> qualityCostRatio,
            IRepository<QualityCostRatioYear, long> qualityCostRatioYear)
        {
            _configUInitPriceForm = configUInitPriceForm;
            _configGrossMarginForm = configGrossMarginForm;
            _configExchangeRate = configExchangeRate;
            _provider = provider;
            _sharedMaterialWarehouse = sharedMaterialWarehouse;
            _foundationLogs = foundationLogs;
            _lossRateInfo = lossRateInfo;
            _lossRateYearInfo = lossRateYearInfo;
            _qualityCostRatio = qualityCostRatio;
            _qualityCostRatioYear = qualityCostRatioYear;
        }
        /// <summary>
        /// 查询单价库信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<UInitPriceFormModel>> GetGainUInitPriceForm(QueryInteractionClass input)
        {
            try
            {
                //定义列表查询
                IQueryable<UInitPriceForm> filter = _configUInitPriceForm.GetAll().WhereIf(!input.Filter.IsNullOrEmpty(), p => p.PriceMasterDataNumber.Contains(input.Filter) || p.MaterialCode.Contains(input.Filter));
                IQueryable<UInitPriceForm> pagedSorted = filter.OrderByDescending(p => p.Id).PageBy(input);
                //获取总数
                var count = await filter.CountAsync();
                //获取查询结果
                List<UInitPriceForm> result = await pagedSorted.ToListAsync();
                return new PagedResultDto<UInitPriceFormModel>(count, ObjectMapper.Map<List<UInitPriceFormModel>>(result));
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 添加 (刷新整个基础单价库)
        /// </summary>
        /// <param Name="stationBind"></param>
        /// <returns></returns>   
        public async Task PostUInitPriceForm(IFormFile file)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    string[] suffix = file.FileName.Split(".");
                    if (suffix.Length < 1)
                    {
                        throw new FriendlyException("文件名错误");
                    }
                    await file.CopyToAsync(memoryStream);
                    //载入xls文档
                    Workbook workbook = new Workbook();
                    if (suffix[suffix.Length - 1].Equals("xls"))
                    {
                        string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\基础单价库.xlsx";
                        workbook.LoadFromStream(memoryStream);
                        //保存为xlsx格式
                        workbook.SaveToFile(templatePath, ExcelVersion.Version2016);
                        using (FileStream fs = File.OpenRead(templatePath))
                        {
                            memoryStream.SetLength(0);
                            await fs.CopyToAsync(memoryStream);
                        }
                    }
                    if (memoryStream.Length < 512 * 1048576)
                    {

                        List<UInitPriceFormModel> rows = memoryStream.Query<UInitPriceFormModel>(startCell: "A3").ToList();
                        #region 校验数据
                        //供应商优先级
                        IEnumerable<string> supplierPriority = rows.Where(p => p.SupplierPriority.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 供应商优先级为空。");
                        if (supplierPriority.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", supplierPriority)}");
                        }
                        //校验供应商ERP编码
                        IEnumerable<string> supplierErpCode = rows.Where(p => p.SupplierErpCode.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 供应商ERP编码为空。");
                        if (supplierErpCode.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", supplierErpCode)}");
                        }
                        //校验供应商名称
                        IEnumerable<string> supplierName = rows.Where(p => p.SupplierName.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 供应商名称为空。");
                        if (supplierName.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", supplierName)}");
                        }
                        //校验物料编码
                        IEnumerable<string> materialCode = rows.Where(p => p.MaterialCode.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 物料编码为空。");
                        if (materialCode.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", materialCode)}");
                        }
                        //校验物料名称
                        IEnumerable<string> materialName = rows.Where(p => p.MaterialCode.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 物料名称为空。");
                        if (materialName.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", materialName)}");
                        }
                        //校验基本单位
                        IEnumerable<string> basicUnit = rows.Where(p => p.BasicUnit.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 基本单位为空。");
                        if (basicUnit.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", basicUnit)}");
                        }
                        //校验是否冻结
                        IEnumerable<string> freezeOrNot = rows.Where(p => p.FreezeOrNot.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 是否冻结为空。");
                        if (freezeOrNot.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", freezeOrNot)}");
                        }
                        //校验生效日期
                        IEnumerable<string> effectiveDate = rows.Where(p => p.EffectiveDate == default(DateTime)).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 生效日期为空。");
                        if (effectiveDate.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", effectiveDate)}");
                        }
                        //校验失效日期
                        IEnumerable<string> expirationDate = rows.Where(p => p.ExpirationDate == default(DateTime)).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 失效日期为空。");
                        if (expirationDate.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", expirationDate)}");
                        }
                        //校验价格基数
                        IEnumerable<string> priceBase = rows.Where(p => p.PriceBase.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 价格基数为空。");
                        if (priceBase.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", priceBase)}");
                        }
                        //校验货币编码
                        IEnumerable<string> currencyCode = rows.Where(p => p.CurrencyCode.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 货币编码为空。");
                        if (currencyCode.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", currencyCode)}");
                        }
                        //校验报价类型
                        IEnumerable<string> quotationType = rows.Where(p => p.QuotationType.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 报价类型为空。");
                        if (quotationType.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", quotationType)}");
                        }
                        //校验物料管制状态
                        IEnumerable<string> materialControlStatus = rows.Where(p => p.MaterialControlStatus.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 3}行, 物料管制状态为空。");
                        if (materialControlStatus.Any())
                        {
                            throw new FriendlyException($"{string.Join("\r\n", materialControlStatus)}");
                        }
                        #endregion
                        //单独获取返点率
                        List<RebateRateColumn> rebateRate = memoryStream.Query<RebateRateColumn>(startCell: "BC2").ToList();
                        //单独获取年降率                    
                        List<AnnualGeneralRateColumn> annualGeneralRate = memoryStream.Query<AnnualGeneralRateColumn>(startCell: "BH2").ToList();
                        //单独获取未税价
                        List<UntaxedPriceColumn> untaxedPrice = memoryStream.Query<UntaxedPriceColumn>(startCell: "BL2").ToList();
                        //返点率表头
                        List<int> rebateRateHeader = rebateRate[0].ToListInt();
                        //年降率表头
                        List<int> annualGeneralRateHeader = annualGeneralRate[0].ToListInt();
                        //未税价表头
                        List<int> untaxedPriceHeader = untaxedPrice[0].ToListInt();
                        if (rows.Count != rebateRate.Count - 1)
                        {
                            throw new FriendlyException("读取文件的过程中产生了错误,返点率行数与表格不一");
                        }
                        if (rows.Count != annualGeneralRate.Count - 1)
                        {
                            throw new FriendlyException("读取文件的过程中产生了错误,年降率行数与表格不一");
                        }
                        if (rows.Count != untaxedPrice.Count - 1)
                        {
                            throw new FriendlyException("读取文件的过程中产生了错误,未税价行数与表格不一");
                        }
                        //将返点率加入到主数据当中
                        try
                        {
                            for (int i = 1; i < rebateRate.Count; i++)
                            {
                                List<UInitPriceFormYearOrValueMode> uInitPriceFormYearOrValueModes = new List<UInitPriceFormYearOrValueMode>();
                                for (int j = 0; j < rebateRateHeader.Count; j++)
                                {
                                    List<decimal> rebateRates = rebateRate[i].ToListDecimal();
                                    UInitPriceFormYearOrValueMode uInitPriceFormYearOrValueMode = new UInitPriceFormYearOrValueMode
                                    {
                                        UInitPriceFormType = UInitPriceFormType.RebateRate,
                                        Year = rebateRateHeader[j],
                                        Value = Convert.ToDecimal(rebateRates[j])
                                    };
                                    uInitPriceFormYearOrValueModes.Add(uInitPriceFormYearOrValueMode);
                                }
                                rows[i - 1].UInitPriceFormYearOrValueModes = uInitPriceFormYearOrValueModes;
                            }

                        }
                        catch (Exception)
                        {

                            throw new FriendlyException("返点率数据出错,请检查是否全部填写完毕(数字类型)或者表头年份错误(不可带中文)!");
                        }
                        //将年降率加入到主数据当中
                        try
                        {
                            for (int i = 1; i < annualGeneralRate.Count; i++)
                            {
                                for (int j = 0; j < annualGeneralRateHeader.Count; j++)
                                {
                                    List<decimal> rebateRates = annualGeneralRate[i].ToListDecimal();
                                    UInitPriceFormYearOrValueMode uInitPriceFormYearOrValueMode = new UInitPriceFormYearOrValueMode
                                    {
                                        UInitPriceFormType = UInitPriceFormType.AnnualDeclineRate,
                                        Year = annualGeneralRateHeader[j],
                                        Value = Convert.ToDecimal(rebateRates[j])
                                    };
                                    rows[i - 1].UInitPriceFormYearOrValueModes.Add(uInitPriceFormYearOrValueMode);
                                }
                            }
                        }
                        catch (Exception)
                        {

                            throw new FriendlyException("年降率数据出错,请检查是否全部填写完毕(数字类型)或者表头年份错误(不可带中文)!");
                        }
                        //将未税价数据加入到主数据当中
                        try
                        {
                            for (int i = 1; i < untaxedPrice.Count; i++)
                            {
                                for (int j = 0; j < untaxedPriceHeader.Count; j++)
                                {
                                    List<decimal> rebateRates = untaxedPrice[i].ToListDecimal();
                                    UInitPriceFormYearOrValueMode uInitPriceFormYearOrValueMode = new UInitPriceFormYearOrValueMode
                                    {
                                        UInitPriceFormType = UInitPriceFormType.AnnualUntaxedPrice,
                                        Year = untaxedPriceHeader[j],
                                        Value = Convert.ToDecimal(rebateRates[j])
                                    };
                                    rows[i - 1].UInitPriceFormYearOrValueModes.Add(uInitPriceFormYearOrValueMode);
                                }
                            }
                        }
                        catch (Exception)
                        {

                            throw new FriendlyException("未税价数据出错,请检查是否全部填写完毕(数字类型)或者表头年份错误(不可带中文)!");
                        }
                        var pp = rows.Where(p => p.UInitPriceFormYearOrValueModes == null);
                        try
                        {
                            int AllCount = await _configUInitPriceForm.CountAsync();
                            if (AllCount is not 0)
                            {
                                await _configUInitPriceForm.DeleteAllEntities();
                            }
                            List<UInitPriceForm> price = ObjectMapper.Map<List<UInitPriceForm>>(rows);
                            await _configUInitPriceForm.BulkInsertAsync(price);
                        }
                        catch (AutoMapperMappingException ex)
                        {
                            //首先捕获 转换过程中的内部异常
                            throw new FriendlyException(ex.InnerException.InnerException.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw new FriendlyException(ex.Message);
                        }
                    }
                    else
                    {
                        throw new FriendlyException("文件过大");
                    }
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 单价库模版下载
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [HttpGet]
        public  IActionResult UnitPriceLibraryTemplateDownload(string FileName = "单价库模版")
        {
            try
            {
                string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\单价库模版.xlsx";
                return new FileStreamResult(File.OpenRead(templatePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{FileName}.xlsx"
                };
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 导入共用物料库
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task ImportPublicMaterialWarehouse(IFormFile file)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    string[] suffix = file.FileName.Split(".");
                    if (suffix.Length < 1)
                    {
                        throw new FriendlyException("文件名错误");
                    }
                    await file.CopyToAsync(memoryStream);
                    //载入xls文档
                    Workbook workbook = new Workbook();
                    if (suffix[suffix.Length - 1].Equals("xls"))
                    {
                        string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\PublicMaterialWarehouse.xlsx";
                        workbook.LoadFromStream(memoryStream);
                        //保存为xlsx格式
                        workbook.SaveToFile(templatePath, ExcelVersion.Version2016);
                        using (FileStream fs = File.OpenRead(templatePath))
                        {
                            memoryStream.SetLength(0);
                            await fs.CopyToAsync(memoryStream);
                        }
                    }
                    //获取共用物料库
                    List<SharedMaterialWarehouseMode> rows = memoryStream.Query<SharedMaterialWarehouseMode>(startCell: "A1").ToList();
                    //第一条数据为表格合并后获取的空数据
                    rows.RemoveAt(0);
                    List<dynamic> row = memoryStream.Query(useHeaderRow: true, startCell: "F2").ToList();
                    for (int i = 0; i < row.Count; i++)
                    {
                        List<YearOrValueModeCanNull> yearOrValueModeCanNulls = new();
                        foreach (var d in row[i])
                        {
                            YearOrValueModeCanNull yearOrValueModeCanNull = null;
                            try
                            {
                                yearOrValueModeCanNull = new YearOrValueModeCanNull()
                                {
                                    Year = Convert.ToInt32(d.Key)
                                };
                                //走量的值是可为空的,所以这里要判断一下
                                if (d.Value is not null)
                                {
                                    yearOrValueModeCanNull.Value = Convert.ToDecimal(d.Value);
                                }
                            }
                            catch (Exception)
                            {
                                throw new FriendlyException($"{i + 3}行,请检查模组走量年份/值,不允许有中文或者空格");
                            }

                            yearOrValueModeCanNulls.Add(yearOrValueModeCanNull);
                        }
                        rows[i].ModuleThroughputs = yearOrValueModeCanNulls;
                    }
                    List<SharedMaterialWarehouse> sharedMaterialWarehouses = new List<SharedMaterialWarehouse>();
                    try
                    {
                        sharedMaterialWarehouses = ObjectMapper.Map<List<SharedMaterialWarehouse>>(rows);
                    }
                    catch (AutoMapperMappingException)
                    {
                        //首先捕获 转换过程中的内部异常 如果捕捉到了 则就是 Excel类型问题,反之则捕捉不到
                        throw new FriendlyException($"对象映射出现了问题,请检查Excel中输入的值的类型!");
                    }
                    catch (Exception ex)
                    {
                        throw new FriendlyException(ex.Message);
                    }
                    //根据财务要求手动添加 删除/覆盖同(子项目代码和产品名称和方案名称)物料的值      
                    #region 添加之前删除同子项目代码的数据            
                    foreach (SharedMaterialWarehouse item in sharedMaterialWarehouses)
                    {
                        await _sharedMaterialWarehouse.HardDeleteAsync(p => p.ProjectSubcode.Equals(item.ProjectSubcode) && p.ProductName.Equals(item.ProductName) && p.SolutionName.Equals(item.SolutionName));
                    }
                    #endregion
                    await _sharedMaterialWarehouse.BulkInsertAsync(sharedMaterialWarehouses);
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 导出共用物料库
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ExportSharedMaterialWarehouse()
        {
            try
            {
                //用MiniExcel读取数据
                var values = new List<Dictionary<string, object>>();
                List<SharedMaterialWarehouse> sharedMaterialWarehouses = await _sharedMaterialWarehouse.GetAllListAsync();
                foreach (SharedMaterialWarehouse item in sharedMaterialWarehouses)
                {
                    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
                {
                    { "项目名称", item.EntryName },
                    { "项目子代码", item.ProjectSubcode },
                    { "产品名称", item.ProductName },
                    { "方案名称", item.SolutionName},
                    { "物料编码", item.MaterialCode },
                    { "物料名称", item.MaterialName },
                    { "装配数量", item.AssemblyQuantity }
                };
                    List<YearOrValueModeCanNull> yearOrValueModeCanNulls = JsonConvert.DeserializeObject<List<YearOrValueModeCanNull>>(item.ModuleThroughputs);
                    foreach (YearOrValueModeCanNull YearOrValue in yearOrValueModeCanNulls)
                    {
                        keyValuePairs.Add(YearOrValue.Year.ToString(), YearOrValue.Value);
                    }
                    values.Add(keyValuePairs);
                }
                //再使用 NPOI 合并需要合并的表格 添加样式等
                MemoryStream memoryStream = new MemoryStream();
                MiniExcel.SaveAs(memoryStream, values);
                memoryStream.Seek(0, SeekOrigin.Begin);
                DataTable dataTable = MiniExcel.QueryAsDataTable(memoryStream, useHeaderRow: true);
                //上下合并的列
                var MergeColumnsUpAndDown = 5;
                // 创建工作簿
                IWorkbook workbook = new XSSFWorkbook();
                // 创建一个字体
                IFont font = workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeightInPoints = 10;
                font.Color = IndexedColors.Black.Index;
                // 创建一个样式
                ICellStyle style = workbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                ISheet sheet = workbook.CreateSheet("Sheet1");
                // 创建表头
                IRow headerRow = sheet.CreateRow(0);
                //创建子表头
                IRow subhead = sheet.CreateRow(1);
                //表头样式
                ICellStyle headerstyle = workbook.CreateCellStyle();
                headerstyle.CloneStyleFrom(style);
                headerstyle.FillForegroundColor = IndexedColors.Yellow.Index;
                headerstyle.FillPattern = FillPattern.SolidForeground;
                headerRow.HeightInPoints = StaticParameterConstant.HeightInPoints; // 设置行高               
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    ICell headerCell = headerRow.CreateCell(i);
                    ICell headerCell1 = subhead.CreateCell(i);
                    headerCell1.CellStyle = headerstyle; // 应用样式
                    if (i >= MergeColumnsUpAndDown)
                    {
                        headerCell.SetCellValue("模组走量");
                        headerCell1.SetCellValue(dataTable.Columns[i].ColumnName);
                    }
                    else
                    {
                        headerCell.SetCellValue(dataTable.Columns[i].ColumnName);
                        headerCell1.SetCellValue(default(decimal).ToString());
                    }
                    headerCell.CellStyle = headerstyle; // 应用样式            
                }
                // 填充数据
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i + 2);
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        ICell dataCell = dataRow.CreateCell(j);
                        dataCell.SetCellValue(dataTable.Rows[i][j].ToString());
                        dataCell.CellStyle = style; // 应用样式
                    }
                }
                //合并模组走量表格
                CellRangeAddress mergeRange = new CellRangeAddress(0, 0, MergeColumnsUpAndDown, dataTable.Columns.Count - 1);
                sheet.AddMergedRegion(mergeRange);
                // 合并前5行第一个单元格
                for (int i = 0; i < MergeColumnsUpAndDown; i++)
                {
                    CellRangeAddress region = new CellRangeAddress(0, 1, i, i);
                    sheet.AddMergedRegion(region);
                }
                // 保存工作簿
                using (MemoryStream fileStream = new MemoryStream())
                {
                    workbook.Write(fileStream);
                    return new FileContentResult(fileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"共用物料库{DateTime.Now}.xlsx"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 添加共用物料库(归档之后添加可用接口)
        /// 财务需求:如果项目归档定标之后 需要在共用物料库中添加相对应的数据,更导入一样,都需要删除/覆盖同子项目代码的数据
        /// </summary>
        /// <param name="sharedMaterialWarehouseModes"></param>
        /// <returns></returns>
        public async Task AddPublicMaterialWarehouse(List<SharedMaterialWarehouseMode> sharedMaterialWarehouseModes)
        {
            try
            {
                //根据财务要求手动添加 删除/覆盖同(子项目代码和产品名称和方案名称)物料的值      
                #region 添加之前删除同子项目代码的数据            
                foreach (SharedMaterialWarehouseMode item in sharedMaterialWarehouseModes)
                {
                    await _sharedMaterialWarehouse.HardDeleteAsync(p => p.ProjectSubcode.Equals(item.ProjectSubcode) &&p.ProductName.Equals(item.ProductName)&& p.SolutionName.Equals(item.SolutionName));
                }                            
                #endregion
                List<SharedMaterialWarehouse> sharedMaterialWarehouses = ObjectMapper.Map<List<SharedMaterialWarehouse>>(sharedMaterialWarehouseModes);
                await _sharedMaterialWarehouse.BulkInsertAsync(sharedMaterialWarehouses);
            }
            catch (AutoMapperMappingException)
            {
                //首先捕获 转换过程中的内部异常 如果捕捉到了 则就是 Excel类型问题,反之则捕捉不到
                throw new FriendlyException($"对象映射出现了问题,请检查Excel中输入的值的类型!");
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 共用物料库模版下载
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ExportSharedMaterialWarehouseDownload(string FileName = "共用物料库模版")
        {
            try
            {
                string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\共用库模版.xlsx";
                return new FileStreamResult(File.OpenRead(templatePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{FileName}.xlsx"
                };
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 删除共用物料
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task DeletePublicMaterials(long Id)
        {
            try
            {
                await _sharedMaterialWarehouse.DeleteAsync(x => x.Id == Id);
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 删除多个共用物料
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public async Task DeleteMultiplePublicMaterials(List<long> Ids)
        {
            try
            {
                await _sharedMaterialWarehouse.DeleteAsync(x => Ids.Contains(x.Id));
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 查询共用物料库(查询依据 MaterialCode=>物料编码)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<SharedMaterialWarehouseMode>> GetQueryPublicMaterialWarehouse(QueryInteractionClass input)
        {
            try
            {
                IQueryable<SharedMaterialWarehouse> filter = _sharedMaterialWarehouse.GetAll().WhereIf(!input.Filter.IsNullOrEmpty(), p => p.MaterialCode.Contains(input.Filter));
                var pagedSorted = filter.OrderByDescending(p => p.Id).PageBy(input);
                //获取总数
                var count = await filter.CountAsync();
                //获取查询结果
                List<SharedMaterialWarehouse> result = await pagedSorted.ToListAsync();
                return new PagedResultDto<SharedMaterialWarehouseMode>(count, ObjectMapper.Map<List<SharedMaterialWarehouseMode>>(result));
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 查询毛利率方案(查询依据 GrossMarginName)
        /// </summary>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task<PagedResultDto<GrossMarginDto>> GetGrossMargin(GrossMarginInputDto input)
        {
            try
            {
                //定义列表查询
                var filter = _configGrossMarginForm.GetAll().WhereIf(!input.GrossMarginName.IsNullOrEmpty(), p => p.GrossMarginName.Contains(input.GrossMarginName));
                var pagedSorted = filter.OrderByDescending(p => p.Id).PageBy(input);
                //获取总数
                var count = await filter.CountAsync();
                //获取查询结果
                var result = await pagedSorted.ToListAsync();
                return new PagedResultDto<GrossMarginDto>(count, ObjectMapper.Map<List<GrossMarginDto>>(result));
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 添加/修改毛利率方案 有则修改无则添加
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task PostGrossMargin(BacktrackGrossMarginDto price)
        {
            try
            {
                //判断添加或者修改的是否是默认的方案
                if (price.IsDefaultn)
                {
                    List<GrossMarginForm> entityDefaultn = await _configGrossMarginForm.GetAllListAsync(p => p.IsDefaultn);
                    foreach (var item in entityDefaultn)
                    {
                        item.IsDefaultn = false;
                        await _configGrossMarginForm.UpdateAsync(item);
                    }
                }
                var prop = ObjectMapper.Map<GrossMarginForm>(price);
                GrossMarginForm entity = await _configGrossMarginForm.FirstOrDefaultAsync(p => p.Id.Equals(price.Id));
                if (entity == null)
                {
                    await _configGrossMarginForm.InsertAsync(prop);
                    await CreateLog("添加了毛利率方案1条", GrossProfitMarginType);
                }
                else
                {
                    entity.GrossMarginPrice = prop.GrossMarginPrice;
                    entity.GrossMarginName = prop.GrossMarginName;
                    entity.IsDefaultn = price.IsDefaultn;
                    await _configGrossMarginForm.UpdateAsync(entity);
                    await CreateLog("编辑毛利率方案1条", GrossProfitMarginType);
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 删除毛利率方案
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task DeleteGrossMargin(long Id)
        {
            GrossMarginForm entity = await _configGrossMarginForm.FirstOrDefaultAsync(p => p.Id.Equals(Id));
            if (entity != null)
            {
                if (entity.IsDefaultn) throw new FriendlyException("不能删除默认的毛利率");
                await _configGrossMarginForm.DeleteAsync(entity);
                await CreateLog("删除毛利率方案1条", GrossProfitMarginType);
            }
        }
        /// <summary>
        /// 添加/修改汇率
        /// </summary>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task PostExchangeRate(ExchangeRateDto exchangeRate)
        {
            try
            {
                ExchangeRate entity = await _configExchangeRate.FirstOrDefaultAsync(p => p.Id.Equals(exchangeRate.Id));
                if (entity == null)
                {
                    var prop = ObjectMapper.Map<ExchangeRate>(exchangeRate);
                    await _configExchangeRate.InsertAsync(prop);
                    await CreateLog("添加汇率1条", ExchangeRateType);
                }
                else
                {
                    entity.ExchangeRateKind = exchangeRate.ExchangeRateKind;
                    entity.ExchangeRateValue = JsonConvert.SerializeObject(exchangeRate.ExchangeRateValue);
                    await _configExchangeRate.UpdateAsync(entity);
                    await CreateLog("编辑汇率1条", ExchangeRateType);
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 查询汇率
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        [AbpAuthorize]
        public async Task<PagedResultDto<ExchangeRateDto>> GetExchangeRate(ExchangeRateInputDto input)
        {
            try
            {
                //定义列表查询
                var filter = _configExchangeRate.GetAll().WhereIf(!input.ExchangeRateKind.IsNullOrEmpty(), p => p.ExchangeRateKind.Contains(input.ExchangeRateKind));
                var pagedSorted = filter.OrderByDescending(p => p.Id).PageBy(input);
                //获取总数
                var count = await filter.CountAsync();
                //获取查询结果
                var result = await pagedSorted.ToListAsync();
                return new PagedResultDto<ExchangeRateDto>(count, ObjectMapper.Map<List<ExchangeRateDto>>(result));
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 删除汇率
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task DeleteExchangeRate(long Id)
        {
            ExchangeRate entity = await _configExchangeRate.FirstOrDefaultAsync(Id);
            if (entity != null)
            {
                await _configExchangeRate.DeleteAsync(entity);
                await CreateLog("删除汇率1条", ExchangeRateType);
            }
        }
        /// <summary>
        ///  损耗率模板导出
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [AbpAuthorize]
        public IActionResult ExportOfLossRateTemplate(string FileName = "损耗率模版")
        {
            try
            {
                string templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\wwwroot\Excel\损耗率模板.xlsx";
                return new FileStreamResult(File.OpenRead(templatePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"{FileName}.xlsx"
                };
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        /// <summary>
        ///  损耗率导入
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [AbpAuthorize]
        public async Task LossRateImport(IFormFile filename)
        {
            try
            {
                if (Path.GetExtension(filename.FileName) is not ".xlsx") throw new FriendlyException("模板文件类型不正确");             
                await _lossRateInfo.HardDeleteAsync(p => true);
                await _lossRateYearInfo.DeleteAllEntities();                
                using (var memoryStream = new MemoryStream())
                {
                    await filename.CopyToAsync(memoryStream);
                    List<LossRateModel> rowExcls = memoryStream.Query<LossRateModel>().ToList();
                    //检验
                    var duplicateItem = rowExcls.GroupBy(x => new { x.SuperType, x.MaterialCategory, x.CategoryName })
                    .Where(g => g.Count() > 1)
                                .SelectMany(g => g.Select(x => new { x, RowNumber = rowExcls.IndexOf(x) + 2 }))
                                .FirstOrDefault();
                    if (duplicateItem != null)
                    {
                        throw new FriendlyException("第" + duplicateItem.RowNumber + "行的产品大类和物料大类和物料种类有相同的,请查看并修改!");                       
                    }               
                    //
                    List<LossRateSopModel> rowExcl = memoryStream.Query<LossRateSopModel>().ToList();
                    if (rowExcls.Count != rowExcl.Count) throw new FriendlyException("读取文件的过程中产生了错误,产品大类/物料大类和SOP行数不一");
                    List<LossRateInfo> prop = await _lossRateInfo.BulkInsertAsync(ObjectMapper.Map<List<LossRateInfo>>(rowExcls));
                    List<LossRateYearInfo> year = new List<LossRateYearInfo>();
                    for (int i = 0; i < prop.Count; i++)
                    {
                        //年降率表头
                        List<decimal> annualGeneralRateHeader = rowExcl[i].ToListDecimal();
                        for (int j = 0; j < annualGeneralRateHeader.Count; j++)
                        {
                            if (annualGeneralRateHeader[j] == 0) throw new FriendlyException($"导入错误!  {i + 2}行,{j + 4}列数值为0");
                            year.Add(new LossRateYearInfo()
                            {
                                LossRateInfoId = prop[i].Id,
                                Year = j,
                                Rate = annualGeneralRateHeader[j]
                            });
                        }
                    }
                    await CreateLog($"导入了损耗率表单 {prop.Count} 条", LossRateType);
                    await _lossRateYearInfo.BulkInsertAsync(year);                    
                }
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 损耗率查询
        /// </summary>
        /// <param name="filterPagedInputDto"></param>
        /// <returns></returns>       
        [AbpAuthorize]
        public async Task<PagedResultDto<LossRatesDto>> LossRateQuery(FilterPagedInputDto filterPagedInputDto)
        {
            try
            {
                //定义列表查询
                List<LossRateInfo> lossRateInfo = _lossRateInfo.GetAll().WhereIf(!filterPagedInputDto.Filter.IsNullOrEmpty(), p => p.SuperType.Contains(filterPagedInputDto.Filter) || p.CategoryName.Contains(filterPagedInputDto.Filter)||p.MaterialCategory.Contains(filterPagedInputDto.Filter)).ToList();
                List<LossRateYearInfo> lossRateYearInfo = await _lossRateYearInfo.GetAllListAsync();
                IQueryable<LossRatesDto> filter = (from s in lossRateInfo
                                                   join b in lossRateYearInfo on s.Id equals b.LossRateInfoId into bGroup
                                                   select new LossRatesDto
                                                   {

                                                       SuperType = s.SuperType,
                                                       CategoryName = s.CategoryName,
                                                       MaterialCategory = s.MaterialCategory,
                                                       LossRateYearList = bGroup.Select(t => new LossRatesYearDto
                                                       {
                                                           Year = t.Year,
                                                           YearAlias = t.Year != 0 ? "SOP+" + t.Year : "SOP",
                                                           Rate = t.Rate
                                                       }).ToList()
                                                   }).AsQueryable();

                IQueryable<LossRatesDto> pagedSorted = filter.PageBy(filterPagedInputDto);
                //获取总数
                var count = filter.Count();
                //获取查询结果
                List<LossRatesDto> result = pagedSorted.ToList();
                return new PagedResultDto<LossRatesDto>(count, result);
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 损耗率导出
        /// </summary>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task<IActionResult> LossRateExport()
        {
            PagedResultDto<LossRatesDto> lossRatesDtos = await LossRateQuery(new FilterPagedInputDto() { Filter = "", MaxResultCount = 9999, PageIndex = 0, SkipCount = 0 });
            //用MiniExcel读取数据
            var values = new List<Dictionary<string, object>>();
            foreach (LossRatesDto item in lossRatesDtos.Items)
            {
                Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
                    {
                     { "产品大类", item.SuperType },
                     { "物料大类", item.MaterialCategory },
                     { "物料种类", item.CategoryName },
                    };
                foreach (LossRatesYearDto pro in item.LossRateYearList)
                {
                    keyValuePairs.Add(pro.YearAlias, pro.Rate);
                }
                values.Add(keyValuePairs);
            }
            MemoryStream memoryStream = new MemoryStream();
            await MiniExcel.SaveAsAsync(memoryStream, values);
            return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"损耗率导出-{DateTime.Now}.xlsx"
            };
        }
        /// <summary>
        /// 添加质量成本比例
        /// </summary>
        /// <param name="qualityCostRatioDtos"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task QualityCostRatioSubmission(List<QualityCostRatioDto> qualityCostRatioDtos)
        {
            //判断前端传入所有数据的年份是否相同
            bool allHaveSameCount = qualityCostRatioDtos.All(x => x.qualityCostRatioYears.Count == qualityCostRatioDtos.First().qualityCostRatioYears.Count);
            if (!allHaveSameCount) throw new FriendlyException("数据中年份必须统一/该数据年份未统一");
            foreach (QualityCostRatioDto qualityModel in qualityCostRatioDtos)
            {
                QualityCostRatio qualityCostRatio = ObjectMapper.Map<QualityCostRatio>(qualityModel);
                long qualityCostRatioId = await _qualityCostRatio.InsertOrUpdateAndGetIdAsync(qualityCostRatio);
                qualityModel.qualityCostRatioYears.Select((p, i) => { p.QualityCostRatioId = qualityCostRatioId; p.Year = i; return p; }).ToList();
                List<QualityCostRatioYear> qualityCostRatioYears = await _qualityCostRatioYear.GetAllListAsync(p => p.QualityCostRatioId.Equals(qualityCostRatioId));
                await _qualityCostRatioYear.HardDeleteAsync(p => qualityCostRatioYears.Select(l => l.Id).Contains(p.Id));
                List<QualityCostRatioYear> qualityCostRatios = ObjectMapper.Map<List<QualityCostRatioYear>>(qualityModel.qualityCostRatioYears);
                await _qualityCostRatioYear.BulkInsertAsync(qualityCostRatios);
            }
            await CreateLog($"添加或者更改了质量成本比例表单 {qualityCostRatioDtos.Count} 条", QualityCostRatioType);
        }
        /// <summary>
        /// 查询质量成本比例
        /// </summary>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task<PagedResultDto<QualityCostRatioDto>> QueryQualityCostRatio(FilterPagedInputDto filterPagedInputDto)
        {
            List<QualityCostRatio> qualityCostRatios = await _qualityCostRatio.GetAllListAsync();
            List<QualityCostRatioYear> qualityCostRatioYears = await _qualityCostRatioYear.GetAllListAsync();
            IQueryable<QualityCostRatioDto> prop = (from a in qualityCostRatios
                                                    join b in qualityCostRatioYears on a.Id equals b.QualityCostRatioId into ab
                                                    where ab != null
                                                    select new QualityCostRatioDto
                                                    {
                                                        Id = a.Id,
                                                        IsItTheFirstProduct = a.IsItTheFirstProduct,
                                                        Category = a.Category,
                                                        qualityCostRatioYears = ab.Select(p => new QualityCostRatioYearDto
                                                        {
                                                            YearAlias = p.Year != 0 ? "SOP+" + p.Year : "SOP",
                                                            Value = p.Value,
                                                            Year = p.Year,
                                                            QualityCostRatioId = a.Id
                                                        }).ToList()
                                                    }).AsQueryable();
            IQueryable<QualityCostRatioDto> pagedSorted = prop.PageBy(filterPagedInputDto);
            //获取总数
            var count = prop.Count();
            //获取查询结果
            List<QualityCostRatioDto> result = pagedSorted.ToList();
            return new PagedResultDto<QualityCostRatioDto>(count, result);
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
