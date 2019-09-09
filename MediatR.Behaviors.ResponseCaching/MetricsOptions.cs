using App.Metrics;
using App.Metrics.Meter;

namespace MediatR.Behaviors.ResponseCaching
{
    internal static class MetricsOptions
    {
        public static readonly MeterOptions MEDIATOR_RESPONSE_CACHE_READ = new MeterOptions
        {
            Name = "Mediator Response Cache Read",
            MeasurementUnit = App.Metrics.Unit.Requests,
            RateUnit = TimeUnit.Seconds,
            Context = "application"
        };

        public static readonly MeterOptions MEDIATOR_RESPONSE_CACHE_READ_MISS = new MeterOptions
        {
            Name = "Mediator Response Cache Read Miss",
            MeasurementUnit = App.Metrics.Unit.Requests,
            RateUnit = TimeUnit.Seconds,
            Context = "application"
        };

        public static readonly MeterOptions MEDIATOR_RESPONSE_CACHE_READ_EXCEPTION = new MeterOptions
        {
            Name = "Mediator Response Cache Read Exception",
            MeasurementUnit = App.Metrics.Unit.Requests,
            RateUnit = TimeUnit.Seconds,
            Context = "application"
        };

        public static readonly MeterOptions MEDIATOR_RESPONSE_CACHE_WRITE = new MeterOptions
        {
            Name = "Mediator Response Cache Write",
            MeasurementUnit = App.Metrics.Unit.Requests,
            RateUnit = TimeUnit.Seconds,
            Context = "application"
        };

        public static readonly MeterOptions MEDIATOR_RESPONSE_CACHE_WRITE_EXCEPTION = new MeterOptions
        {
            Name = "Mediator Response Cache Write Exception",
            MeasurementUnit = App.Metrics.Unit.Requests,
            RateUnit = TimeUnit.Seconds,
            Context = "application"
        };
    }
}
