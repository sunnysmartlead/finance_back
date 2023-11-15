using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Finance.Authorization.Users;
using Finance.Hr.Dto;
using Finance.Users.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Finance.Hr
{
    /// <summary>
    /// 人力资源服务
    /// </summary>
    public class HrAppService : FinanceAppServiceBase
    {
        private readonly IRepository<Department, long> _departmentRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="departmentRepository"></param>
        public HrAppService(IRepository<Department, long> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// 添加新部门
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task AddDepartment(AddDepartment input)
        {
            var d = await _departmentRepository.GetAll().AnyAsync(p => p.Name == input.Name);
            if (d)
            {
                throw new UserFriendlyException("部门名称不可重复，已存在相同名称的部门。");
            }

            //首先以映射转换对象
            var department = ObjectMapper.Map<Department>(input);

            department.Level = 1;//默认层级为1

            //如果Fid有值且非0（不是根节点），就赋予即将新增的记录Path
            if (input.Fid.HasValue && input.Fid.Value is not 0)
            {
                //Path是根据父节点的Path加上它本身。Id用.分隔，Name用/分隔。
                var fDepartment = await _departmentRepository.GetAsync(input.Fid.Value);
                department.Level = fDepartment.Level + 1;
                department.CompanyId = fDepartment.CompanyId;
                department.PathId = string.Format(FinanceConsts.DepartmentPathIdRegular, fDepartment.PathId, input.Fid.Value);
                department.PathName = string.Format(FinanceConsts.DepartmentPathNameRegular, fDepartment.PathName, fDepartment.Name);
                await _departmentRepository.InsertAsync(department);
            }
            else
            {
                //如果是根节点
                var id = await _departmentRepository.InsertAndGetIdAsync(department);
                department.CompanyId = id;
                await _departmentRepository.UpdateAsync(department);
            }
        }

        /// <summary>
        /// 更新部门信息（只变名称）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task UpdateDepartment(UpdateDepartmentInput input)
        {
            var d = await _departmentRepository.GetAll().AnyAsync(p => p.Name == input.Name);
            if (d)
            {
                throw new UserFriendlyException("部门名称不可重复，已存在相同名称的部门。");
            }

            //获取要更新的数据
            var department = await _departmentRepository.GetAsync(input.Id);

            //计算子部门原有的路径
            var oldPath = string.Format(FinanceConsts.DepartmentPathNameRegular, department.PathName, department.Name);

            //计算子部门新路径
            var newPath = string.Format(FinanceConsts.DepartmentPathNameRegular, department.PathName, input.Name);

            //目前只可能会更改名称
            department.Name = input.Name;

            //更新
            await _departmentRepository.UpdateAsync(department);

            //更新子部门的Path
            await _departmentRepository.GetAll()
                .Where(p => p.PathName.StartsWith(oldPath))
                .UpdateAsync(p => new Department
                {
                    //替换首先匹配到的而不是其他情况
                    PathName = Regex.Replace(p.PathName, $"^{oldPath}", newPath)
                });

        }

        /// <summary>
        /// 删除部门信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task DeleteDepartment(EntityDto<long> input)
        {
            //获取要删除的部门
            var department = await _departmentRepository.GetAsync(input.Id);

            //计算子部门原有的路径
            var oldPath = string.Format(FinanceConsts.DepartmentPathIdRegular, department.PathId, department.Id);

            //获得本部门和子部门（要删除的部门）
            var cDepartment = _departmentRepository.GetAll().Where(p => p.Id == input.Id || p.PathId.StartsWith(oldPath));

            await cDepartment.DeleteAsync();
        }

        /// <summary>
        /// 获取根部门
        /// </summary>
        /// <returns></returns>
        public async virtual Task<ListResultDto<DepartmentListDto>> GetRootDepartment()
        {
            var department = _departmentRepository.GetAll().Where(p => p.PathId == null || p.PathId == string.Empty);
            var result = await department.ToListAsync();
            var entity = ObjectMapper.Map<List<DepartmentListDto>>(result);
            foreach (var o in entity)
            {
                //获取本部门
                var department2 = await _departmentRepository.GetAsync(o.Id);

                //计算子部门原有的路径
                var oldPath2 = string.Format(FinanceConsts.DepartmentPathIdRegular, department2.PathId, department2.Id);

                //var count = await _departmentRepository.CountAsync(p => p.PathId.StartsWith(oldPath2));//子部门的数量
                var count = await _departmentRepository.CountAsync(p => p.PathId == oldPath2);//子部门的数量

                o.ChildCount = count;
            }
            return new ListResultDto<DepartmentListDto>(entity);
        }

        /// <summary>
        /// 获取下属部门
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<ListResultDto<DepartmentListDto>> GetSubordinateDepartment(EntityDto<long> input)
        {
            //获取本部门
            var department = await _departmentRepository.GetAsync(input.Id);

            //计算子部门原有的路径
            var oldPath = string.Format(FinanceConsts.DepartmentPathIdRegular, department.PathId, department.Id);

            //获得子部门
            var cDepartment = _departmentRepository.GetAll().Where(p => p.PathId == oldPath);


            var result = await cDepartment.ToListAsync();

            var entity = ObjectMapper.Map<List<DepartmentListDto>>(result);
            foreach (var o in entity)
            {
                //获取本部门
                var department2 = await _departmentRepository.GetAsync(o.Id);

                //计算子部门原有的路径
                var oldPath2 = string.Format(FinanceConsts.DepartmentPathIdRegular, department2.PathId, department2.Id);

                var count = await _departmentRepository.CountAsync(p => p.PathId == oldPath2);//子部门的数量

                o.ChildCount = count;
            }

            return new ListResultDto<DepartmentListDto>(ObjectMapper.Map<List<DepartmentListDto>>(entity));
        }

        /// <summary>
        /// 根据Id获取部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async virtual Task<DepartmentListDto> GetDepartmentById(long id)
        {
            var entity = await _departmentRepository.FirstOrDefaultAsync(id);
            return ObjectMapper.Map<DepartmentListDto>(entity);
        }

        /// <summary>
        /// 根据部门名获取用户
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultDto<UserDto>> GetUserByDeptName(GetUserByDeptNameInput input)
        {
            var data = (from u in UserManager.Users
                        join d in _departmentRepository.GetAll() on u.DepartmentId equals d.Id
                        select new { u, d })
                        .WhereIf(!input.Name.IsNullOrWhiteSpace(), p => p.u.Name.Contains(input.Name))
                        .WhereIf(!input.DeptName.IsNullOrWhiteSpace(), p => p.d.Name.Contains(input.DeptName))
                        .Select(p => p.u);


            var count = await data.CountAsync();
            var paged = await data.PageBy(input).ToListAsync();
            return new PagedResultDto<UserDto>(count, ObjectMapper.Map<List<UserDto>>(paged));

        }
    }
}
