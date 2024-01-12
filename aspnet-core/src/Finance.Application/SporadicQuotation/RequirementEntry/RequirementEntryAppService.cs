

using Abp.Application.Services;
using Abp.Domain.Repositories;
using Finance.Ext;
using Finance.LXRequirementEntry;
using Finance.SporadicQuotation.RequirementEntry.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.SporadicQuotation.RequirementEntry
{
    /// <summary>
    /// 零星报价的需求录入页面
    /// </summary>
    public class RequirementEntryAppService: ApplicationService
    {
        /// <summary>
        /// 零星报价需求录入表
        /// </summary>
        private readonly IRepository<RequirementEnt, long> _requirementEnt;

        /// <summary>
        /// 数据列表
        /// </summary>
        private readonly IRepository<DataList, long> _dataList;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requirementEnt"></param>
        /// <param name="dataList"></param>
        public RequirementEntryAppService(IRepository<RequirementEnt, long> requirementEnt, IRepository<DataList, long> dataList)
        {
            _requirementEnt= requirementEnt;
            _dataList= dataList;
        }
        /// <summary>
        /// 零星报价需求录入 保存\提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task LXRequirementEnt(LXRequirementEntDto lXRequirementEntDto)
        {
            try
            {
                //保存\修改 需求录入实体类
                RequirementEnt requirementEnt = ObjectMapper.Map<RequirementEnt>(lXRequirementEntDto);
                long id= await _requirementEnt.InsertOrUpdateAndGetIdAsync(requirementEnt);
                lXRequirementEntDto?.LXDataListDtos.Select(p => p.RequirementEntryId = id).ToList();
                //保存\修改 数据列表实体类
                List<DataList> dataLists = ObjectMapper.Map<List<DataList>>(lXRequirementEntDto?.LXDataListDtos);
                await _dataList.BulkInsertOrUpdateAsync(dataLists);
                //流程提交
                if (lXRequirementEntDto.IsSubmit)
                {

                }
            }
            catch (System.Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }           
        }
        /// <summary>
        /// 零星报价 查询需求录入
        /// </summary>
        /// <param name="auditFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<LXRequirementEntDto> QueryLXRequirementEnt(long auditFlowId)
        {
            try
            {

                LXRequirementEntDto lXRequirementEntDto = new();
                RequirementEnt requirementEnt = await _requirementEnt.FirstOrDefaultAsync(p => p.AuditFlowId.Equals(auditFlowId));
                if (requirementEnt is null)
                {
                    lXRequirementEntDto = new();
                }
                else
                {
                    lXRequirementEntDto = ObjectMapper.Map<LXRequirementEntDto>(requirementEnt);
                }             
                List<DataList> dataLists =await _dataList.GetAllListAsync(p=>p.RequirementEntryId.Equals(lXRequirementEntDto.Id));
                List<LXDataListDto> dataList = new();
                if(dataList is null||dataList.Count is 0)
                {
                    dataList = new();
                    List<ListName> strings = new List<ListName>() { ListName.零件名称, ListName.数量, ListName.单价, ListName.单位成本, ListName.销售收入, ListName.销售成本, ListName.销售毛利, ListName.毛利率, ListName.备注 };
                    foreach (var item in strings)
                    {
                        dataList.Add(new LXDataListDto() {ListName= item, ListNameDisplayName = item.ToString(), RequirementEntryId= lXRequirementEntDto.Id, Data = new List<string>() { ""} });
                    }                
                }else
                {
                    dataList=ObjectMapper.Map<List<LXDataListDto>>(dataLists);
                }
                lXRequirementEntDto.LXDataListDtos = dataList;
                return lXRequirementEntDto;
            }
            catch (System.Exception ex)
            {
                throw new FriendlyException(ex.Message);
            }          
        }
    }
}
