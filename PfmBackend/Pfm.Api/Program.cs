using Pfm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pfm.Infrastructure;
using Microsoft.OpenApi.Models;
using Pfm.Application;
using Pfm.Domain.Interfaces;
using Pfm.Api.Formatters;
using Pfm.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PFM API", Version = "v1" });
});

builder.Services.AddInfrastructure();
builder.Services.AddApplicationServices();
builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, new TextPlainInputFormatter());
    options.Filters.Add<AppExceptionFilter>();
});


var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

