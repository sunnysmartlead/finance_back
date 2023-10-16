using Finance.Dto;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetFoundationreliablesInput: PagedInputDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Classification { get; set; }

        /// <summary>
        /// type
        /// </summary>
        public int Type { get; set; }
    }
}