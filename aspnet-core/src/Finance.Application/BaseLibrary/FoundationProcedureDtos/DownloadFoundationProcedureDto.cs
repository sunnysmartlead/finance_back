using Abp.Application.Services.Dto;
using MiniExcelLibs.Attributes;
using System;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class DownloadFoundationProcedureDto
    {
        [ExcelColumnName("工序编号")]
        public string ProcessNumber { get; set; }
        [ExcelColumnName("工序名称")]
        public string ProcessName { get; set; }
        [ExcelColumnName("工装名")]
        public string InstallationName { get; set; }
        [ExcelColumnName("工装单价")]
        public System.Nullable<System.Decimal> InstallationPrice { get; set; }
        [ExcelColumnName("工装供应商")]
        public string InstallationSupplier { get; set; }
        
        [ExcelColumnName("测试线名称")]
        public string TestName { get; set; }
        [ExcelColumnName("测试线单价")]
        public System.Nullable<System.Decimal> TestPrice { get; set; }
    }
}