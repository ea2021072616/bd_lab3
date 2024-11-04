using ClienteAPI.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BdClientesContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ClienteDB")));

builder.Services.AddControllers();

// Configurar HealthChecks para la base de datos
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("ClienteDB"),
        name: "DatabaseHealth",
        failureStatus: HealthStatus.Unhealthy);

// Configurar Prometheus y Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMetricServer();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();

// Agregar métricas de HealthCheck para Prometheus
app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                exception = e.Value.Exception?.Message,
                duration = e.Value.Duration.ToString()
            })
        });
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
});
app.UseHttpMetrics();

app.MapControllers();
app.Run();
