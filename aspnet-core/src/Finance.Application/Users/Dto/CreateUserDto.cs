using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Finance.Authorization.Users;
using MiniExcelLibs.Attributes;

namespace Finance.Users.Dto
{
    /// <summary>
    /// 创建用户需要输入的参数
    /// </summary>
    [AutoMapTo(typeof(User))]
    public class CreateUserDto : IShouldNormalize
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CreateUserDto()
        {
            IsActive = true;
        }
        /// <summary>
        /// 该用户所属部门
        /// </summary>
        public virtual long DepartmentId { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public virtual string Position { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public virtual long Number { get; set; }

        ///// <summary>
        ///// 用户名
        ///// </summary>
        //[Required]
        //[StringLength(AbpUserBase.MaxUserNameLength)]
        //public string UserName { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }
        ///// <summary>
        ///// 姓
        ///// </summary>
        //[Required]
        //[StringLength(AbpUserBase.MaxSurnameLength)]
        //public string Surname { get; set; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 选取的用户角色
        /// </summary>
        public string[] RoleNames { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public void Normalize()
        {
            if (RoleNames == null)
            {
                RoleNames = new string[0];
            }
        }
    }

    /// <summary>
    /// 导入用户需要输入的参数
    /// </summary>
    [AutoMapTo(typeof(User))]
    public class ExcelImportUserDto
    {
        /// <summary>
        /// 该用户Id
        /// </summary>
        [ExcelIgnore]
        public virtual long Id { get; set; }

        /// <summary>
        /// 该用户所属部门
        /// </summary>
        [ExcelIgnore]
        public virtual long DepartmentId { get; set; }

        /// <summary>
        /// 该用户所属部门
        /// </summary>
        [ExcelColumnName(UserConsts.DepartmentName)]
        [ExcelColumnWidth(UserConsts.DepartmentNameWidth)]
        public virtual string DepartmentName { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [ExcelColumnName(UserConsts.Position)]
        [ExcelColumnWidth(UserConsts.PositionWidth)]
        public virtual string Position { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [ExcelColumnName(UserConsts.Number)]
        [ExcelColumnWidth(UserConsts.NumberWidth)]
        public virtual string Number { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        [ExcelColumnName(UserConsts.Name)]
        [ExcelColumnWidth(UserConsts.NameWidth)]
        public string Name { get; set; }
        /// <summary>
        /// 选取的用户角色
        /// </summary>
        [ExcelColumnName(UserConsts.RoleNames)]
        [ExcelColumnWidth(UserConsts.RoleNamesWidth)]
        public string RoleNames { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [ExcelColumnName(UserConsts.EmailAddress)]
        [ExcelColumnWidth(UserConsts.EmailAddressWidth)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        [ExcelColumnName(UserConsts.Password)]
        [ExcelColumnWidth(UserConsts.PasswordWidth)]
        public string Password { get; set; }
    }

}
