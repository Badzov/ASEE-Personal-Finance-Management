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
            var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Pfm.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var isPostgreSql = args.Any(a => a.Equals("postgresql", StringComparison.OrdinalIgnoreCase)) ||
                              configuration["Database:Provider"]?.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase) == true;

            var optionsBuilder = new DbContextOptionsBuilder<PfmDbContext>();
            var connectionString = isPostgreSql
                ? configuration.GetSection("Database:ConnectionStrings")["PostgreSql"]
                : configuration.GetSection("Database:ConnectionStrings")["SqlServer"];

            if (isPostgreSql)
            {
                optionsBuilder.UseNpgsql(connectionString);
                Console.WriteLine("Using PostgreSQL provider with connection: " + connectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(connectionString);
                Console.WriteLine("Using SQL Server provider with connection: " + connectionString);
            }

            return new PfmDbContext(optionsBuilder.Options, configuration);
        }
    }
    
}
