using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MediatR.Behaviors.ResponseCaching
{
    public class MediatorResponseCache<TRequest, TResponse> : IMediatorResponseCache<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IOptions<MediatorResponseCacheOptions<TRequest, TResponse>> _options;
        private readonly ILogger<MediatorResponseCache<TRequest, TResponse>> _logger;

        public MediatorResponseCache(
            IDistributedCache distributedCache,
            IOptions<MediatorResponseCacheOptions<TRequest, TResponse>> options,
            ILogger<MediatorResponseCache<TRequest, TResponse>> logger
        )
        {
            _distributedCache = distributedCache;
            _options = options;
            _logger = logger;
        }

        public async Task<(bool Found, TResponse CachedResponse)> TryGetAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            var cacheKey = GetKey(request);

            _logger?.LogDebug("Checking distributed cache for {CacheKey}", cacheKey);

            var cachedResponse = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            if (cachedResponse != null)
            {
                try
                {
                    var cached = JsonConvert.DeserializeObject<TResponse>(cachedResponse);

                    _logger?.LogInformation("Cached response found for {CacheKey}", cacheKey);

                    return (true, cached);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Failed to deserialize cached response for {CacheKey}", cacheKey);
                }
            }

            _logger?.LogInformation("No cached response found for {CacheKey}", cacheKey);
            return (false, default);
        }

        public async Task SetAsync(TRequest request, TResponse response, CancellationToken cancellationToken = default)
        {
            var cacheKey = GetKey(request);

            try
            {
                _logger?.LogDebug("Caching response for {CacheKey}", cacheKey);
                var responseJson = JsonConvert.SerializeObject(response);
                await _distributedCache.SetStringAsync(cacheKey, responseJson, _options.Value.EntryOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to cache response for {CacheKey}", cacheKey);
            }
        }

        public async Task RemoveAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            var cacheKey = GetKey(request);

            _logger?.LogDebug("Clearing response for {CacheKey}", cacheKey);

            await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
        }

        private string GetKey(TRequest request)
        {
            return _options.Value.KeyPrefix + _options.Value.RequestHashDelegate(request);
        }
    }
}
