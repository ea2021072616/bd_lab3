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

// Obtener la cadena de conexión para la base de datos
var connectionString = builder.Configuration.GetConnectionString("ClienteDB");

if (!string.IsNullOrEmpty(connectionString))
{
    // Agregar el servicio de Health Checks
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            connectionString: connectionString,  // Cadena de conexión
            name: "ClienteDB_HealthCheck",        // Nombre del Health Check
            failureStatus: HealthStatus.Unhealthy // Estado de fallo
        );
}
else
{
    // Si no se encuentra la cadena de conexión, lanzar una excepción
    throw new InvalidOperationException("La cadena de conexión 'ClienteDB' no está configurada correctamente.");
}

// Configurar Prometheus y Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// **Definir la métrica de Health Check para Prometheus**
var healthCheckMetric = Metrics.CreateGauge(
    "healthcheck_status", 
    "Indicates the health status of the application. 1 for healthy, 0 for unhealthy."
);

// Usar el middleware para el endpoint de Health Checks y exponer métricas Prometheus
app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true, // Ejecuta todos los checks configurados
    ResponseWriter = async (context, report) =>
    {
        // Actualizar la métrica healthcheck_status en Prometheus
        healthCheckMetric.Set(report.Status == HealthStatus.Healthy ? 1 : 0);

        // Imprimir el estado de salud en consola (opcional)
        Console.WriteLine($"Health Check Status Updated: {report.Status}");

        // Serializar el reporte de estado de salud a formato JSON
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                exception = e.Value.Exception?.Message ?? "none",
                duration = e.Value.Duration.ToString()
            })
        });

        // Enviar la respuesta en formato JSON
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
});


// **Usar MetricServer solo una vez**
app.UseMetricServer();

// Configuración de Swagger y otros middleware
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

// Configuración del controlador y la ejecución
app.MapControllers();
app.Run();
