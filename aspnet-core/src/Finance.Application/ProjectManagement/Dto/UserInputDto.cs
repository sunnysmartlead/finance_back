using Finance.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.ProjectManagement.Dto
{
    /// <summary>
    /// 核价团队输入Dto
    /// </summary>
    public class UserInputDto: UserInputInfo
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

    }
}