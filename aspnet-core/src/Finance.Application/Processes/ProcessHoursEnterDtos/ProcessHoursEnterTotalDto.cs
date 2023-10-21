using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Processes.ProcessHoursEnterDtos
{
    public  class ProcessHoursEnterTotalDto
    {
        //硬件总价
        public decimal HardwareTotalPrice { get; set; }
        public decimal SoftwarePrice { get; set; } 
        public decimal TraceabilitySoftware { get; set; } 
    }
}
