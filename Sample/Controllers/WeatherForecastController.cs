using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using MediatR.Behaviors.ResponseCaching;
using Microsoft.AspNetCore.Mvc;
using Sample.Requests;

namespace Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMediatorResponseCache<GetWeatherForecastRequest, GetWeatherForecastResponse> _mediatorResponseCache;

        public WeatherForecastController(IMediator mediator, IMediatorResponseCache<GetWeatherForecastRequest, GetWeatherForecastResponse> mediatorResponseCache)
        {
            _mediator = mediator;
            _mediatorResponseCache = mediatorResponseCache;
        }

        [HttpGet("{user}/{city}")]
        public async Task<IEnumerable<WeatherForecast>> GetAsync(string user, string city)
        {
            var result = await _mediator.Send(new GetWeatherForecastRequest
            {
                City = city,
                User = user
            });

            return result.Forecasts;
        }


        [HttpGet("{user}/{city}/invalidate")]
        public async Task<bool> InvalidateAsync(string user, string city)
        {
            await _mediatorResponseCache.RemoveAsync(new GetWeatherForecastRequest { City = city, User = user });

            return true;
        }
    }
}
