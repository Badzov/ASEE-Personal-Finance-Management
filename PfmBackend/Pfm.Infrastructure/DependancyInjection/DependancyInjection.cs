using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pfm.Application.Interfaces;
using Pfm.Domain.Services;
using Pfm.Infrastructure.Persistence.DbContexts;
using Pfm.Infrastructure.Persistence.Initializer;
using Pfm.Infrastructure.Persistence.Repositories;
using Pfm.Infrastructure.Persistence.UnitOfWork;
using Pfm.Infrastructure.Services;
using Pfm.Infrastructure.Services.RulesProvider;

namespace Pfm.Infrastructure.DependancyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddDbContext<PfmDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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

            services.AddSingleton<IRulesToSqlTranslator, RulesToSqlTranslator>();

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

