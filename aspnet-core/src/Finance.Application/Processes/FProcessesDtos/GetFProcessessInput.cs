using Finance.Dto;

namespace Finance.Processes
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetFProcessessInput: PagedInputDto
    {
        public string ProcessName { get; set; }
    }
}