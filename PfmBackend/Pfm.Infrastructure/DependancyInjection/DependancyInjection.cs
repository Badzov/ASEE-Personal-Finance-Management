using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pfm.Application.Interfaces;
using Pfm.Domain.Services;
using Pfm.Infrastructure.Persistence;
using Pfm.Infrastructure.Persistence.DbContexts;
using Pfm.Infrastructure.Persistence.Initializer;
using Pfm.Infrastructure.Persistence.Repositories;
using Pfm.Infrastructure.Persistence.UnitOfWork;
using Pfm.Infrastructure.Services;
using Pfm.Infrastructure.Services.RulesProvider;
using System.Configuration;

namespace Pfm.Infrastructure.DependancyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            var dbSettings = configuration.GetSection("Database").Get<DatabaseSettings>();
            services.AddSingleton(dbSettings);

            if (dbSettings.Provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
            {
                services.AddDbContext<PfmPostgreSqlDbContext>(options =>
                    options.UseNpgsql(dbSettings.ConnectionStrings["PostgreSql"],
                        o => o.MigrationsAssembly("Pfm.Infrastructure")
                              .MigrationsHistoryTable("__EFMigrationsHistory_PostgreSql")));
            }
            else
            {
                services.AddDbContext<PfmSqlServerDbContext>(options =>
                    options.UseSqlServer(dbSettings.ConnectionStrings["SqlServer"],
                        o => o.MigrationsAssembly("Pfm.Infrastructure")
                              .MigrationsHistoryTable("__EFMigrationsHistory_SqlServer")));
            }

            services.AddScoped<PfmDbContext>(provider =>
                dbSettings.Provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase)
                    ? provider.GetRequiredService<PfmPostgreSqlDbContext>()
                    : provider.GetRequiredService<PfmSqlServerDbContext>());

            services.AddHostedService<DbInitializer>();


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITransactionsCsvParser, TransactionCsvParser>();
            services.AddScoped<ICategoriesCsvParser, CategoriesCsvParser>();

           

            // Add YAML config for rules
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), "../auto-categorization-rules.yml");

            // For Docker environment fallback
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(AppContext.BaseDirectory, "auto-categorization-rules.yml");
            }

            if (!File.Exists(configPath))
            {
                throw new Exception($">>>>>>>>>>>>>>>>>> Critical configuration missing: {configPath}");
            }

            var yamlConfig = new ConfigurationBuilder()
                .AddYamlFile(configPath, optional: false, reloadOnChange: false)
                .Build();

            services.AddSingleton<IRulesProvider>(new RulesProvider(yamlConfig));

            services.AddSingleton<IRulesToSqlTranslator>(new RulesToSqlTranslator(dbSettings.Provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase)));

            services.AddScoped<IFileValidator, FileValidator>();

            return services;
        }
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<CategoryHierarchyService>(provider =>
            {
                var uow = provider.GetRequiredService<IUnitOfWork>();
                var categories = uow.Categories.GetAllAsync().GetAwaiter().GetResult(); 
                return new CategoryHierarchyService(categories);
            });

            return services;
        }
    }
}

