using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pfm.Domain.Entities;
using Pfm.Domain.Interfaces;
using Pfm.Infrastructure.Data;
using Pfm.Infrastructure.Repositories;

namespace Pfm.Infrastructure
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

            return services;
        }
    }
}

