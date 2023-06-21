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
    /// �����û���Ҫ����Ĳ���
    /// </summary>
    [AutoMapTo(typeof(User))]
    public class CreateUserDto : IShouldNormalize
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        public CreateUserDto()
        {
            IsActive = true;
        }
        /// <summary>
        /// ���û���������
        /// </summary>
        public virtual long DepartmentId { get; set; }

        /// <summary>
        /// ְλ
        /// </summary>
        public virtual string Position { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public virtual long Number { get; set; }

        ///// <summary>
        ///// �û���
        ///// </summary>
        //[Required]
        //[StringLength(AbpUserBase.MaxUserNameLength)]
        //public string UserName { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }
        ///// <summary>
        ///// ��
        ///// </summary>
        //[Required]
        //[StringLength(AbpUserBase.MaxSurnameLength)]
        //public string Surname { get; set; }
        /// <summary>
        /// �����ַ
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// ѡȡ���û���ɫ
        /// </summary>
        public string[] RoleNames { get; set; }
        /// <summary>
        /// �û�����
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
    /// �����û���Ҫ����Ĳ���
    /// </summary>
    [AutoMapTo(typeof(User))]
    public class ExcelImportUserDto
    {
        /// <summary>
        /// ���û�Id
        /// </summary>
        [ExcelIgnore]
        public virtual long Id { get; set; }

        /// <summary>
        /// ���û���������
        /// </summary>
        [ExcelIgnore]
        public virtual long DepartmentId { get; set; }

        /// <summary>
        /// ���û���������
        /// </summary>
        [ExcelColumnName(UserConsts.DepartmentName)]
        [ExcelColumnWidth(UserConsts.DepartmentNameWidth)]
        public virtual string DepartmentName { get; set; }

        /// <summary>
        /// ְλ
        /// </summary>
        [ExcelColumnName(UserConsts.Position)]
        [ExcelColumnWidth(UserConsts.PositionWidth)]
        public virtual string Position { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [ExcelColumnName(UserConsts.Number)]
        [ExcelColumnWidth(UserConsts.NumberWidth)]
        public virtual string Number { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        [ExcelColumnName(UserConsts.Name)]
        [ExcelColumnWidth(UserConsts.NameWidth)]
        public string Name { get; set; }
        /// <summary>
        /// ѡȡ���û���ɫ
        /// </summary>
        [ExcelColumnName(UserConsts.RoleNames)]
        [ExcelColumnWidth(UserConsts.RoleNamesWidth)]
        public string RoleNames { get; set; }


        /// <summary>
        /// ����
        /// </summary>
        [ExcelColumnName(UserConsts.EmailAddress)]
        [ExcelColumnWidth(UserConsts.EmailAddressWidth)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// �û�����
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        [ExcelColumnName(UserConsts.Password)]
        [ExcelColumnWidth(UserConsts.PasswordWidth)]
        public string Password { get; set; }
    }

}
