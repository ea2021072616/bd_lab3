using Prometheus;

namespace ClienteAPI.Services
{
    public static class MetricsService
    {
        private static readonly Gauge TasaCambioPromedio = Metrics.CreateGauge("cambio_monedas_tasa_promedio", "Tasa promedio de cambio registrada");
        private static readonly Gauge TasaCambioMaxima = Metrics.CreateGauge("cambio_monedas_tasa_maxima", "Tasa máxima de cambio registrada");
        private static readonly Gauge TasaCambioMinima = Metrics.CreateGauge("cambio_monedas_tasa_minima", "Tasa mínima de cambio registrada");
        private static readonly Counter TotalLecturasCambio = Metrics.CreateCounter("cambio_monedas_total_lecturas", "Total de lecturas de tasas de cambio");

        public static void ReportarCambioMonedas(double tasaCambio)
        {
            TasaCambioPromedio.Set(tasaCambio);
            TasaCambioMaxima.Set(Math.Max(tasaCambio, TasaCambioMaxima.Value));
            TasaCambioMinima.Set(Math.Min(tasaCambio, TasaCambioMinima.Value));
            TotalLecturasCambio.Inc();
        }
    }
}
