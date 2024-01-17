using Finance.WorkFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.VersionManagement.Dto
{
    public class ResultDto
    {
        public virtual string ProjectName { get; set; }
        public virtual int Version { get; set; }
        public virtual string ProcessName { get; set; }
        public virtual NodeInstanceStatus NodeInstanceStatus { get; set; }
        public virtual DateTime? CreationTime { get; set; }
        public virtual DateTime? LastModificationTime { get; set; }
        public virtual string RoleId { get; set; }
        public virtual long WorkFlowInstanceId { get; set; }
        public virtual string ProcessIdentifier { get; set; }
        public virtual DateTime ResetTime { get; set; }
        public virtual long Id { get; set; }
        public virtual long NodeInstanceId { get; set; }

    }
}
