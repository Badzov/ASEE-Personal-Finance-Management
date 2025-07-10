using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Pfm.Application.UseCases.Transactions.Mappings;

namespace Pfm.Application
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile<TransactionProfile>();
            });

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependancyInjection).Assembly));

            return services;
        }
    }
}
