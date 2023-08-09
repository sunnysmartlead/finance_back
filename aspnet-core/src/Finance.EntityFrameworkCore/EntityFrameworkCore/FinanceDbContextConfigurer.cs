using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Finance.EntityFrameworkCore
{
    public static class FinanceDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<FinanceDbContext> builder, string connectionString)
        {
            //builder.UseSqlServer(connectionString);
            builder.UseOracle(connectionString, p => p.UseOracleSQLCompatibility("11"));
        }

        public static void Configure(DbContextOptionsBuilder<FinanceDbContext> builder, DbConnection connection)
        {
            //builder.UseSqlServer(connection);
            builder.UseOracle(connection, p => p.UseOracleSQLCompatibility("11"));
        }
    }
}
