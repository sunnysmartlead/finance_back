using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Hr.Dto
{
    /// <summary>
    /// 用于返回部门列表
    /// </summary>
    public class DepartmentListDto : FullAuditedEntityDto<long>
    {
        /// <summary>
        /// 该部门所属公司的Id，是本表Department的主键
        /// </summary>
        public virtual long CompanyId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 父部门
        /// </summary>
        public virtual long? Fid { get; set; }

        /// <summary>
        /// 部门等级。根目录是1，根目录的子级是2，子级的子级是3，以此类推
        /// </summary>
        public virtual long Level { get; set; }


        /// <summary>
        /// 用点（.）分隔的路径，部门名称，是根部门，此字段为空。此字段是以Fid计算得到，目的是缩短查询的时间和复杂度。
        /// </summary>
        public virtual string PathName { get; set; }

        /// <summary>
        /// 用点（.）分隔的路径，部门Id，是根部门，此字段为空。此字段是以Fid计算得到，目的是缩短查询的时间和复杂度。
        /// </summary>
        public virtual string PathId { get; set; }

        /// <summary>
        /// 子部门数量
        /// </summary>
        public virtual int ChildCount { get; set; }
    }
}
