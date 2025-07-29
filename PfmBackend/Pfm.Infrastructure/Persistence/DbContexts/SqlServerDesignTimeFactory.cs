using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Pfm.Infrastructure.Persistence.DbContexts
{
    public class SqlServerDesignTimeFactory : IDesignTimeDbContextFactory<PfmSqlServerDbContext>
    {
        public PfmSqlServerDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Pfm.Api"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PfmSqlServerDbContext>();
            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("SqlServer"),
                o => o.MigrationsAssembly("Pfm.Infrastructure")
                      .MigrationsHistoryTable("__EFMigrationsHistory_SqlServer"));

            return new PfmSqlServerDbContext(optionsBuilder.Options);
        }
    }
}
