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
app.UseHttpMetrics();

// Métricas para cambios de moneda
var TasaCambioMedia = Metrics.CreateGauge("cambio_monedas_tasa_media", "Tasa promedio de cambio de moneda.");
var TasaCambioMaxima = Metrics.CreateGauge("cambio_monedas_tasa_maxima", "Tasa máxima de cambio de moneda registrada.");
var TasaCambioMinima = Metrics.CreateGauge("cambio_monedas_tasa_minima", "Tasa mínima de cambio de moneda registrada.");
var TotalLecturasCambio = Metrics.CreateCounter("cambio_monedas_total_lecturas", "Total de lecturas de tasa de cambio.");

// Simular la recolección de datos de cambio de moneda
app.Lifetime.ApplicationStarted.Register(() =>
{
    var random = new Random();
    Task.Run(async () =>
    {
        while (true)
        {
            // Generar datos de tasa de cambio aleatoria entre 18 y 25
            var tasaCambio = random.NextDouble() * (25 - 18) + 18;

            // Actualizar las métricas de Prometheus
            TasaCambioMedia.Set(tasaCambio);
            TasaCambioMaxima.Set(Math.Max(tasaCambio, TasaCambioMaxima.Value));
            TasaCambioMinima.Set(Math.Min(tasaCambio, TasaCambioMinima.Value));
            TotalLecturasCambio.Inc();

            // Esperar 5 segundos antes de la siguiente lectura
            await Task.Delay(5000);
        }
    });
});

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

app.MapControllers();
app.Run();
