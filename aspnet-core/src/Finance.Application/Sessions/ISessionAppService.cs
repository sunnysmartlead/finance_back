using System.Threading.Tasks;
using Abp.Application.Services;
using Finance.Sessions.Dto;

namespace Finance.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
