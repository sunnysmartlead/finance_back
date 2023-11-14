using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using Finance.Audit;
using Finance.Authorization;
using Finance.Authorization.Accounts;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;
using Finance.Ext;
using Finance.Hr;
using Finance.Hr.Dto;
using Finance.Roles.Dto;
using Finance.Users.Dto;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Finance.Users
{
    /// <summary>
    /// 用户
    /// </summary>
    //[AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;
        private readonly IRepository<Department, long> _departmentRepository;

        //private readonly AuditFlowAppService _auditFlowAppService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="roleRepository"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="abpSession"></param>
        /// <param name="logInManager"></param>
        /// <param name="departmentRepository"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="auditFlowAppService"></param>
        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            LogInManager logInManager, IRepository<Department, long> departmentRepository, IRepository<UserRole, long> userRoleRepository)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;
            _departmentRepository = departmentRepository;
            _userRoleRepository = userRoleRepository;
            //_auditFlowAppService = auditFlowAppService;
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            try
            {
                CheckCreatePermission();

                var user = ObjectMapper.Map<User>(input);
                user.UserName = input.Number.ToString();
                user.Surname = "用户";
                //var guid = Guid.NewGuid();
                //user.EmailAddress = $"{guid:N}@123qwe.com";
                user.TenantId = AbpSession.TenantId;
                user.IsEmailConfirmed = true;

                await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

                CheckErrors(await _userManager.CreateAsync(user, input.Password));

                if (input.RoleNames != null)
                {
                    CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
                }

                CurrentUnitOfWork.SaveChanges();

                return MapToEntityDto(user);
            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }

        /// <summary>
        /// 批量用户导入（Excel文件里的用户信息必须全部合法，才导入。如果有不合法用户，不执行导入，并返回不合法用户信息详情。修改至Excel文件中的用户信息全部合法为止）
        /// </summary>
        /// <param name="excle"></param>
        /// <returns></returns>
        public async virtual Task<ExcelImportResult> ExcelImport([Required] IFormFile excle)
        {
            #region excel校验
            var stream = excle.OpenReadStream();
            var rows = (await MiniExcel.QueryAsync<ExcelImportUserDto>(stream)).ToList();



            //校验职位是否为空
            var notPosition = rows.Where(p => p.Position.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行, 职位为空。");

            //校验部门名称是否为空
            var notDepartmentName = rows.Where(p => p.DepartmentName.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行, 部门名称为空。");

            //校验姓名是否为空
            var notName = rows.Where(p => p.Name.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行, 姓名为空。");

            //校验密码是否为空
            var notPassword = rows.Where(p => p.Password.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行, 密码为空。");

            //校验工号是否为空且是否为数字
            var notNumber = rows.Where(p => !p.Number.IsNumber()).Select(p => $"文件第{rows.IndexOf(p) + 2}行, 工号【{p.Number}】不为数字（数字开头不能为0）。");

            //校验工号是否重复
            var repeatNumber = rows.GroupBy(p => p.Number).Where(p => p.Count() > 1)
                .Select(p => $"工号【{p.Key}】，在第{string.Join("、", p.Select(o => rows.IndexOf(o) + 2))}行同时出现。").ToList();

            //校验部门
            var checkDept = rows.Where(p => !_departmentRepository.GetAll().Select(o => o.Name).Contains(p.DepartmentName))
                .Select(p => $"文件第{rows.IndexOf(p) + 2}行, 部门【{p.DepartmentName}】在系统中不存在。");

            //校验角色是否为空
            var nullRole = rows.Where(p => p.RoleNames.IsNullOrWhiteSpace())
                .Select(p => $"文件第{rows.IndexOf(p) + 2}行，角色组为空。");

            //校验同一行的角色是否重复
            var repeatRoleNames = rows.Where(p => !p.RoleNames.IsNullOrWhiteSpace() && (p.RoleNames.SplitPro(',').GroupBy(o => o).Any(o => o.Count() > 1)))
                  .Select(p => $"文件第{rows.IndexOf(p) + 2}行，角色组有重复的角色。");



            //校验角色在数据库中是否存在
            var checkRole = rows.Where(p => (!p.RoleNames.IsNullOrWhiteSpace()) && (!_roleRepository.GetAll().Select(o => o.Name).Any(o => p.RoleNames.SplitPro(',').Contains(o))))
                .Select(p => $"文件第{rows.IndexOf(p) + 2}行, 角色组【{p.RoleNames}】中有系统中不存在的角色。");


            //校验EmailAddress是否为空
            var notEmailAddress = rows.Where(p => p.EmailAddress.IsNullOrWhiteSpace()).Select(p => $"文件第{rows.IndexOf(p) + 2}行, 邮箱为空。");

            //校验EmailAddress是否合法
            var notEmailAddressAttribute = rows.Where(p => !new EmailAddressAttribute().IsValid(p.EmailAddress)).Select(p => $"文件第{rows.IndexOf(p) + 2}行, 邮箱格式不合法。");

            //校验EmailAddress是否已存在
            var checkEmailAddress = new List<string>();
            if (!notNumber.Any())
            {
                checkEmailAddress = rows.Where(p => _userManager.Users.Where(o => o.Number != p.Number.To<long>()).Select(o => o.EmailAddress).Contains(p.EmailAddress))
                    .Select(p => $"文件第{rows.IndexOf(p) + 2}行, 邮箱【{p.EmailAddress}】在系统中已存在。").ToList();
            }

            var failMessage = notNumber.Union(repeatNumber).Union(checkDept)
                .Union(nullRole).Union(repeatRoleNames).Union(checkRole)
                .Union(notPosition).Union(notDepartmentName).Union(notName).Union(notPassword)
                .Union(notEmailAddress).Union(notEmailAddressAttribute).Union(checkEmailAddress);
            if (failMessage.Any())
            {
                return new ExcelImportResult { Total = rows.Count, IsSuccess = false, Message = $"{string.Join("\r\n", failMessage)}" };
            }
            #endregion

            #region 参数处理
            //填入部门Id
            rows.ForEach(async item =>
            {
                var department = await _departmentRepository.FirstOrDefaultAsync(p => p.Name == item.DepartmentName);
                item.DepartmentId = department.Id;
            });
            #endregion

            #region 导入

            var users = await _userManager.Users.Where(p => rows.Select(o => o.Number.To<long>()).Contains(p.Number)).ToListAsync();

            var userJoin = rows.GroupJoin(users, p => p.Number.To<long>(), p => p.Number, (excelUser, user) =>
              new { excelUser, user });

            //文件和数据库中都存在的用户，更新
            var updateUser = userJoin.Where(p => p.user.Any()).Select(p =>
            {
                var userDto = ObjectMapper.Map<UserDto>(p.excelUser);
                userDto.Id = p.user.FirstOrDefault().Id;
                userDto.IsActive = true;
                //userDto.Password = p.excelUser.Password;
                return new { userDto, p.user };
            }).ToList();

            updateUser.ForEach(async p =>
            {
                //p.userDto.Password = _passwordHasher.HashPassword(p.user.FirstOrDefault(), p.userDto.Password);//更新密码
                await UpdateAsync(p.userDto);
            });

            //文件存在，数据库不存在的用户，创建
            var createUser = userJoin.Where(p => !p.user.Any()).Select(p => ObjectMapper.Map<CreateUserDto>(p.excelUser)).ToList();
            createUser.ForEach(async p => await CreateAsync(p));

            //文件不存在，数据库存在的用户，除Admin外，停用
            var numbers = rows.Select(o => o.Number.To<long>());
            var noUsers = await _userManager.Users
                .Where(p => p.UserName != AbpUserBase.AdminUserName && (!numbers
                .Contains(p.Number))).ToListAsync();
            noUsers.ForEach(p => p.IsActive = false);

            return new ExcelImportResult
            {
                Total = rows.Count,
                InsertTotal = createUser.Count,
                UpdateTotal = updateUser.Count,
                NegativeTotal = noUsers.Count,
                IsSuccess = true,
                Message = $"用户导入成功！文档中共有{rows.Count}条记录，根据文档，新建了{createUser.Count}个用户，更新了{updateUser.Count}个用户，禁用了{noUsers.Count}个用户。"
            };
            #endregion
        }

        /// <summary>
        /// 获取用户导入模板
        /// </summary>
        /// <returns></returns>
        public virtual async Task<FileResult> GetImportTemplate()
        {
            //读取数据
            var department = await _departmentRepository.GetAll().Select(p => p.Name).ToListAsync();
            var roles = await _roleRepository.GetAll().Select(p => p.Name).ToListAsync();

            //打开内存流
            var memoryStream = new MemoryStream();

            //创建Excel工作簿
            var workbook = new XSSFWorkbook();

            //创建用户Sheet
            var sheetUser = workbook.SetSheet("用户",
                 (UserConsts.DepartmentName, UserConsts.DepartmentNameWidth),
                 (UserConsts.Position, UserConsts.PositionWidth),
                 (UserConsts.Number, UserConsts.NumberWidth),
                 (UserConsts.Name, UserConsts.NameWidth),
                 (UserConsts.RoleNames, UserConsts.RoleNamesWidth),
                 (UserConsts.EmailAddress, UserConsts.EmailAddressWidth),
                 (UserConsts.Password, UserConsts.PasswordWidth)
            );

            //创建部门列数据约束
            workbook.SetConstraint(sheetUser, 0, department.ToArray());

            //创建角色列数据约束
            workbook.SetConstraint(sheetUser, 4, roles.ToArray());

            //输出流
            workbook.Write(memoryStream);
            return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = "用户导入模板.xlsx" };
        }

        /// <summary>
        /// 获取用户列表，根据角色Id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<UserDto>> GetUserListByRoleId(GetUserListByRoleIdInput input)
        {
            var users = from u in _userManager.Users
                        join ur in _userRoleRepository.GetAll() on u.Id equals ur.UserId
                        join r in _roleManager.Roles on ur.RoleId equals r.Id
                        where r.Id == input.RoleId
                        select u;
            var count = await users.CountAsync();
            var result = await users.PageBy(input).ToListAsync();
            var dto = ObjectMapper.Map<List<UserDto>>(result);
            return new PagedResultDto<UserDto>(count, dto);
        }

        /// <summary>
        /// 根据角色名获取用户列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<PagedResultDto<UserDto>> GetUserListByRoleName(GetUserListByRoleNameInput input)
        {
            var users = from u in _userManager.Users
                        join ur in _userRoleRepository.GetAll() on u.Id equals ur.UserId
                        join r in _roleManager.Roles on ur.RoleId equals r.Id
                        where r.Name == input.RoleName
                        select u;
            var count = await users.CountAsync();
            var result = await users.PageBy(input).ToListAsync();
            var dto = ObjectMapper.Map<List<UserDto>>(result);
            return new PagedResultDto<UserDto>(count, dto);
        }

        /// <summary>
        /// 根据多个角色名获取用户列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<PagedResultDto<UserDto>> GetUserListByRoleNames(GetUserListByRoleNamesInput input)
        {
            var users = from u in _userManager.Users
                        join ur in _userRoleRepository.GetAll() on u.Id equals ur.UserId
                        join r in _roleManager.Roles on ur.RoleId equals r.Id
                        where input.RoleNames.Contains(r.Name)
                        select u;
            var count = await users.CountAsync();
            var result = await users.PageBy(input).ToListAsync();
            var dto = ObjectMapper.Map<List<UserDto>>(result);
            return new PagedResultDto<UserDto>(count, dto);
        }


        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<UserDto> UpdateAsync(UserDto input)
        {
            try
            {
                CheckUpdatePermission();

                var user = await _userManager.GetUserByIdAsync(input.Id);
                user.UserName = input.Number.ToString();
                user.Surname = "用户";
                //var guid = Guid.NewGuid();
                //user.EmailAddress = $"{guid:N}@123qwe.com";
                //if (!input.Password.IsNullOrWhiteSpace())
                //{
                //    user.Password = input.Password;
                //}
                MapToEntity(input, user);

                CheckErrors(await _userManager.UpdateAsync(user));

                var roleNames = (from a in await _userRoleRepository.GetAllListAsync(p => p.UserId == input.Id)
                                 join r in await _roleRepository.GetAllListAsync() on a.RoleId equals r.Id
                                 select r.Name).ToList();

                if (input.RoleNames != null)
                {
                    var inputRoleNames = input.RoleNames.ToList();
                    if (!roleNames.EqualList(inputRoleNames))
                    {
                        CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
                        //await _auditFlowAppService.DeleteFlowRightByUserId(input.Id);
                    }
                }

                return await GetAsync(input);

            }
            catch (Exception e)
            {
                throw new FriendlyException(e.Message);
            }
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            //await _auditFlowAppService.DeleteFlowRightByUserId(input.Id);
            await _userManager.DeleteAsync(user);
        }
        /// <summary>
        /// 启用某用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
        public async Task Activate(EntityDto<long> user)
        {
            await Repository.UpdateAsync(user.Id, async (entity) =>
            {
                entity.IsActive = true;
            });
        }
        /// <summary>
        /// 禁用某用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
        public async Task DeActivate(EntityDto<long> user)
        {
            await Repository.UpdateAsync(user.Id, async (entity) =>
            {
                entity.IsActive = false;
            });
        }
        /// <summary>
        /// 获取全部的角色
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        /// <summary>
        /// 根据用户Id，获取全部的角色
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<RoleDto>> GetRolesByUserId(long userId)
        {
            var roles = await _userRoleRepository.GetAll().Where(p => p.UserId == userId)
                .Join(_roleRepository.GetAll(), p => p.RoleId, p => p.Id, (a, b) => b).ToListAsync();

            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        /// <summary>
        /// 修改登录用户的语言
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roleIds = user.Roles.Select(x => x.RoleId).ToArray();

            var roles = _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.NormalizedName);

            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();

            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }


        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
        {
            return query.OrderByDescending(r => r.Id);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
        /// <summary>
        /// 修改登录用户的密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ChangePasswordResult> ChangePassword(ChangePasswordDto input)
        {
            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            var user = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                return new ChangePasswordResult { IsSuccess = false, Message = "当前未登录到系统！" };
                //throw new Exception("There is no current user!");
            }

            if (await _userManager.CheckPasswordAsync(user, input.CurrentPassword))
            {
                CheckErrors(await _userManager.ChangePasswordAsync(user, input.NewPassword));
            }
            else
            {
                CheckErrors(IdentityResult.Failed(new IdentityError
                {
                    Description = "Incorrect password."
                }));
            }

            return new ChangePasswordResult { IsSuccess = true, Message = "修改成功！" };
        }
        /// <summary>
        /// 重置密码（管理员进行重置）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task ResetPassword(ResetPasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new FriendlyException("请先登录，然后再尝试重置密码。");
            }

            var currentUser = await _userManager.GetUserByIdAsync(_abpSession.GetUserId());
            var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new FriendlyException("您的“管理员密码”与记录中的不匹配。 请再试一次。");
            }

            if (currentUser.IsDeleted || !currentUser.IsActive)
            {
                throw new FriendlyException("要修改的用户已被删除或未激活。");
            }

            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!roles.Contains(StaticRoleNames.Tenants.Admin))
            {
                throw new FriendlyException("只有管理员可以重置密码！");
            }

            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
    }
}

