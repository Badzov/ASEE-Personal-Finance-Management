using Microsoft.Extensions.DependencyInjection;
using Pfm.Application.Services;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITransactionService, TransactionService>();
            return services;
        }
    }
}
