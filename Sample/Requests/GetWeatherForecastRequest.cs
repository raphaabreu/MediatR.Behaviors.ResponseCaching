using MediatR;

namespace Sample.Requests
{
    public class GetWeatherForecastRequest : IRequest<GetWeatherForecastResponse>
    {
        public string City { get; set; }
        public string User { get; set; }
    }
}
