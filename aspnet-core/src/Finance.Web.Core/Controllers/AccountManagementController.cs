using Abp.Application.Services;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.UI;
using Finance.Authentication.JwtBearer;
using Finance.Authorization;
using Finance.Authorization.Users;
using Finance.Controllers;
using Finance.Hr;
using Finance.Login.Dto;
using Finance.MultiTenancy;
using Finance.Roles;
using Finance.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Login
{
    /// <summary>
    /// 登录api
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class AccountManagementController : FinanceControllerBase, ILoginController
    {

        private readonly ITenantCache _tenantCache;
        private readonly LogInManager _logInManager;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IUserAppService _userAppService;
        private readonly HrAppService _hrAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logInManager"></param>
        /// <param name="configuration"></param>
        /// <param name="tenantCache"></param>
        /// <param name="abpLoginResultTypeHelper"></param>
        /// <param name="userAppService"></param>
        /// <param name="hrAppService"></param>
        public AccountManagementController(LogInManager logInManager, TokenAuthConfiguration configuration, ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper, IUserAppService userAppService, HrAppService hrAppService)
        {
            this._logInManager = logInManager;
            this._configuration = configuration;

            this._abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            this._tenantCache = tenantCache;

            _userAppService = userAppService;
            _hrAppService = hrAppService;

        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<LoginResultModel> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                if (string.IsNullOrEmpty(loginModel.UserNameOrEmailAddress) || string.IsNullOrEmpty(loginModel.Password) || loginModel.UserNameOrEmailAddress == "null" || loginModel.Password == "null")
                {
                    throw new FriendlyException("账号或密码不能为空！");
                }
                var loginResult = await GetLoginResultAsync(
                loginModel.UserNameOrEmailAddress,
                loginModel.Password,
                GetTenancyNameOrNull());
                var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));

                var d = await _hrAppService.GetDepartmentById(loginResult.User.DepartmentId);

                var c = d == null ? d : await _hrAppService.GetDepartmentById(d.CompanyId);
                var r = await _userAppService.GetRolesByUserId(loginResult.User.Id);
                return new LoginResultModel
                {
                    AccessToken = accessToken,
                    ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                    User = new UserModel
                    {
                        UserId = loginResult.User.Id,
                        UserCompany = c,
                        UserDepartment = d,
                        UserJobs = loginResult.User.Position,
                        UserName = loginResult.User.UserName,
                        UserNumber = loginResult.User.Number,
                        Name = loginResult.User.Name,
                        UserRole = r
                    }
                };
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            };
        }



        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }
        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }
        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }
    }
}
