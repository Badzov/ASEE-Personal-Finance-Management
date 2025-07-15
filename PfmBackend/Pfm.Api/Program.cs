using Microsoft.OpenApi.Models;
using Pfm.Api.Formatters;
using Pfm.Api.Filters;
using Pfm.Api.Filters.Swagger;
using Pfm.Application.DependancyInjection;
using Pfm.Infrastructure.DependancyInjection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Pfm.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PFM API", Version = "v1" });
    c.OperationFilter<ErrorExamplesFilter>();
});

builder.Services.AddInfrastructure();
builder.Services.AddApplicationServices();


builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, new TextPlainInputFormatter());
    options.Filters.Add<DatabaseExceptionFilter>();
});

var app = builder.Build();

app.MapControllers();

app.UseMiddleware<ProblemDetailsMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

