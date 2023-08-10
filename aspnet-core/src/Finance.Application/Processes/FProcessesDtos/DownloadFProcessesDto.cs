using Abp.Application.Services.Dto;
using MiniExcelLibs.Attributes;
using System;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class DownloadFProcessesDto
    {
        [ExcelColumnName("工序编号")]
        public string ProcessNumber { get; set; }
        [ExcelColumnName("工序名称")]
        public string ProcessName { get; set; }

    }
}