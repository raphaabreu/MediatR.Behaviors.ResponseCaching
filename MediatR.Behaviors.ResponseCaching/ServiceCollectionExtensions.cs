using System;
using MediatR.Behaviors.ResponseCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MediatR
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatorResponseCaching<TRequest, TResponse>(
            this IServiceCollection services,
            Action<MediatorResponseCacheOptions<TRequest, TResponse>> configureAction
        ) where TRequest : IRequest<TResponse>
        {
            services.Configure(configureAction);

            services.TryAddSingleton<IMediatorResponseCache<TRequest, TResponse>, MediatorResponseCache<TRequest, TResponse>>();

            services.TryAddTransient<IPipelineBehavior<TRequest, TResponse>, ResponseCachingBehavior<TRequest, TResponse>>();

            return services;
        }

        public static IServiceCollection AddMediatorResponseCaching<TRequest, TResponse>(
            this IServiceCollection services,
            string cacheKeyPrefix,
            DistributedCacheEntryOptions entryOptions
        ) where TRequest : IRequest<TResponse>
        {
            services.AddMediatorResponseCaching<TRequest, TResponse>(opt =>
            {
                opt.KeyPrefix = cacheKeyPrefix;
                opt.EntryOptions = entryOptions;
            });

            return services;
        }

        public static IServiceCollection AddMediatorResponseCaching<TRequest, TResponse>(
            this IServiceCollection services,
            string cacheKeyPrefix,
            TimeSpan lifetime
        ) where TRequest : IRequest<TResponse>
        {
            services.AddMediatorResponseCaching<TRequest, TResponse>(opt => opt
                .WithPrefix(cacheKeyPrefix)
                .WithLifetime(lifetime)
            );

            return services;
        }

        public static IServiceCollection AddMediatorResponseCaching<TRequest, TResponse>(
            this IServiceCollection services,
            string cacheKeyPrefix,
            TimeSpan lifetime,
            Func<TRequest, object> indexingOn
        ) where TRequest : IRequest<TResponse>
        {
            services.AddMediatorResponseCaching<TRequest, TResponse>(opt => opt
                .WithPrefix(cacheKeyPrefix)
                .WithLifetime(lifetime)
                .WithIndexingOn(indexingOn)
            );

            return services;
        }
    }
}
