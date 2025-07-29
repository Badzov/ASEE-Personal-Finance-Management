using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pfm.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
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
            var dbContext = scope.ServiceProvider.GetRequiredService<PfmDbContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var provider = config["Database:Provider"] ?? "SqlServer";
            var connectionString = provider switch
            {
                "PostgreSql" => config.GetConnectionString("PostgreSql"),
                _ => config.GetConnectionString("SqlServer")
            };

            _logger.LogInformation($"Using {provider} with connection: {connectionString}");

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);

            var providerMigrations = pendingMigrations
                .Where(m => m.EndsWith($"_PostgreSql") == (provider == "PostgreSql"))
                .ToList();

            if (providerMigrations.Any())
            {
                _logger.LogInformation($"Applying {providerMigrations.Count} {provider} migrations...");
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            else
            {
                _logger.LogInformation("No pending migrations for current provider");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
