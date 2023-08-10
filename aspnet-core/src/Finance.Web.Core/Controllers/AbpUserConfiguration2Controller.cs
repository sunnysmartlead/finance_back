using Abp.AspNetCore.Mvc.Controllers;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    /// <summary>
    /// AbpUserConfiguration的新版本，保留框架原版。增加返回用户拥有角色权限的信息
    /// </summary>
    public class AbpUserConfiguration2Controller : AbpController
    {
        private readonly AbpUserConfigurationBuilder _abpUserConfigurationBuilder;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionSettingRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        public AbpUserConfiguration2Controller(AbpUserConfigurationBuilder abpUserConfigurationBuilder, IRepository<RolePermissionSetting, long> rolePermissionSettingRepository, IRepository<UserRole, long> userRoleRepository)
        {
            _abpUserConfigurationBuilder = abpUserConfigurationBuilder;
            _rolePermissionSettingRepository = rolePermissionSettingRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<JsonResult> GetAll()
        {
            //联表查询
            var data = from ur in _userRoleRepository.GetAll()
                       join rp in _rolePermissionSettingRepository.GetAll() on ur.RoleId equals rp.RoleId
                       where ur.UserId == AbpSession.UserId && rp.IsGranted == true
                       select rp.Name;
            //读取配置
            var userConfig = await _abpUserConfigurationBuilder.GetAll();

            //查库
            var list = await data.ToListAsync();

            //赋值
            userConfig.Auth.GrantedPermissions = userConfig.Auth.GrantedPermissions.Where(p => list.Contains(p.Key)).ToDictionary(i => i.Key, i => i.Value);
            
            return Json(userConfig);
        }
    }
}
