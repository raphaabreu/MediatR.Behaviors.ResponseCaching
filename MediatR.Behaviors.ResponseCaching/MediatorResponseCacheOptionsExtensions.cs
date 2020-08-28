using System;
using MediatR.Behaviors.ResponseCaching;

namespace MediatR
{
    public static class MediatorResponseCacheOptionsExtensions
    {
        public static MediatorResponseCacheOptions<TRequest, TResponse> WithPrefix<TRequest, TResponse>(
            this MediatorResponseCacheOptions<TRequest, TResponse> options, string prefix) where TRequest : IRequest<TResponse>
        {
            options.KeyPrefix = prefix;

            return options;
        }

        public static MediatorResponseCacheOptions<TRequest, TResponse> WithLifetime<TRequest, TResponse>(
            this MediatorResponseCacheOptions<TRequest, TResponse> options, TimeSpan lifetime) where TRequest : IRequest<TResponse>
        {
            options.EntryOptions.AbsoluteExpirationRelativeToNow = lifetime;

            return options;
        }

        public static MediatorResponseCacheOptions<TRequest, TResponse> WithIndexingOn<TRequest, TResponse>(
            this MediatorResponseCacheOptions<TRequest, TResponse> options, Func<TRequest, object> keySelection) where TRequest : IRequest<TResponse>
        {
            options.RequestHashDelegate = request => DefaultRequestHasher.JsonMd5(keySelection(request));

            return options;
        }
    }
}
