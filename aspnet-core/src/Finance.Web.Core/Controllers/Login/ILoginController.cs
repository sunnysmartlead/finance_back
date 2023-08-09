using Finance.Login.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Login
{
    /// <summary>
    /// 登录接口
    /// </summary>
    public interface ILoginController
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        Task<LoginResultModel> Login([FromBody] LoginModel loginModel);
    }
}
