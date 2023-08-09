using Abp.Domain.Repositories;
using Finance.Audit;
using Finance.PriceEval.Dto.Timelinesss;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval
{
    /// <summary>
    /// 时效性页面
    /// </summary>
    public class TimelinessAppService : FinanceAppServiceBase
    {
        private readonly IRepository<Timeliness, long> _timelinessRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timelinessRepository"></param>
        public TimelinessAppService(IRepository<Timeliness, long> timelinessRepository)
        {
            _timelinessRepository = timelinessRepository;
        }

        /// <summary>
        /// 写入时效性页面信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task SetTimeliness(SetTimelinessDto input)
        {
            var isHas = await _timelinessRepository.GetAll().AnyAsync(p => p.AuditFlowId == input.AuditFlowId);
            if (isHas)
            {
                var timeliness = await _timelinessRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);
                ObjectMapper.Map(input, timeliness);
            }
            else
            {
                await _timelinessRepository.InsertAsync(ObjectMapper.Map<Timeliness>(input));
            }
        }

        /// <summary>
        /// 读取时效性页面信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async virtual Task<TimelinessDto> GetTimeliness(GetTimelinessDto input)
        {

            var timeliness = await _timelinessRepository.FirstOrDefaultAsync(p => p.AuditFlowId == input.AuditFlowId);
            if (timeliness == null)
            {
                var dto = new TimelinessDto { AuditFlowId = input.AuditFlowId };
                dto.Data = new List<NameValue>
                {
                    new NameValue{ Name="客户",Value=string.Empty },
                    new NameValue{ Name="车型",Value=string.Empty },
                    new NameValue{ Name="产品类型",Value=string.Empty },
                    new NameValue{ Name="Sensor",Value=string.Empty },
                    new NameValue{ Name="Lens",Value=string.Empty },
                    new NameValue{ Name="终端总量",Value=string.Empty },
                    new NameValue{ Name="关键节点",Value=string.Empty },
                    new NameValue{ Name="单目&多目",Value=string.Empty },
                    new NameValue{ Name="Serial",Value=string.Empty },
                    new NameValue{ Name="ISP",Value=string.Empty },
                    new NameValue{ Name="业务",Value=string.Empty },
                    new NameValue{ Name="PM",Value=string.Empty },
                    new NameValue{ Name="IP等级",Value=string.Empty },
                    new NameValue{ Name="Vcsel/LED",Value=string.Empty },
                    new NameValue{ Name="MCU",Value=string.Empty },
                    new NameValue{ Name="结构",Value=string.Empty },
                    new NameValue{ Name="电子",Value=string.Empty },
                    new NameValue{ Name="NRE报价策略",Value=string.Empty },
                    new NameValue{ Name="线束",Value=string.Empty },
                    new NameValue{ Name="其他",Value=string.Empty },
                    new NameValue{ Name="工艺",Value=string.Empty },
                    new NameValue{ Name="IE",Value=string.Empty },
                    new NameValue{ Name="客户目标价",Value=string.Empty },
                    new NameValue{ Name="包装方案",Value=string.Empty },
                    new NameValue{ Name="工艺方案",Value=string.Empty },
                    new NameValue{ Name="治具",Value=string.Empty },
                    new NameValue{ Name="包装",Value=string.Empty },
                    new NameValue{ Name="报价策略",Value=string.Empty },
                    new NameValue{ Name="竞争对手",Value=string.Empty },
                    new NameValue{ Name="可靠性",Value=string.Empty },
                    new NameValue{ Name="QE",Value=string.Empty },
                    new NameValue{ Name="月产能",Value=string.Empty },
                    new NameValue{ Name="其他",Value=string.Empty },
                };
                return dto;
            }
            else
            {
                return ObjectMapper.Map<TimelinessDto>(timeliness);
            }
        }
    }
}
