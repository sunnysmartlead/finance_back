﻿using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Nre
{
    /// <summary>
    /// 差旅费 修改项 实体类
    /// </summary>
    public class TravelExpenseModify : FullAuditedEntity<long>
    {
        /// <summary>
        /// 修改项的id
        /// </summary>
        public long ModifyId { get; set; }
        /// <summary>
        /// 事由外键
        /// </summary>
        public string ReasonsId { get; set; }      
        /// <summary>
        /// 人数
        /// </summary>
        public int PeopleCount { get; set; }
        /// <summary>
        /// 费用/天
        /// </summary>
        public decimal CostSky { get; set; }
        /// <summary>
        /// 天数
        /// </summary>
        public int SkyCount { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
