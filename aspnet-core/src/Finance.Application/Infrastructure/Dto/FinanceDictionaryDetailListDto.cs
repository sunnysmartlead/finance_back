using Abp.Application.Services.Dto;

namespace Finance.Infrastructure.Dto
{
    /// <summary>
    /// 字典明细列表
    /// </summary>
    public class FinanceDictionaryDetailListDto : FullAuditedEntityDto<string>
    {
        /// <summary>
        /// 排序依据（以此字段倒序排序）
        /// </summary>
        public virtual long Order { get; set; }

        /// <summary>
        /// 字典明细名
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 字典明细显示名
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
