using Finance.PropertyDepartment.DemandApplyAudit.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Dto
{
    public class dto
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        public long NewAuditFlowId { get; set; }
        /// <summary>
        /// 引用流程的流程ID
        /// </summary>
        public long QuoteAuditFlowId { get; set; }
        /// <summary>
        /// 方案ID和引用流程的方案ID
        /// </summary>
        public List<SolutionIdAndQuoteSolutionId> SolutionIdAndQuoteSolutionId { get; set; }
    }
    public class IdMapping
    {
        /// <summary>
        /// 新ID
        /// </summary>
        public long NewId { get; set; }
        /// <summary>
        /// 引用ID
        /// </summary>
        public long QuoteId { get; set; }      
    }
    public class IdMappingList
    {
        /// <summary>
        /// Nre手板件
        /// </summary>
        public List<IdMapping> HandPieceCost { get; set; }
        /// <summary>
        /// 其他
        /// </summary>
        public List<IdMapping> RestsCost { get; set; }
        /// <summary>
        /// 差旅费
        /// </summary>
        public List<IdMapping> TravelExpense { get; set; }
    }
    public class AllIdMappingList: IdMappingList
    {
        /// <summary>
        /// NRE环境实验费
        /// </summary>
        public List<IdMapping> FastPostExperimentItemsSingle { get; set; }
        /// <summary>
        /// NRE模具费
        /// </summary>
        public List<IdMapping> FastPostResourcesManagementSingle { get; set; }
        /// <summary>
        /// NREEMC实验费
        /// </summary>
        public List<IdMapping> FastPostEmcItemsSingle { get; set; }

     
    }
    public class IdMappingListHoursEnter
    {
        /// <summary>
        /// ProcessHoursEnterDevice
        /// </summary>
        public List<IdMapping> ProcessHoursEnterDevice { get; set; }
        /// <summary>
        /// ProcessHoursEnterFixture
        /// </summary>
        public List<IdMapping> ProcessHoursEnterFixture { get; set; }
        /// <summary>
        /// ProcessHoursEnterFrock
        /// </summary>
        public List<IdMapping> ProcessHoursEnterFrock { get; set; }
        /// <summary>
        /// ProcessHoursEnter
        /// </summary>
        public List<IdMapping> ProcessHoursEnter { get; set; }
        /// <summary>
        /// ProcessHoursEnterUph
        /// </summary>
        public List<IdMapping> ProcessHoursEnterUph { get; set; }
        /// <summary>
        /// ProcessHoursEnterLine
        /// </summary>
        public List<IdMapping> ProcessHoursEnterLine { get; set; }
        /// <summary>
        /// ProcessHoursEnteritem
        /// </summary>
        public List<IdMapping> ProcessHoursEnteritem { get; set; }
    }
}
