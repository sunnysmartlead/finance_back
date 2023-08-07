using System.Threading.Tasks;
using Abp.Application.Services;
using Finance.Authorization.Accounts.Dto;

namespace Finance.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
