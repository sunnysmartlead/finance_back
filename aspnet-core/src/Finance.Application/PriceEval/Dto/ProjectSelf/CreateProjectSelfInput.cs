using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto.ProjectSelf
{
    public class CreateProjectSelfInput
    {
        /// <summary>
        /// 客户
        /// </summary>
        public virtual string Custom { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public virtual string CustomName { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 子项目代码
        /// </summary>
        public virtual string SubCode { get; set; }

        /// <summary>
        /// 子项目描述
        /// </summary>
        public virtual string SubDescription { get; set; }
    }
}
