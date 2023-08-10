using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Finance.Roles.Dto;
using Finance.Users.Dto;

namespace Finance.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task DeActivate(EntityDto<long> user);
        Task Activate(EntityDto<long> user);
        Task<ListResultDto<RoleDto>> GetRoles();
        Task<ListResultDto<RoleDto>> GetRolesByUserId(long userId);
        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<ChangePasswordResult> ChangePassword(ChangePasswordDto input);
    }
}
