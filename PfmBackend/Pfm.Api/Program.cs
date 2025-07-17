using Microsoft.OpenApi.Models;
using Pfm.Api.Formatters;
using Pfm.Api.Filters;
using Pfm.Application.DependancyInjection;
using Pfm.Infrastructure.DependancyInjection;
using Pfm.Api.Middleware;
using Pfm.Api.Serialization;
using System.Text.Json.Serialization;
using Pfm.Api.Swagger;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PFM API", Version = "v1" });
    c.CustomSchemaIds(x => SchemaNaming.GetSchemaId(x));
});

builder.Services.AddInfrastructure();
builder.Services.AddApplicationServices();


builder.Services
    .AddControllers(options =>
    {
        options.InputFormatters.Insert(0, new TextPlainInputFormatter());
        options.Filters.Add<DatabaseExceptionFilter>();

    })
    .AddJsonOptions(json =>
    {
        json.JsonSerializerOptions.PropertyNamingPolicy = new KebabCaseNamingPolicy();
        json.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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

