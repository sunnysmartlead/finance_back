﻿using Finance.Dto;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetFoundationFixtureItemsInput: PagedInputDto
    {
        public string FixtureName { get; set; }
    }
}