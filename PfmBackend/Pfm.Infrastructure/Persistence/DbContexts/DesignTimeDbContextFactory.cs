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
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PfmDbContext>();

            // Use the same connection string name everywhere
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new PfmDbContext(optionsBuilder.Options);
        }
    }
}
