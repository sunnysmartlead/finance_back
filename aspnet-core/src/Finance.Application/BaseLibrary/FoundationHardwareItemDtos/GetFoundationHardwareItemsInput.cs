using Finance.Dto;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetFoundationHardwareItemsInput: PagedInputDto
    {
        public string HardwareName { get; set; }
    }
}