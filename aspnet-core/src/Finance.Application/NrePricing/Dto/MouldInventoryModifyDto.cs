﻿using Finance.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Finance.Ext.FriendlyRequiredAttribute;

namespace Finance.NrePricing.Dto
{
    // <summary>
    /// Nre 资源部 模具清单 修改项交互类
    /// </summary>
    public class MouldInventoryModifyDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 流程号Id
        /// </summary> 
        [FriendlyRequired("流程id", SpecialVerification.AuditFlowIdVerification)]
        public long AuditFlowId { get; set; }
        /// <summary>
        /// 方案的id
        /// </summary>
        [FriendlyRequired("方案id", SpecialVerification.SolutionIdVerification)]
        public long SolutionId { get; set; }
        /// <summary>
        /// 修改项的id
        /// </summary>
        public long ModifyId { get; set; }
        /// <summary>
        /// 结构BOM Id
        /// </summary>
        public long StructuralId { get; set; }
        /// <summary>
        /// 模具名称
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// 模穴数
        /// </summary>
        public int MoldCavityCount { get; set; }
        /// <summary>
        /// 模次数
        /// </summary>
        public int ModelNumber { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 提交人 Id
        /// </summary>
        public long PeopleId { get; set; }
        /// <summary>
        /// 是否提交 true/1 提交  false/0 未提交
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
