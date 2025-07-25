using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pfm.Application.Interfaces;
using Pfm.Domain.Services;
using Pfm.Infrastructure.Persistence.DbContexts;
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
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddDbContext<PfmDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MSSQLPfmDatabase")));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITransactionsCsvParser, TransactionCsvParser>();
            services.AddScoped<ICategoriesCsvParser, CategoriesCsvParser>();

            var rulesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "auto-categorization-rules.yml");
            if (!File.Exists(rulesPath)) {
                throw new Exception($">>>>>>>>>>>>>>>>>> Critical configuration missing: {rulesPath}");
            }
            // Add YAML config for rules
            var yamlConfig = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), ".."))
                .AddYamlFile(Path.Combine("auto-categorization-rules.yml"), optional: false, reloadOnChange: false)
                .Build();

            services.AddSingleton<IRulesProvider>(new RulesProvider(yamlConfig));

            services.AddSingleton<IRulesToSqlTranslator, RulesToSqlTranslator>();

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

