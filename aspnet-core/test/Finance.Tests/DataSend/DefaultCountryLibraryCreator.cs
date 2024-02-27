using Finance.EntityFrameworkCore;
using Finance.Infrastructure;
using Finance.TradeCompliance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Tests.DataSend
{
    internal class DefaultCountryLibraryCreator
    {
        private readonly FinanceDbContext _context;
        public DefaultCountryLibraryCreator(FinanceDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateCountryLibrary();
        }

        private void CreateCountryLibrary()
        {
            var countryLibraryList = new List<CountryLibrary>
            {
                new CountryLibrary { Id = 1, NationalType = "2", Country = "其他国家" },
                new CountryLibrary { Id = 2, NationalType = "1", Country = "伊朗" },
                new CountryLibrary { Id = 3, NationalType = "1", Country = "朝鲜" },
                new CountryLibrary { Id = 4, NationalType = "1", Country = "古巴" },
                new CountryLibrary { Id = 5, NationalType = "1", Country = "叙利亚" },
            };
            var noDb = countryLibraryList.Where(p => !_context.CountryLibrary.Contains(p));
            if (noDb.Any())
            {
                _context.CountryLibrary.AddRange(noDb);
                _context.SaveChanges();
            }
            var isDeleted = _context.CountryLibrary.Where(p => countryLibraryList.Contains(p) && p.IsDeleted).ToList();
            if (isDeleted.Any())
            {
                isDeleted.ForEach(p => p.IsDeleted = false);
                _context.CountryLibrary.UpdateRange(isDeleted);
                _context.SaveChanges();
            }
        }
    }
}
