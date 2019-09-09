using System;
using App.Metrics;
using MediatR.Behaviors.ResponseCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediatR
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatorResponseCaching<TRequest, TResponse>(
            this IServiceCollection services,
            string cacheKeyPrefix,
            DistributedCacheEntryOptions options
        ) where TRequest : IRequest<TResponse>
        {
            services.AddTransient<IPipelineBehavior<TRequest, TResponse>>(provider =>
                new ResponseCachingBehavior<TRequest, TResponse>(
                    cacheKeyPrefix,
                    options,
                    provider.GetRequiredService<IDistributedCache>(),
                    provider.GetService<IMetrics>(),
                    provider.GetService<ILogger<ResponseCachingBehavior<TRequest, TResponse>>>()
                )
            );

            return services;
        }

        public static IServiceCollection AddMediatorResponseCaching<TRequest, TResponse>(
            this IServiceCollection services,
            string cacheKeyPrefix,
            TimeSpan absoluteExpirationRelativeToNow
        ) where TRequest : IRequest<TResponse>
        {
            services.AddTransient<IPipelineBehavior<TRequest, TResponse>>(provider =>
                new ResponseCachingBehavior<TRequest, TResponse>(
                    cacheKeyPrefix,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
                    },
                    provider.GetRequiredService<IDistributedCache>(),
                    provider.GetService<IMetrics>(),
                    provider.GetService<ILogger<ResponseCachingBehavior<TRequest, TResponse>>>()
                )
            );

            return services;
        }
    }
}
