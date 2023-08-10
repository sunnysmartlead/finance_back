using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Finance.Authorization.Users;

namespace Finance.Users.Dto
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        /// <summary>
        /// 密码
        /// </summary>
        internal virtual string? Password { get; set; }
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
        ///// 姓氏
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
        ///// <summary>
        ///// 全部名称（姓氏+名称）
        ///// </summary>
        //public string FullName { get; set; }
        /// <summary>
        /// 追进登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 所属角色名称
        /// </summary>
        public string[] RoleNames { get; set; }
    }
}
