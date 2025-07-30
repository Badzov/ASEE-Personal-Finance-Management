using Microsoft.OpenApi.Models;
using Pfm.Api.Formatters;
using Pfm.Application.DependancyInjection;
using Pfm.Infrastructure.DependancyInjection;
using Pfm.Api.Middleware;
using Pfm.Api.Serialization;
using System.Text.Json.Serialization;
using Pfm.Api.Swagger;
using Microsoft.AspNetCore.Mvc;
using Pfm.Api.Filters;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PFM API", Version = "v1" });
    c.CustomSchemaIds(x => SchemaNaming.GetSchemaId(x));
    c.OperationFilter<DefaultProblemResponseFilter>();
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddDomainServices();

//Frontent integration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Services
    .AddControllers(options =>
    {
        options.InputFormatters.Insert(0, new TextPlainInputFormatter());
        options.Filters.Add<ModelValidationFilter>();
    })
    .AddJsonOptions(json =>
    {
        json.JsonSerializerOptions.PropertyNamingPolicy = new KebabCaseNamingPolicy();
        json.JsonSerializerOptions.Converters.Add(new MccCodeEnumConverter()); 
        json.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); 
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//Frontend integration
app.UseCors();

app.UseHttpsRedirection();
app.MapControllers();
app.UseMiddleware<ProblemDetailsMiddleware>();

app.Run();