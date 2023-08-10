using Finance.Dto;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetFoundationWorkingHoursInput: PagedInputDto
    {
        public string ProcessName { get; set; }
    }
}