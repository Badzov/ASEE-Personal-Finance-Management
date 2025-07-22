using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pfm.Application.Interfaces;
using Pfm.Domain.Interfaces;
using Pfm.Domain.Services;
using Pfm.Infrastructure.Persistence.DbContexts;
using Pfm.Infrastructure.Persistence.Repositories;
using Pfm.Infrastructure.Persistence.UnitOfWork;
using Pfm.Infrastructure.Services;

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

