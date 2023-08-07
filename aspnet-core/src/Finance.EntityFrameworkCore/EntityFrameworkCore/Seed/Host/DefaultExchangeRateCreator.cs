using Finance.FinanceMaintain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EntityFrameworkCore.Seed.Host
{
    public class DefaultExchangeRateCreator
    {
        private readonly FinanceDbContext _context;
        public DefaultExchangeRateCreator(FinanceDbContext context)
        {
            _context = context;
        }
        public void Create()
        {
            CreateEditions();
        }
        private void CreateEditions()
        {
            //仅把ChartPointCountName提取成配置，其他的字符串仅在程序中使用一次，不提取
            var exchangeRateList = new List<ExchangeRate>
            {
                new ExchangeRate { ExchangeRateKind="RMB",ExchangeRateValue=JsonConvert.SerializeObject(new List<YearOrValueMode>{ new YearOrValueMode{ Year=2022, Value=6.8M, }, new YearOrValueMode{ Year=2023, Value=6.7M, },new YearOrValueMode{ Year=2024, Value=6.6M, },new YearOrValueMode{ Year=2025, Value=6.5M, }}) },
                new ExchangeRate {ExchangeRateKind="USD",ExchangeRateValue=JsonConvert.SerializeObject(new List<YearOrValueMode>{ new YearOrValueMode{ Year=2022, Value=6.8M, }, new YearOrValueMode{ Year=2023, Value=6.7M, },new YearOrValueMode{ Year=2024, Value=6.6M, },new YearOrValueMode{ Year=2025, Value=6.5M, }}) },
                new ExchangeRate {ExchangeRateKind="EUR",ExchangeRateValue=JsonConvert.SerializeObject(new List<YearOrValueMode>{ new YearOrValueMode{ Year=2022, Value=6.8M, }, new YearOrValueMode{ Year=2023, Value=6.7M, },new YearOrValueMode{ Year=2024, Value=6.6M, },new YearOrValueMode{ Year=2025, Value=6.5M, }}) },
            };

            var idName = exchangeRateList.Select(o => o.ExchangeRateKind).ToList();

            var query = _context.ExchangeRate.AsQueryable();
            exchangeRateList.ForEach(o => query.Where(p => p.ExchangeRateKind == o.ExchangeRateKind));
            var database = query.Select(o => o.ExchangeRateKind).ToList();

            var dataDetail = idName.Except(database).ToList();
            dataDetail.ForEach(item => _context.ExchangeRate.Add(exchangeRateList.FirstOrDefault(p => p.ExchangeRateKind == item)));
            if (dataDetail.Count != 0)
            {
                _context.SaveChanges();
            }
        }
    }

    /// <summary>
    /// 键值对 一个年份对应一个值 模型（从Finance.Application项目中复制）
    /// </summary>
    public class YearOrValueMode
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public decimal Value { get; set; }
    }
}
