using Abp.Domain.Uow;

namespace Finance.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly FinanceDbContext _context;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public InitialHostDbBuilder(FinanceDbContext context, IUnitOfWorkManager unitOfWorkManager)
        {
            _context = context;
            _unitOfWorkManager= unitOfWorkManager;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            new DefaultRolesCreator(_context).Create();

            new DefaultDepartmentCreator(_context).Create();

            


            new DefaultFinanceDictionaryCreator(_context, _unitOfWorkManager).Create();

            new WorkFlowCreator(_context).Create();

            //汇率初始值
            //new DefaultExchangeRateCreator(_context).Create();
#if DEBUG
#else
            new DefaultFlowProcessCreator(_context).Create();
#endif
            _context.SaveChanges();
        }
    }
}
