using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;
using Abp.Application.Services.Dto;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 修改项查询返回
    /// </summary>
    public class UpdateItemListDto : Material
    {
        /// <summary>
        /// 上传佐证材料
        /// </summary>
        [Required]
        public virtual long File { get; set; }
    }
}
