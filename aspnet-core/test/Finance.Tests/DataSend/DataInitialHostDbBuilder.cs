using Abp.Domain.Uow;
using Finance.EntityFrameworkCore;
using Finance.EntityFrameworkCore.Seed.Host;
using Finance.EntityFrameworkCore.Seed.Tenants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Tests.DataSend
{
    internal class DataInitialHostDbBuilder
    {
        private readonly FinanceDbContext _context;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DataInitialHostDbBuilder(FinanceDbContext context, IUnitOfWorkManager unitOfWorkManager)
        {
            _context = context;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void Create()
        {
            new DefaultCountryLibraryCreator(_context).Create();
            _context.SaveChanges();
        }
    }
}
