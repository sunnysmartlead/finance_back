﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.UpdateLog.Dto
{
    public class VersionsUpdateLogInfoDto
    {
        /// <summary>
        /// 版本号id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNumber { get; set; }
        /// <summary>
        /// 环境标识
        /// </summary>
        public string Identify { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsStart { get; set; }

        public List<UpdateLogInfoDto> children { get; set; }

        public DateTime? CreationTime { get; set; }

    }


   
}
