using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Pfm.Application.UseCases.Transactions.Mappings;
using FluentValidation;
using Pfm.Application.UseCases.Transactions.Commands.CategorizeTransaction;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Application.UseCases.Categories.Commands.ImportCategories;
using Pfm.Application.UseCases.SpendingAnalytics.Mappings;

namespace Pfm.Application.DependancyInjection
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile<TransactionProfile>();
                config.AddProfile<SpendingAnalysisProfile>();
            });

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependancyInjection).Assembly));

            services.AddScoped<IValidator<TransactionCategoryDto>, TransactionCategoryDtoValidator>();
            services.AddScoped<IValidator<ImportTransactionsDto>, ImportTransactionsDtoValidator>();
            services.AddScoped<IValidator<ImportCategoriesDto>, ImportCategoriesDtoValidator>();
            return services;
        }
    }
}
