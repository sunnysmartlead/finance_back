using Abp.Application.Services.Dto;
using MiniExcelLibs.Attributes;
using System;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 列表
    /// </summary>
    public class FoundationreliableExport 
    {
        [ExcelColumnName("分类")]
        public string Classification { get; set; }
        [ExcelColumnName("名称")]
        public string Name { get; set; }
        [ExcelColumnName("价格")]
        public double Price { get; set; }
        [ExcelColumnName("单位")]
        public string Unit { get; set; }
    }
}