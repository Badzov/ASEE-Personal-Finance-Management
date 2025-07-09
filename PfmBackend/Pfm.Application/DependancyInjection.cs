using Microsoft.Extensions.DependencyInjection;
using Pfm.Application.Services;
using Pfm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Pfm.Application.Mappings.Transactions;

namespace Pfm.Application
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<TransactionProfile>();
            });

            services.AddScoped<ITransactionService, TransactionService>();
            return services;
        }
    }
}
