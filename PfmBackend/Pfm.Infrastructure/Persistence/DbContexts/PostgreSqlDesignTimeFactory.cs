using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Persistence.DbContexts
{
    public class PostgreSqlDesignTimeFactory : IDesignTimeDbContextFactory<PfmPostgreSqlDbContext>
    {
        public PfmPostgreSqlDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Pfm.Api"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PfmPostgreSqlDbContext>();
            optionsBuilder.UseNpgsql(
                configuration.GetConnectionString("PostgreSql"),
                o => o.MigrationsAssembly("Pfm.Infrastructure")
                      .MigrationsHistoryTable("__EFMigrationsHistory_PostgreSql"));

            return new PfmPostgreSqlDbContext(optionsBuilder.Options);
        }
    }
}
