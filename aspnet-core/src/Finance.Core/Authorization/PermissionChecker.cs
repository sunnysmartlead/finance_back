using Abp.Authorization;
using Finance.Authorization.Roles;
using Finance.Authorization.Users;

namespace Finance.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
