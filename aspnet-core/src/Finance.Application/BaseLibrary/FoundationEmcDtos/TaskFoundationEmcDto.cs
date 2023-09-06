using Abp.Application.Services.Dto;
using System;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 自定义返回
    /// </summary>
    public class TaskFoundationEmcDto 
    {
        public bool Success { get; set; }

        public string Error { get; set; }

    }
}