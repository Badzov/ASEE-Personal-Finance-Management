using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pfm.Domain.Entities;
using Pfm.Domain.Interfaces;
using Pfm.Infrastructure.Data;
using Pfm.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PfmDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PfmDatabase")));
            services.AddScoped<IRepository<Transaction>, TransactionRepository>();

            return services;
        }
    }
}

