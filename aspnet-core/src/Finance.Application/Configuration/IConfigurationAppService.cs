using System.Threading.Tasks;
using Finance.Configuration.Dto;

namespace Finance.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
