using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Pfm.Application.UseCases.Transactions.Mappings;
using FluentValidation;
using Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Application.UseCases.Categories.Commands.ImportCategories;
using Pfm.Application.UseCases.Queries;
using Pfm.Application.UseCases.Transactions.Queries.GetTransactions;

namespace Pfm.Application.DependancyInjection
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

            services.AddScoped<IValidator<CategorizeTransactionCommand>, CategorizeTransactionCommandValidator>();
            services.AddScoped<IValidator<ImportTransactionsDto>, ImportTransactionsDtoValidator>();
            services.AddScoped<IValidator<ImportCategoriesDto>, ImportCategoriesDtoValidator>();
            services.AddScoped<IValidator<ImportCategoriesCommand>, ImportCategoriesCommandValidator>();
            services.AddScoped<IValidator<GetSpendingAnalyticsQuery>, GetSpendingAnalyticsQueryValidator>();
            services.AddScoped<IValidator<ImportTransactionsCommand>, ImportTransactionsCommandValidator>();
            services.AddScoped<IValidator<TransactionFilters>, TransactionFiltersValidator>();
            return services;
        }
    }
}
