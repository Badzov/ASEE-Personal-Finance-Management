using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pfm.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Persistence.Initializer
{
    public class DbInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var provider = config["Database:Provider"] ?? "SqlServer";

            if (provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PfmPostgreSqlDbContext>();
                await MigrateDatabase(dbContext, cancellationToken);
            }
            else
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PfmSqlServerDbContext>();
                await MigrateDatabase(dbContext, cancellationToken);
            }
        }

        private async Task MigrateDatabase(DbContext dbContext, CancellationToken cancellationToken)
        {
            try
            {
                if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    await dbContext.Database.MigrateAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during migration");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
