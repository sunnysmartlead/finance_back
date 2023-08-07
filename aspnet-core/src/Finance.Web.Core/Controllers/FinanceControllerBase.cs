using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Finance.Controllers
{
    public abstract class FinanceControllerBase: AbpController
    {
        protected FinanceControllerBase()
        {
            LocalizationSourceName = FinanceConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
