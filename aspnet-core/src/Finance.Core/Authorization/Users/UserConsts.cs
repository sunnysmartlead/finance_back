using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Authorization.Users
{
    public class UserConsts
    {
        public const string DepartmentName = "部门名称";
        public const string Position = "职位";
        public const string Number = "工号";
        public const string Name = "姓名";
        public const string RoleNames = "选取的用户角色（多个角色用英文逗号分隔）";
        public const string EmailAddress = "邮箱";
        public const string Password = "用户密码";


        public const int DepartmentNameWidth = 20;
        public const int PositionWidth = 20;
        public const int NumberWidth = 20;
        public const int NameWidth = 20;
        public const int RoleNamesWidth = 60;
        public const int PasswordWidth = 20;
        public const int EmailAddressWidth = 20;

        public const int WidthUnit = 8;

    }
}
