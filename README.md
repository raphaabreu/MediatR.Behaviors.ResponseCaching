# MediatR.Behaviors.ResponseCaching

This is a very thin behavior that allows responses from mediator request handlers to be cached
and reused for subsequent calls.

Installing

```
Install-Package MediatR.Behaviors.ResponseCaching
```

Configuring

```
services.AddMediatorResponseCaching<GetWeatherForecastRequest, GetWeatherForecastResponse>(
    "WEATHER-FORECAST:", TimeSpan.FromMinutes(1), r => r.City);
```

That is it.

You can customize the key prefix, request hash generation and what fields inside the request
should be used to differentiate one call from the next.

If you need to invalidate a previously cached response, you can do so by injecting an instance
of `IMediatorResponseCache<TRequest, TResponse>` and calling `.RemoveAsync(TRequest request)`.

Sample project available.
