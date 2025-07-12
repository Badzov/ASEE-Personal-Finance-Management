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
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PfmDbContext>
    {
        public PfmDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PfmDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MSSQLPfmDatabase"));

            return new PfmDbContext(optionsBuilder.Options);
        }
    }
}
