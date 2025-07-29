using Microsoft.EntityFrameworkCore;
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

            var dbSettings = new DatabaseSettings();
            configuration.GetSection("Database").Bind(dbSettings);
            services.AddSingleton(dbSettings);

            services.AddDbContext<PfmDbContext>(options =>
            {
                if (dbSettings.Provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseNpgsql(dbSettings.ConnectionStrings["PostgreSql"],
                        npgsql => npgsql.MigrationsAssembly("Pfm.Infrastructure"));
                }
                else
                {
                    options.UseSqlServer(dbSettings.ConnectionStrings["SqlServer"],
                        sql => sql.MigrationsAssembly("Pfm.Infrastructure"));
                }
            });


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

