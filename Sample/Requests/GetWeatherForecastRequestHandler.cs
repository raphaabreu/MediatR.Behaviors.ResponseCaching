using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Sample.Requests
{
    public class GetWeatherForecastRequestHandler : IRequestHandler<GetWeatherForecastRequest, GetWeatherForecastResponse>
    {
        private static readonly string[] SUMMARIES = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public async Task<GetWeatherForecastResponse> Handle(GetWeatherForecastRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);

            var rng = new Random();
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = SUMMARIES[rng.Next(SUMMARIES.Length)]
            })
                .ToArray();

            return new GetWeatherForecastResponse { Forecasts = forecasts };
        }
    }
}
