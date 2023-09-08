using Finance.Dto;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetFoundationProceduresInput: PagedInputDto
    {
        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 工装名称
        /// </summary>
        public string InstallationName { get; set; }
    }
}