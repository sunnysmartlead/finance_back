using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Finance.Authorization.Users;

namespace Finance.Users.Dto
{
    /// <summary>
    /// �û���Ϣ
    /// </summary>
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        /// <summary>
        /// ����
        /// </summary>
        internal virtual string? Password { get; set; }
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
        ///// ����
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
        ///// <summary>
        ///// ȫ�����ƣ�����+���ƣ�
        ///// </summary>
        //public string FullName { get; set; }
        /// <summary>
        /// ׷����¼ʱ��
        /// </summary>
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// ������ɫ����
        /// </summary>
        public string[] RoleNames { get; set; }
    }
}
