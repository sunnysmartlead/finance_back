using Finance.Dto;

namespace Finance.BaseLibrary
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GetFoundationHardwaresInput: PagedInputDto
    {
        public string SoftwareName { get; set; }
        public string DeviceName { get; set; }
        public string TraceabilitySoftware { get; set; }
    }
}