using Abp.Dependency;
using Finance.Audit;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Job
{

    public static class EmailService  
    {
        public static NoticeEmailInfo emailInfoListL { get; set; }
    }
}
