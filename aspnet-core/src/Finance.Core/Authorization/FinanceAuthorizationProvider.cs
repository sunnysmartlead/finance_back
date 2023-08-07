using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Finance.Authorization
{
    public class FinanceAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

            //context.CreatePermission(PermissionNames.Pages_1, L(PermissionNames.Pages_1));
            //context.CreatePermission(PermissionNames.Pages_2, L(PermissionNames.Pages_2));
            context.CreatePermission(PermissionNames.Pages_3, L(PermissionNames.Pages_3));
            context.CreatePermission(PermissionNames.Pages_4, L(PermissionNames.Pages_4));
            context.CreatePermission(PermissionNames.Pages_5, L(PermissionNames.Pages_5));
            context.CreatePermission(PermissionNames.Pages_6, L(PermissionNames.Pages_6));
            context.CreatePermission(PermissionNames.Pages_7, L(PermissionNames.Pages_7));
            context.CreatePermission(PermissionNames.Pages_8, L(PermissionNames.Pages_8));
            context.CreatePermission(PermissionNames.Pages_9, L(PermissionNames.Pages_9));
            context.CreatePermission(PermissionNames.Pages_10, L(PermissionNames.Pages_10));
            context.CreatePermission(PermissionNames.Pages_11, L(PermissionNames.Pages_11));
            context.CreatePermission(PermissionNames.Pages_12, L(PermissionNames.Pages_12));

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, FinanceConsts.LocalizationSourceName);
        }
    }
}
