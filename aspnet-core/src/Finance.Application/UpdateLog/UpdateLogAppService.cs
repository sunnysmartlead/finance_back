using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Finance.FinanceMaintain;
using Finance.Nre;
using Finance.UpdateLog.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Finance.UpdateLog
{
    public class UpdateLogAppService: FinanceAppServiceBase
    {
        /// <summary>
        /// 更新日志表
        /// </summary>
        private readonly IRepository<UpdateLogInfo, long> _resourceUpdateLogInfo;
        /// <summary>
        /// 版本表
        /// </summary>
        private readonly IRepository<Versions, long> _resourceVersions;

        public UpdateLogAppService(IRepository<UpdateLogInfo, long> UpdateLogInfo, IRepository<Versions, long> Versions)
        {
            _resourceUpdateLogInfo = UpdateLogInfo;
            _resourceVersions = Versions;
        }
        /// <summary>
        /// 添加/更新 版本号
        /// </summary>
        /// <returns></returns>
        public async Task AddVersions(VersionsUpdateLogInfoDto versionsDto)
        {
            Versions versions = await _resourceVersions.FirstOrDefaultAsync(p => p.Id.Equals(versionsDto.Id));
            Versions prop = ObjectMapper.Map<Versions>(versionsDto);
            if (versions is null)
            {           
                _resourceVersions.GetDbContext().Set<Versions>().AddRange(prop);
                _resourceVersions.GetDbContext().SaveChanges();
            }
            else
            {
                versions.Identify = prop.Identify;
                versions.IsStart = prop.IsStart;
                versions.VersionNumber = prop.VersionNumber;
                await _resourceVersions.UpdateAsync(versions);
            }            
            List<UpdateLogInfo> upLog= ObjectMapper.Map<List<UpdateLogInfo>>(versionsDto.children);
            await _resourceUpdateLogInfo.HardDeleteAsync(p => p.VersionsId.Equals(prop.Id));
            foreach (UpdateLogInfo updateLogInfo in upLog)
            {          
                updateLogInfo.VersionsId = prop.Id;
                await _resourceUpdateLogInfo.InsertAsync(updateLogInfo);
            }
           
        }
        /// <summary>
        /// 删除版本号
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>        
        [HttpGet]
        public async Task DelUpdateLogInfo(long Id)
        {
            await _resourceVersions.DeleteAsync(Id);
            await _resourceUpdateLogInfo.DeleteAsync(p => p.VersionsId.Equals(Id));
        }    
        /// <summary>
        /// 获取更新日志表
        /// </summary>
        /// <returns></returns>
        public async Task<List<VersionsUpdateLogInfoDto>> GetUpdateLogInfo()
        {
            List<VersionsUpdateLogInfoDto> prop = ObjectMapper.Map<List<VersionsUpdateLogInfoDto>>(await _resourceVersions.GetAllListAsync());
            prop = prop.OrderByDescending(p => p.CreationTime).ToList();
            foreach (var item in prop)
            {
                item.children = ObjectMapper.Map<List<UpdateLogInfoDto>>(await _resourceUpdateLogInfo.GetAllListAsync(p => p.VersionsId.Equals(item.Id)));
            }
            return prop;
        }     
    }
}
