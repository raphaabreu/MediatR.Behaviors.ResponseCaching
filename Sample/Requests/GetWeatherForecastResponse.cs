using System.Collections.Generic;

namespace Sample.Requests
{
    public class GetWeatherForecastResponse
    {
        public IEnumerable<WeatherForecast> Forecasts { get; set; }
    }
}
