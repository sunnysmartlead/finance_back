﻿using Finance.Dto;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Finance.Processes
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetProcessHoursEnteritemsInput: PagedInputDto
    {
     
        public int? Type { get; set; }
        public List<FoundationLogsDto> ListFoundationLogs { get; set; }
    }
}