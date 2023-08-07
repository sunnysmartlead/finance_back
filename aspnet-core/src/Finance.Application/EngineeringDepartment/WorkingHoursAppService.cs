using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Finance.Audit;
using Finance.Dto;
using Finance.EngineeringDepartment.Dto;
using Finance.Ext;
using Finance.FinanceDepartment.Dto;
using Finance.FinanceParameter;
using Finance.Nre;
using Finance.PriceEval;
using Finance.ProductDevelopment;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EngineeringDepartment
{
    
    public class WorkingHoursAppService : FinanceAppServiceBase
    {
        /// <summary>
        /// 工时工序保存字符
        /// </summary>
        public const string WorkHours = "WORKHOURS";
        /// <summary>
        /// 切线工时保存字符
        /// </summary>
        public const string TangentHours = "TANGENTHOURS";

        private readonly ILogger<WorkingHoursAppService> _logger;

        private readonly IRepository<WorkingHoursInfo, long> _workingHoursRepository;

        private readonly IRepository<EquipmentInfo, long> _equipmentInfoRepository;

        private readonly IRepository<YearInfo, long> _yearRepository;

        private readonly IRepository<Pcs, long> _pcsRepository;

        private readonly IRepository<PcsYear, long> _pcsYearRepository;

        private readonly IRepository<UPHInfo, long> _uphRepository;

        private readonly IRepository<ManufacturingCostInfo, long> _manufacturingCostInfo;

        private readonly IRepository<ModelCount, long> _modelCountRepository;

        private readonly IRepository<ProductDevelopmentInput, long> _productDevelopmentInputRepository;

        private readonly IRepository<FileManagement, long> _fileManagementRepository;
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
        /// <param name="logger"></param>
        /// <param name="workingHoursRepository"></param>
        /// <param name="equipmentInfoRepository"></param>
        /// <param name="yearRepository"></param>
        /// <param name="pcsRepository"></param>
        /// <param name="pcsYearRepository"></param>
        /// <param name="uphRepository"></param>
        /// <param name="manufacturingCostInfo"></param>
        /// <param name="modelCountRepository"></param>
        /// <param name="productDevelopmentInputRepository"></param>
        /// <param name="fileManagementRepository"></param>
        /// <param name="flowAppService"></param>
        /// <param name="productIsSubmit"></param>
        public WorkingHoursAppService(ILogger<WorkingHoursAppService> logger, IRepository<WorkingHoursInfo, long> workingHoursRepository, IRepository<EquipmentInfo, long> equipmentInfoRepository, IRepository<YearInfo, long> yearRepository, IRepository<Pcs, long> pcsRepository, IRepository<PcsYear, long> pcsYearRepository, IRepository<UPHInfo, long> uphRepository, IRepository<ManufacturingCostInfo, long> manufacturingCostInfo, IRepository<ModelCount, long> modelCountRepository, IRepository<ProductDevelopmentInput, long> productDevelopmentInputRepository, IRepository<FileManagement, long> fileManagementRepository, AuditFlowAppService flowAppService, IRepository<NreIsSubmit, long> productIsSubmit)
        {
            _logger = logger;
            _workingHoursRepository = workingHoursRepository;
            _equipmentInfoRepository = equipmentInfoRepository;
            _yearRepository = yearRepository;
            _pcsRepository = pcsRepository;
            _pcsYearRepository = pcsYearRepository;
            _uphRepository = uphRepository;
            _manufacturingCostInfo = manufacturingCostInfo;
            _modelCountRepository = modelCountRepository;
            _productDevelopmentInputRepository = productDevelopmentInputRepository;
            _fileManagementRepository = fileManagementRepository;
            _flowAppService = flowAppService;
            _productIsSubmit = productIsSubmit;
        }





        /// <summary>
        /// 上传后读取工时工序接口
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<WorkingHourDto> UploadExcel(IFormFile file,long AuditFlowId)
        {

            //打开上传文件的输入流
            Stream stream = file.OpenReadStream();

            //根据文件流创建excel数据结构
            IWorkbook workbook = WorkbookFactory.Create(stream);
            stream.Close();

            //尝试获取第一个sheet
            var sheet = workbook.GetSheetAt(0);

            List<WorkingHourDetail> list = new List<WorkingHourDetail>();
            WorkingHourDto result = new WorkingHourDto();

            //各部分的设备循环数量 SOP数量
            int EquipmentNum = 0;
            int RetrospectNum = 0;
            int ToolingNum = 0;
            int SOPNum = 0;

            //判断是否获取到 sheet
            if (sheet != null)
            {
                //首先逐列循环第二行内容，根据内容获取各部分的动态数量
                var initRow = sheet.GetRow(1);
                //设备总价的列
                int cellNum_EquipmentTotal = 0;
                //追溯部分硬件总价 的列
                int cellNum_RetrospectTotal = 0;
                //工装治具-工装名称的列
                int cellNum_ToolingName = 0;
                //最后一列
                int cellNum_last = 0;
                for (int i = 2; i < 100; i++)//100为自定义，实际循环中不会达到
                {
                    if (null==initRow.GetCell(i)||string.IsNullOrEmpty(initRow.GetCell(i).ToString()))
                    {
                        cellNum_last = i-1;
                        break;
                    }
                    else {
                        string value = initRow.GetCell(i).ToString().Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""); 
                        switch (value) 
                        {
                            case "设备总价" : cellNum_EquipmentTotal = i;break;
                            case "硬件总价": cellNum_RetrospectTotal = i; break;
                            case "工装名称": cellNum_ToolingName = i; break;
                        }
                    }

                }
                
                //验证获取到的列数是否异常
                if ((cellNum_EquipmentTotal-2)%4!=0)
                {
                    throw new FriendlyException("设备部分列名或列数存在错误，请检查！");
                }
                else {
                    EquipmentNum = (cellNum_EquipmentTotal-2)/4;
                }
                if ((cellNum_RetrospectTotal-cellNum_EquipmentTotal-1)%3!=0)
                {
                    throw new FriendlyException("追溯部分列名或列数存在错误，请检查！");
                }
                else {
                    RetrospectNum=(cellNum_RetrospectTotal-cellNum_EquipmentTotal-1)/3;
                }
                if ((cellNum_ToolingName-cellNum_RetrospectTotal-6)%3!=0)
                {
                    throw new FriendlyException("工装治具部分列名或列数存在错误，请检查！");
                }
                else {
                    ToolingNum =(cellNum_ToolingName-cellNum_RetrospectTotal-6)/3;
                }
                if ((cellNum_last-cellNum_ToolingName-6)%3!=0)
                {
                    throw new FriendlyException("SOP工时与数量部分列名或列数存在错误，请检查！");
                }
                else {
                    SOPNum = (cellNum_last-cellNum_ToolingName-6)/3;
                }

                //获取年份数据
                List<int> yearList = new List<int>();
                List<Pcs> data = await _pcsRepository.GetAll().Where(p => AuditFlowId.Equals(p.AuditFlowId)).ToListAsync();
                if (data.Count > 0)
                {
                    List<PcsYear> pcsYears = await _pcsYearRepository.GetAllListAsync(p => p.PcsId == data.FirstOrDefault().Id);
                    if (pcsYears.Count > 0)
                    {
                        foreach (PcsYear pcsYear in pcsYears)
                        {
                            yearList.Add(pcsYear.Year);
                        }
                    }
                }
                if (SOPNum!=yearList.Count) {
                    throw new FriendlyException("SOP工时与数量部分与流程数据不一致，请检查！");
                }


                //最后一行的标号
                int lastRowNum = sheet.LastRowNum;

                //从第三行开始获取
                for (int i = 2; i <=lastRowNum; i++)
                {
                    int nowCellNum = 0;
                    try { 

                        var row = sheet.GetRow(i);

                        WorkingHourDetail dto = new WorkingHourDetail();
                        string rowcell = row.GetCell(0).ToString();
                        if (rowcell == "")
                        {
                            break;
                        }
                        dto.SequenceNumber =  int.Parse(row.GetCell(0).ToString());
                        dto.Procedure = row.GetCell(1).ToString();
                        //设备部分
                        EquipmentPart equipmentPart = new EquipmentPart();
                        equipmentPart.Total = double.Parse(row.GetCell(cellNum_EquipmentTotal).ToString());
                        List<EquipmentDetail> EquipmentDetails_Equipment = new List<EquipmentDetail>();
                        nowCellNum = 1;
                        for (int a=0;a<EquipmentNum; a++) {

                            EquipmentDetail equipmentDetail = new EquipmentDetail();
                                if (null==row.GetCell(nowCellNum+a*4+1)||string.IsNullOrEmpty(row.GetCell(nowCellNum+a*4+1).ToString()))
                                {
                                    equipmentDetail.EquipmentName = null;
                                    equipmentDetail.Status = null;
                                    equipmentDetail.Number = 0;
                                    equipmentDetail.Price = 0;
                                }
                                else {
                                    equipmentDetail.EquipmentName = row.GetCell(nowCellNum+a*4+1).ToString();
                                    equipmentDetail.Status = row.GetCell(nowCellNum+a*4+2).ToString();
                                    equipmentDetail.Number = int.Parse(row.GetCell(nowCellNum+a*4+3).ToString());
                                    equipmentDetail.Price = double.Parse(row.GetCell(nowCellNum+a*4+4).ToString());
                                }
                        
                            EquipmentDetails_Equipment.Add(equipmentDetail);
                        }
                        equipmentPart.EquipmentDetails = EquipmentDetails_Equipment;
                        dto.EquipmentPart = equipmentPart;
                        //追溯部分
                        nowCellNum = cellNum_EquipmentTotal;
                        RetrospectPart retrospectPart = new RetrospectPart();
                        List<EquipmentDetail> EquipmentDetails_Retrospect = new List<EquipmentDetail>();
                        for (int a = 0; a<RetrospectNum; a++)
                        {

                            EquipmentDetail equipmentDetail = new EquipmentDetail();
                            if (null==row.GetCell(nowCellNum+a*3+1)||string.IsNullOrEmpty(row.GetCell(nowCellNum+a*3+1).ToString()))
                            {
                                equipmentDetail.EquipmentName = null;
                                equipmentDetail.Number = 0;
                                equipmentDetail.Price = 0;
                            }
                            else {
                                equipmentDetail.EquipmentName = row.GetCell(nowCellNum+a*3+1).ToString();
                                equipmentDetail.Number = int.Parse(row.GetCell(nowCellNum+a*3+2).ToString());
                                equipmentDetail.Price = double.Parse(row.GetCell(nowCellNum+a*3+3).ToString());
                            }
                            EquipmentDetails_Retrospect.Add(equipmentDetail);
                        }
                        retrospectPart.EquipmentDetails=EquipmentDetails_Retrospect;
                        nowCellNum = cellNum_RetrospectTotal;//定位到硬件总价列
                        retrospectPart.HardwareTotal = string.IsNullOrEmpty(row.GetCell(nowCellNum).ToString())?0:   double.Parse(row.GetCell(nowCellNum).ToString());
                        retrospectPart.RetrospectSoftware =string.IsNullOrEmpty(row.GetCell(nowCellNum+1).ToString()) ? null : row.GetCell(nowCellNum+1).ToString();
                        retrospectPart.RetrospectFee = string.IsNullOrEmpty(row.GetCell(nowCellNum+2).ToString()) ? 0 : double.Parse(row.GetCell(nowCellNum+2).ToString());
                        retrospectPart.OpenGraphSoftware = string.IsNullOrEmpty(row.GetCell(nowCellNum+3).ToString()) ? null : row.GetCell(nowCellNum+3).ToString();
                        retrospectPart.OpenGraphFee = string.IsNullOrEmpty(row.GetCell(nowCellNum+4).ToString()) ? 0 : double.Parse(row.GetCell(nowCellNum+4).ToString());
                        retrospectPart.Total = string.IsNullOrEmpty(row.GetCell(nowCellNum+5).ToString()) ? 0 : double.Parse(row.GetCell(nowCellNum+5).ToString());
                        dto.RetrospectPart = retrospectPart;
                        nowCellNum = nowCellNum+5;//定位到工装治具开始前一列
                        /*
                         *工装治具部分
                         */
                        ToolingFixturePart toolingFixturePart = new ToolingFixturePart();
                        List<EquipmentDetail> EquipmentDetails_Tooling = new List<EquipmentDetail>();
                        for (int a = 0; a<ToolingNum; a++)
                        {

                            EquipmentDetail equipmentDetail = new EquipmentDetail();
                            if (null==row.GetCell(nowCellNum+a*3+1)||string.IsNullOrEmpty(row.GetCell(nowCellNum+a*3+1).ToString()))
                            {
                                equipmentDetail.EquipmentName = null;
                                equipmentDetail.Number = 0;
                                equipmentDetail.Price = 0;
                            }
                            else {
                                equipmentDetail.EquipmentName = row.GetCell(nowCellNum+a*3+1).ToString();
                                equipmentDetail.Number = int.Parse(row.GetCell(nowCellNum+a*3+2).ToString());
                                equipmentDetail.Price = double.Parse(row.GetCell(nowCellNum+a*3+3).ToString());
                            }
                            EquipmentDetails_Tooling.Add(equipmentDetail);
                        }
                        toolingFixturePart.EquipmentDetails = EquipmentDetails_Tooling;

                        nowCellNum = cellNum_ToolingName;//定位到工装名称列
                        toolingFixturePart.ToolingName = string.IsNullOrEmpty(row.GetCell(nowCellNum).ToString()) ? null : row.GetCell(nowCellNum).ToString();
                        toolingFixturePart.ToolingNum = string.IsNullOrEmpty(row.GetCell(nowCellNum+1).ToString()) ? 0 : int.Parse(row.GetCell(nowCellNum+1).ToString());
                        toolingFixturePart.ToolingPrice = string.IsNullOrEmpty(row.GetCell(nowCellNum+2).ToString()) ? 0 : double.Parse(row.GetCell(nowCellNum+2).ToString());
                        toolingFixturePart.TestName = string.IsNullOrEmpty(row.GetCell(nowCellNum+3).ToString()) ? null : row.GetCell(nowCellNum+3).ToString();
                        toolingFixturePart.TestNum = string.IsNullOrEmpty(row.GetCell(nowCellNum+4).ToString()) ? 0 : int.Parse(row.GetCell(nowCellNum+4).ToString());
                        toolingFixturePart.TestPrice = string.IsNullOrEmpty(row.GetCell(nowCellNum+5).ToString()) ? 0 : double.Parse(row.GetCell(nowCellNum+5).ToString());
                        toolingFixturePart.Total = string.IsNullOrEmpty(row.GetCell(nowCellNum+6).ToString()) ? 0 : double.Parse(row.GetCell(nowCellNum+6).ToString());
                        dto.ToolingFixturePart=toolingFixturePart;
                        nowCellNum = nowCellNum+6;//定位到sop前一列
                        /*
                         * SOP部分
                         */
                        List<HumanMachineHoursDetail> humanMachineHoursDetailList = new List<HumanMachineHoursDetail>();
                        for (int a = 0; a<SOPNum; a++)
                        {

                            HumanMachineHoursDetail humanMachineHoursDetail = new HumanMachineHoursDetail();
                            humanMachineHoursDetail.Year = yearList[a];
                            if (null==row.GetCell(nowCellNum+a*3+1)||string.IsNullOrEmpty(row.GetCell(nowCellNum+a*3+1).ToString()))
                            {
                                humanMachineHoursDetail.LaborTime = 0;
                                humanMachineHoursDetail.MachineHours = 0;
                                humanMachineHoursDetail.PersonnelNumber = 0;
                            }
                            else {
                                humanMachineHoursDetail.LaborTime = double.Parse(row.GetCell(nowCellNum+a*3+1).ToString());
                                humanMachineHoursDetail.MachineHours = double.Parse(row.GetCell(nowCellNum+a*3+2).ToString());
                                humanMachineHoursDetail.PersonnelNumber = double.Parse(row.GetCell(nowCellNum+a*3+3).ToString());
                            }
                            humanMachineHoursDetailList.Add(humanMachineHoursDetail);
                        }
                        dto.HumanMachineHoursDetailList = humanMachineHoursDetailList;

                        list.Add(dto);
                    }
                    catch {
                        throw new FriendlyException(i+"行"+nowCellNum+"列往后附件数据异常");
                    }

                }
            }
            result.EquipmentNum = EquipmentNum;
            result.RetrospectNum = RetrospectNum;
            result.ToolingNum = ToolingNum;
            result.SOPNum = SOPNum;
            result.WorkingHourDetailList=list;
            result.IsSuccess = true;
            return result;

        }
        /// <summary>
        /// 保存工时工序
        /// </summary>
        /// <param name="workingHourDto"></param>
        /// <returns></returns>
        [ParameterValidator]
        public async Task<WorkingHourDto> SaveWorkingHour(WorkingHourDto workingHourDto)
        {
            WorkingHourDto result = new WorkingHourDto();

            long AuditFlowId = workingHourDto.AuditFlowId;
            long ProductId = workingHourDto.ProductId;
            List<WorkingHourDetail> WorkingHourDetailList = workingHourDto.WorkingHourDetailList;

            //List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(AuditFlowId) && p.ProductId.Equals(ProductId) && p.EnumSole.Equals(WorkingHoursAppService.WorkHours));
            //if (productIsSubmits.Count is not 0)
            //{
            //    throw new FriendlyException(ProductId + ":该零件id工时工序已经提交过了");
            //}
            //else
            {
                long workingHoursInfoId = 0;
                List<WorkingHoursInfo> workingHoursInfos = _workingHoursRepository.GetAllList(s => s.AuditFlowId.Equals(AuditFlowId) && s.ProductId.Equals(ProductId));
                foreach (WorkingHoursInfo workingHoursInfo in workingHoursInfos)
                {
                    workingHoursInfoId = workingHoursInfo.Id;
                    await _yearRepository.HardDeleteAsync(s => s.WorkHoursId.Equals(workingHoursInfoId) && s.Part.Equals(YearPart.WorkingHour));
                    await _equipmentInfoRepository.HardDeleteAsync(s => s.WorkHoursId.Equals(workingHoursInfoId));
                    await _workingHoursRepository.HardDeleteAsync(s => s.Id.Equals(workingHoursInfo.Id));
                }
                foreach (WorkingHourDetail workingHourDetail in WorkingHourDetailList)
                {
                    // 序号
                    int SequenceNumber = workingHourDetail.SequenceNumber;
                    // 工序
                    string Procedure = workingHourDetail.Procedure;
                    // 设备部分
                    EquipmentPart EquipmentPart = workingHourDetail.EquipmentPart;
                    // 追溯部分（硬件及软件开发费用）
                    RetrospectPart RetrospectPart = workingHourDetail.RetrospectPart;
                    // 工装治具部分
                    ToolingFixturePart ToolingFixturePart = workingHourDetail.ToolingFixturePart;
                    // 工时
                    List<HumanMachineHoursDetail> HumanMachineHoursDetailList = workingHourDetail.HumanMachineHoursDetailList;
                    WorkingHoursInfo workingHoursInfo = new WorkingHoursInfo();
                    List<EquipmentInfo> allEquipmentInfoList = new List<EquipmentInfo>();

                    workingHoursInfo.AuditFlowId = AuditFlowId;
                    workingHoursInfo.ProductId = ProductId;
                    workingHoursInfo.IdNumber = SequenceNumber;
                    workingHoursInfo.Procedure = Procedure;
                    //设备部分
                    workingHoursInfo.EquipmentTotalPrice = (decimal)EquipmentPart.Total;
                    List<EquipmentDetail> EquipmentList_Equipment = EquipmentPart.EquipmentDetails;
                    allEquipmentInfoList = GetAllEquipmentInfoList(allEquipmentInfoList, EquipmentList_Equipment, 0);

                    //追溯部分
                    workingHoursInfo.HardwareTotalPrice = (decimal)RetrospectPart.HardwareTotal;
                    workingHoursInfo.TraceabilitySoftware = RetrospectPart.RetrospectSoftware;
                    workingHoursInfo.TraceabilityDevelopmentFee = (decimal)RetrospectPart.RetrospectFee;
                    workingHoursInfo.MappingSoftware = RetrospectPart.OpenGraphSoftware;
                    workingHoursInfo.MappingDevelopmentFee = (decimal)RetrospectPart.OpenGraphFee;
                    workingHoursInfo.SoftwareAndHardwareTotalPrice = (decimal)RetrospectPart.Total;

                    List<EquipmentDetail> EquipmentList_Retrospect = RetrospectPart.EquipmentDetails;
                    allEquipmentInfoList = GetAllEquipmentInfoList(allEquipmentInfoList, EquipmentList_Retrospect, 1);
                    //工装治具
                    workingHoursInfo.ToolingName = ToolingFixturePart.ToolingName;
                    workingHoursInfo.ToolingNum = ToolingFixturePart.ToolingNum;
                    workingHoursInfo.ToolingPrice = (decimal)ToolingFixturePart.ToolingPrice;
                    workingHoursInfo.TestName = ToolingFixturePart.TestName;
                    workingHoursInfo.TestNum = ToolingFixturePart.TestNum;
                    workingHoursInfo.TestPrice = (decimal)ToolingFixturePart.TestPrice;
                    workingHoursInfo.TotalPriceOfToolingAndFixtures = (decimal)ToolingFixturePart.Total;
                    List<EquipmentDetail> EquipmentList_Tooling = ToolingFixturePart.EquipmentDetails;
                    allEquipmentInfoList = GetAllEquipmentInfoList(allEquipmentInfoList, EquipmentList_Tooling, 2);

                    //WorkingHoursInfo数据存储
                    long workingHourId = _workingHoursRepository.InsertAndGetId(workingHoursInfo);
                    //将工时工序静态字段表的ID放置到设备表进行关联
                    foreach (EquipmentInfo eq in allEquipmentInfoList)
                    {
                        eq.WorkHoursId = workingHourId;
                    }
                    //存储设备信息
                    for (int i = 0; i < allEquipmentInfoList.Count; i++)
                    {
                        EquipmentInfo eqInfo = allEquipmentInfoList[i];
                        if (eqInfo.Number > 0)
                        {
                            _equipmentInfoRepository.Insert(allEquipmentInfoList[i]);
                        }
                    }
                    //处理工时等信息
                    List<YearInfo> yearInfoList = new List<YearInfo>();
                    //获取年份数据
                    List<int> yearList = new List<int>();
                    List<Pcs> data = await _pcsRepository.GetAll().Where(p => AuditFlowId.Equals(p.AuditFlowId)).ToListAsync();
                    if (data.Count > 0)
                    {
                        List<PcsYear> pcsYears = await _pcsYearRepository.GetAllListAsync(p => p.PcsId == data.FirstOrDefault().Id);
                        if (pcsYears.Count > 0)
                        {
                            foreach (PcsYear pcsYear in pcsYears)
                            {
                                yearList.Add(pcsYear.Year);
                            }
                        }

                    }
                    if (yearList.Count != HumanMachineHoursDetailList.Count)
                    {
                        string yearString = string.Join(",", yearList);
                        result.IsSuccess = false;
                        result.Message = "PCS部分填录的年份为：" + yearString + "，请检查是否和表格中填录相同！";
                        return result;
                    }
                    else if (yearList.Count == HumanMachineHoursDetailList.Count)
                    {
                        for (int i = 0; i < HumanMachineHoursDetailList.Count; i++)
                        {

                            HumanMachineHoursDetail hmDetail = HumanMachineHoursDetailList[i];
                            YearInfo yearInfo = new YearInfo();
                            yearInfo.Year = yearList[i];
                            yearInfo.WorkHoursId = workingHourId;
                            yearInfo.StandardLaborHours = hmDetail.LaborTime;
                            yearInfo.StandardMachineHours = hmDetail.MachineHours;
                            yearInfo.PersonCount = hmDetail.PersonnelNumber;
                            yearInfo.Part = YearPart.WorkingHour;
                            yearInfo.AuditFlowId = AuditFlowId;
                            yearInfo.ProductId = ProductId;
                            yearInfoList.Add(yearInfo);

                        }
                    }
                    for (int i = 0; i < yearInfoList.Count; i++)
                    {
                        _yearRepository.Insert(yearInfoList[i]);
                    }
                }

                #region 录入完成之后
                List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(AuditFlowId) && p.SolutionId.Equals(ProductId) && p.EnumSole.Equals(WorkingHoursAppService.WorkHours));
                if (productIsSubmits.Count is not 0)
                {
                    await _productIsSubmit.UpdateAsync(productIsSubmits.FirstOrDefault());
                }
                else
                {
                    await _productIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = AuditFlowId, SolutionId = ProductId, EnumSole = WorkingHoursAppService.WorkHours });
                }
                #endregion
            }
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 获取工时工序
        /// </summary>
        /// <param name="workingHourDto"></param>
        /// <returns></returns>
        public async Task<WorkingHourDto> GetWorkingHour(WorkingHourDto workingHourDto)
        {
            WorkingHourDto result = new WorkingHourDto();
            long AuditFlowId = workingHourDto.AuditFlowId;
            long ProductId = workingHourDto.ProductId;
            List<WorkingHoursInfo> workingHourList = _workingHoursRepository.GetAllList(s => s.AuditFlowId.Equals(AuditFlowId)&&s.ProductId.Equals(ProductId));
            List<WorkingHourDetail> WorkingHourDetailList = new List<WorkingHourDetail>();
            if (null!=workingHourList&&workingHourList.Count>0) {
                for (int i=0;i<workingHourList.Count;i++) {
                    WorkingHoursInfo workingHoursInfo = workingHourList[i];
                    long WorkHoursId = workingHoursInfo.Id;

                    List<EquipmentInfo> equipmentInfoList = _equipmentInfoRepository.GetAllList(s => s.WorkHoursId.Equals(WorkHoursId));

                    WorkingHourDetail workingHourDetail = new WorkingHourDetail();


                    // 序号
                    workingHourDetail.SequenceNumber = workingHoursInfo.IdNumber;
                    // 工序
                    workingHourDetail.Procedure= workingHoursInfo.Procedure;

                    // 设备部分
                    EquipmentPart EquipmentPart = new EquipmentPart();
                    EquipmentPart.Total = (double)workingHoursInfo.EquipmentTotalPrice;

                    // 追溯部分（硬件及软件开发费用）
                    RetrospectPart RetrospectPart = new RetrospectPart();
                    RetrospectPart.HardwareTotal = (double)workingHoursInfo.HardwareTotalPrice;//硬件总价
                    RetrospectPart.RetrospectSoftware = workingHoursInfo.TraceabilitySoftware;//追溯软件
                    RetrospectPart.RetrospectFee = (double)workingHoursInfo.TraceabilityDevelopmentFee;//开发费-追溯
                    RetrospectPart.OpenGraphSoftware = workingHoursInfo.MappingSoftware;//开发费-追溯
                    RetrospectPart.OpenGraphFee = (double)workingHoursInfo.MappingDevelopmentFee;//开发费-开图
                    RetrospectPart.Total = (double)workingHoursInfo.SoftwareAndHardwareTotalPrice;// 软硬件总价


                    // 工装治具部分
                    ToolingFixturePart ToolingFixturePart = new ToolingFixturePart ();
                    ToolingFixturePart.ToolingName = workingHoursInfo.ToolingName;//工装名称
                    ToolingFixturePart.ToolingNum = workingHoursInfo.ToolingNum;//工装数量
                    ToolingFixturePart.ToolingPrice = (double)workingHoursInfo.ToolingPrice;//工装单价
                    ToolingFixturePart.TestName = workingHoursInfo.TestName;//测试线名称
                    ToolingFixturePart.TestNum = workingHoursInfo.TestNum; //测试线数量
                    ToolingFixturePart.TestPrice = (double)workingHoursInfo.TestPrice;//测试线单价
                    ToolingFixturePart.Total = (double)workingHoursInfo.TotalPriceOfToolingAndFixtures;

                    List<EquipmentDetail> EquipmentList_Equipment = new List<EquipmentDetail> ();
                    List<EquipmentDetail> EquipmentList_Retrospect = new List<EquipmentDetail>();
                    List<EquipmentDetail> EquipmentList_Tooling = new List<EquipmentDetail>();

                    if (null!=equipmentInfoList&&equipmentInfoList.Count>0) {
                        for (int j = 0; j<equipmentInfoList.Count; j++)
                        {
                            EquipmentInfo equipmentInfo = equipmentInfoList[j];
                            EquipmentDetail equipmentDetail = new EquipmentDetail ();

                            equipmentDetail.EquipmentName = equipmentInfo.EquipmentName;
                            equipmentDetail.Number = equipmentInfo.Number;
                            equipmentDetail.Price = (double)equipmentInfo.UnitPrice;


                            if (Part.Equipment==equipmentInfo.Part)
                            {
                                equipmentDetail.Status = equipmentInfo.Status;
                                EquipmentList_Equipment.Add(equipmentDetail);
                            }
                            else if (Part.Trace==equipmentInfo.Part)
                            {
                                EquipmentList_Retrospect.Add(equipmentDetail);
                            }
                            else if (Part.Fixture==equipmentInfo.Part)
                            {
                                EquipmentList_Tooling.Add(equipmentDetail);
                            }
                        }
                    }
                    EquipmentPart.EquipmentDetails = EquipmentList_Equipment;
                    RetrospectPart.EquipmentDetails = EquipmentList_Retrospect;
                    ToolingFixturePart.EquipmentDetails = EquipmentList_Tooling;

                    workingHourDetail.EquipmentPart = EquipmentPart;
                    workingHourDetail.RetrospectPart = RetrospectPart;
                    workingHourDetail.ToolingFixturePart = ToolingFixturePart;

                    // 工时
                    List<YearInfo> yearInfoList = _yearRepository.GetAllList(s => s.WorkHoursId.Equals(WorkHoursId));
                    List<HumanMachineHoursDetail> HumanMachineHoursDetailList = new List<HumanMachineHoursDetail>();
                    if (null!=yearInfoList&&yearInfoList.Count>0) {
                        for (int m = 0; m<yearInfoList.Count; m++) {
                            YearInfo yearInfo = yearInfoList[m];
                            HumanMachineHoursDetail humanMachineHoursDetail = new HumanMachineHoursDetail();
                            humanMachineHoursDetail.Year = yearInfo.Year;
                            humanMachineHoursDetail.MachineHours = yearInfo.StandardMachineHours;
                            humanMachineHoursDetail.LaborTime = yearInfo.StandardLaborHours;
                            humanMachineHoursDetail.PersonnelNumber = yearInfo.PersonCount;
                            HumanMachineHoursDetailList.Add(humanMachineHoursDetail);
                        }
                        workingHourDetail.HumanMachineHoursDetailList = HumanMachineHoursDetailList;
                    }
                   
                    WorkingHourDetailList.Add(workingHourDetail);
                }
            }


            result.WorkingHourDetailList = WorkingHourDetailList;
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// 保存切线工时
        /// </summary>
        /// <param name="tangentSaveDto"></param>
        /// <returns></returns>
        [ParameterValidator]
        public async Task<TangentSaveDto> SaveTangentHours(TangentSaveDto tangentSaveDto)
        {
            TangentSaveDto result = new TangentSaveDto();

            //long auditFlowId = _yearRepository.GetAll()
            long AuditFlowId = tangentSaveDto.AuditFlowId;
            long ProductId = tangentSaveDto.ProductId;
            List<TangentHoursDetail> tangentHoursDetailList = tangentSaveDto.TangentHoursDetailList;

            //List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(AuditFlowId) && p.ProductId.Equals(ProductId) && p.EnumSole.Equals(WorkingHoursAppService.TangentHours));
            //if (productIsSubmits.Count is not 0)
            //{
            //    throw new FriendlyException(ProductId + ":该零件id切线工序已经提交过了");
            //}
            //else
            {
                List<YearInfo> yearInfo1 = _yearRepository.GetAllList(s => s.AuditFlowId.Equals(AuditFlowId) && s.ProductId.Equals(ProductId));
                if (null != yearInfo1 && yearInfo1.Count > 0)
                {
                    await _yearRepository.HardDeleteAsync(s => s.AuditFlowId.Equals(AuditFlowId) && s.ProductId.Equals(ProductId) && s.Part.Equals(YearPart.SwitchLine));
                    await _uphRepository.HardDeleteAsync(s => s.AuditFlowId.Equals(AuditFlowId) && s.ProductId.Equals(ProductId));
                }

                //处理工时等信息
                List<YearInfo> yearInfoList = new List<YearInfo>();
                for (int i = 0; i < tangentHoursDetailList.Count; i++)
                {
                    TangentHoursDetail thDetail = tangentHoursDetailList[i];
                    YearInfo yearInfo = new YearInfo();
                    yearInfo.Year = thDetail.year;
                    yearInfo.StandardLaborHours = thDetail.LaborTime;
                    yearInfo.StandardMachineHours = thDetail.MachineHours;
                    yearInfo.PersonCount = thDetail.PersonnelNumber;
                    yearInfo.Part = YearPart.SwitchLine;
                    yearInfo.AuditFlowId = tangentSaveDto.AuditFlowId;
                    yearInfo.ProductId = tangentSaveDto.ProductId;
                    yearInfoList.Add(yearInfo);
                }
                //保存年份的工时信息
                for (int i = 0; i < yearInfoList.Count; i++)
                {
                    _yearRepository.Insert(yearInfoList[i]);
                }

                //保存UPH
                decimal uph = tangentSaveDto.UPH;

                UPHInfo uPHInfo = new UPHInfo();
                uPHInfo.AuditFlowId = tangentSaveDto.AuditFlowId;
                uPHInfo.ProductId = tangentSaveDto.ProductId;
                uPHInfo.UPH = uph;
                _uphRepository.Insert(uPHInfo);

                #region 录入完成之后

                List<NreIsSubmit> productIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(AuditFlowId) && p.SolutionId.Equals(ProductId) && p.EnumSole.Equals(WorkingHoursAppService.TangentHours));
                if (productIsSubmits.Count is not 0)
                {
                    await _productIsSubmit.UpdateAsync(productIsSubmits.FirstOrDefault());
                }
                else
                {
                    await _productIsSubmit.InsertAsync(new NreIsSubmit() { AuditFlowId = AuditFlowId, SolutionId = ProductId, EnumSole = WorkingHoursAppService.TangentHours });
                }
                #endregion
            }
            result.IsSuccess = true;
            return result;

        }
        /// <summary>
        /// 获取切线工时
        /// </summary>
        /// <param name="tangentGetDto"></param>
        /// <returns></returns>
        public async Task<TangentSaveDto> GetTangentHoursList(TangentGetDto tangentGetDto)
        {
            
            List<YearInfo> yearInfoList = await _yearRepository.GetAll().Where(p => tangentGetDto.AuditFlowId.Equals(p.AuditFlowId)&&tangentGetDto.ProductId.Equals(p.ProductId)&&p.Part==YearPart.SwitchLine).ToListAsync();

            if (null != yearInfoList && yearInfoList.Count>0)
            {
                TangentSaveDto result = new TangentSaveDto();
                List<TangentHoursDetail> tangentHoursDetailsList = new List<TangentHoursDetail>();
                for (int i = 0; i<yearInfoList.Count; i++)
                {
                    YearInfo yearInfo = yearInfoList[i];
                    TangentHoursDetail tangentHoursDetail = new TangentHoursDetail();
                    tangentHoursDetail.year= yearInfo.Year;
                    tangentHoursDetail.LaborTime=yearInfo.StandardLaborHours;
                    tangentHoursDetail.MachineHours=yearInfo.StandardMachineHours;
                    tangentHoursDetail.PersonnelNumber=yearInfo.PersonCount;
                    tangentHoursDetailsList.Add(tangentHoursDetail);
                }

                //查询UPH
                UPHInfo uPHInfo = _uphRepository.GetAll().Where(p => tangentGetDto.AuditFlowId.Equals(p.AuditFlowId)&&tangentGetDto.ProductId.Equals(p.ProductId)).FirstOrDefault();
                if (uPHInfo!=null)
                {
                    result.UPH = uPHInfo.UPH;
                }

                result.TangentHoursDetailList = tangentHoursDetailsList;
                result.IsSuccess=true;
                return result;
            }
            else
            {
                long AuditFlowId = tangentGetDto.AuditFlowId;
                long ProductId = tangentGetDto.ProductId;
                //获取年份数据
                List<int> yearList = new List<int>();
                List<Pcs> data = await _pcsRepository.GetAll().Where(p => AuditFlowId.Equals(p.AuditFlowId)).ToListAsync();
                if (data.Count > 0)
                {
                    List<PcsYear> pcsYears = await _pcsYearRepository.GetAllListAsync(p => p.PcsId == data.FirstOrDefault().Id);
                    if (pcsYears.Count > 0)
                    {
                        foreach (PcsYear pcsYear in pcsYears)
                        {
                            yearList.Add(pcsYear.Year);
                        }
                    }
                }

                List<ManufacturingCostInfo> manufacturingCostInfoList = new List<ManufacturingCostInfo>();
                manufacturingCostInfoList = await _manufacturingCostInfo.GetAll().ToListAsync();

                List<TangentHoursDetail> tangentHoursDetailList = new List<TangentHoursDetail>();
                for (int i = 0; i<yearList.Count; i++)
                {
                    int year1 = yearList[i];
                    TangentHoursDetail tangentHoursDetail = new TangentHoursDetail();
                    tangentHoursDetail.year = year1;
                    bool ifContain = false;
                    for (int j = 0; j<manufacturingCostInfoList.Count; j++)
                    {
                        int year2 = manufacturingCostInfoList[j].Year;
                        if (year1==year2)
                        {
                            tangentHoursDetail.PersonnelNumber = manufacturingCostInfoList[j].TraceLineOfPerson;
                            ifContain = true;
                        }
                    }
                    if (!ifContain)
                    {
                        tangentHoursDetail.PersonnelNumber = manufacturingCostInfoList.Last().TraceLineOfPerson;
                    }
                    tangentHoursDetailList.Add(tangentHoursDetail);

                }

                //查询UPH
                UPHInfo uPHInfo = _uphRepository.GetAll().Where(p => AuditFlowId.Equals(p.AuditFlowId)&&tangentGetDto.ProductId.Equals(p.ProductId)).FirstOrDefault();

                TangentSaveDto result = new TangentSaveDto();
                if (uPHInfo!=null) {
                    result.UPH = uPHInfo.UPH;
                }

               
                result.TangentHoursDetailList = tangentHoursDetailList;
                result.IsSuccess=true;
                return result;
            }

        }

        public static List<EquipmentInfo> GetAllEquipmentInfoList(List<EquipmentInfo> allEquipmentInfoList,List<EquipmentDetail> detailList,byte part)
        {
            foreach (EquipmentDetail equipment in detailList)
            {
                EquipmentInfo equipmentInfo = new EquipmentInfo();
                equipmentInfo.Status = equipment.Status;
                equipmentInfo.Number = equipment.Number;
                equipmentInfo.EquipmentName = equipment.EquipmentName;
                equipmentInfo.UnitPrice = (decimal)equipment.Price;
                equipmentInfo.Part = (Part)part;
                allEquipmentInfoList.Add(equipmentInfo);
            }
            return allEquipmentInfoList;
        }

        /// <summary>
        /// 工序工时及切线工时界面提交
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnDto> SubmitWorkingHourAndSwitchLine(AuditFlowIdDto auditFlowIdDto)
        {
            long auditFlowId = long.Parse(auditFlowIdDto.AuditFlowId);
            //查询核价需求导入时的零件信息
            var productIds = await _modelCountRepository.GetAllListAsync(p => p.AuditFlowId == auditFlowId);
            //查询已保存的零件工序工时信息
            List<NreIsSubmit> allWorkHoursProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(WorkingHoursAppService.WorkHours));
            List<long> workHoursProductIdList = (from a in allWorkHoursProductIsSubmits.Select(p => p.SolutionId).Distinct() select a).ToList();

            //查询已保存的零件切线工时信息
            List<NreIsSubmit> allTangentHoursProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(auditFlowId) && p.EnumSole.Equals(WorkingHoursAppService.TangentHours));
            List<long> tangentHoursProductIdList = (from a in allTangentHoursProductIsSubmits.Select(p => p.SolutionId).Distinct() select a).ToList();
            //当前已保存的零件数目等于 核价需求导入时的零件数目
            if (productIds.Count == workHoursProductIdList.Count && productIds.Count == tangentHoursProductIdList.Count)
            {
                //执行跳转
                if (AbpSession.UserId is null)
                {
                    throw new FriendlyException("请先登录");
                }

                ReturnDto retDto = await _flowAppService.UpdateAuditFlowInfo(new Audit.Dto.AuditFlowDetailDto()
                {
                    AuditFlowId = auditFlowId,
                    ProcessIdentifier = AuditFlowConsts.AF_ManHourImport,
                    UserId = AbpSession.UserId.Value,
                    Opinion = OPINIONTYPE.Submit_Agreee,
                });
                return retDto;
            }
            else
            {
                if(productIds.Count != allWorkHoursProductIsSubmits.Count)
                {
                    throw new FriendlyException("请先保存完所有零件工序工时！");
                }
                else if(productIds.Count != allTangentHoursProductIsSubmits.Count)
                {
                    throw new FriendlyException("请先保存完所有零件跟线/换线工时！");
                }
                return null;
            }
        }

        /// <summary>
        /// 工时导入 退回重置状态
        /// </summary>
        /// <returns></returns>
        public async Task ClearWorkHoursState(long Id)
        {
            List<NreIsSubmit> workHoursProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(WorkingHoursAppService.WorkHours));
            foreach (NreIsSubmit item in workHoursProductIsSubmits)
            {
                await _productIsSubmit.HardDeleteAsync(item);
            }
            List<NreIsSubmit> tangentHoursProductIsSubmits = await _productIsSubmit.GetAllListAsync(p => p.AuditFlowId.Equals(Id) && p.EnumSole.Equals(WorkingHoursAppService.TangentHours));
            foreach (NreIsSubmit item in tangentHoursProductIsSubmits)
            {
                await _productIsSubmit.HardDeleteAsync(item);
            }
        }


        /// <summary>
        /// 获取3D爆炸图
        /// </summary>
        /// <param name="auditFlowAndProductIdDto"></param>
        /// <returns></returns>
        public async Task<_3DFileDto> GetPicture3DByAuditFlowId(AuditFlowAndProductIdDto auditFlowAndProductIdDto)
        {

            //PriceEvaluation priceEvaluation = await _priceEvaluationRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowId);
            //return priceEvaluation;
            _3DFileDto _3DFileDto = new();
            try
            {
                var ProductDevelopInputInfo = await _productDevelopmentInputRepository.FirstOrDefaultAsync(p => p.AuditFlowId == auditFlowAndProductIdDto.AuditFlowId && p.ProductId == auditFlowAndProductIdDto.ProductId);
               
                if(ProductDevelopInputInfo != null)
                {
                    long Picture3DFileId = ProductDevelopInputInfo.Picture3DFileId;
                    //long Picture3DFileId = JsonConvert.DeserializeObject<List<long>>(ProductDevelopInputInfo.Picture3DFileId).FirstOrDefault();
                    var fileName = await _fileManagementRepository.GetAllListAsync(p => p.Id == Picture3DFileId);
                    if (fileName.Count > 0)
                    {
                        _3DFileDto.AuditFlowId = auditFlowAndProductIdDto.AuditFlowId;
                        _3DFileDto.ProductId = auditFlowAndProductIdDto.ProductId;
                        _3DFileDto.ThreeDFileName = fileName.FirstOrDefault().Name;
                        _3DFileDto.ThreeDFileId = Picture3DFileId;
                        _3DFileDto.IsSuccess = true;
                        return _3DFileDto;
                    }
                    else
                    {
                        throw new FriendlyException("文件找不到");
                    }
                }
                else
                {
                    throw new FriendlyException("零件信息查询失败！");
                }
            }
            catch (Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }
        }


    }
}
