using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Dto
{
    /// <summary>
    /// 分页Dto
    /// </summary>
    public class PagedInputDto : IPagedResultRequest
    {
        /// <summary>
        /// 每页行数
        /// </summary>
        [Range(1, FinanceConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        /// <summary>
        /// 跳过数量
        /// </summary>
        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        /// <summary>
        /// 当前第几页，下标从0开始,第一页传过来为0
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PagedInputDto()
        {
            MaxResultCount = FinanceConsts.DefaultPageSize;
        }
    }

    /// <summary>
    /// 过滤和分页
    /// </summary>
    public class FilterPagedInputDto : PagedInputDto
    {
        /// <summary>
        /// 查询过滤关键字
        /// </summary>
        public virtual string Filter { get; set; }
    }

    /// <summary>
    /// 分页、排序Dto
    /// </summary>
    public class PagedAndSortedInputDto : PagedInputDto, ISortedResultRequest
    {
        /// <summary>
        /// 排序条件
        /// </summary>
        public string Sorting { get; set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        public PagedAndSortedInputDto()
        {
            MaxResultCount = FinanceConsts.DefaultPageSize;

            Sorting = FinanceConsts.Sorting;
        }
    }
    /// <summary>
    /// 查询和分页
    /// </summary>
    public class QueryPaged : PagedInputDto
    {
        /// <summary>
        /// 动态查询条件
        /// </summary>
        public virtual string Filter { get; set; }

        /// <summary>
        /// 动态查询条件表达式的参数
        /// </summary>
        public virtual object[] Args { get; set; }
    }

    /// <summary>
    /// 查询、分页、排序
    /// </summary>
    public class QueryPagedAndSortedDto : PagedAndSortedInputDto
    {
        /// <summary>
        /// 动态查询条件
        /// </summary>
        public virtual string Filter { get; set; }

        /// <summary>
        /// 动态查询条件表达式的参数
        /// </summary>
        public virtual object[] Args { get; set; }

    }
    /// <summary>
    /// 返回前端错误信息 交互类
    /// </summary>
    public class ResultDto
    {
        /// <summary>
        /// 是否有错误  
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; } = "调用成功";
        /// <summary>
        /// 返回结果
        /// </summary>
        public Object ResultData { get; set; }
    }

}
