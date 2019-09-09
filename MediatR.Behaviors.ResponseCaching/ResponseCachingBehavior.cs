using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MediatR.Behaviors.ResponseCaching
{
    public class ResponseCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly string _cacheKeyPrefix;
        private readonly DistributedCacheEntryOptions _options;
        private readonly IDistributedCache _cache;
        private readonly IMetrics _metrics;
        private readonly ILogger<ResponseCachingBehavior<TRequest, TResponse>> _logger;

        public ResponseCachingBehavior(
            string cacheKeyPrefix,
            DistributedCacheEntryOptions options,
            IDistributedCache cache,
            IMetrics metrics,
            ILogger<ResponseCachingBehavior<TRequest, TResponse>> logger
        )
        {
            _cacheKeyPrefix = cacheKeyPrefix;
            _options = options;
            _cache = cache;
            _metrics = metrics;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var cacheKey = ComputeCacheKey(request);

            _logger?.LogDebug("Checking distributed cache for {CacheKey}", cacheKey);
            _metrics?.Measure.Meter.Mark(MetricsOptions.MEDIATOR_RESPONSE_CACHE_READ, new MetricTags("request", request.GetType().Name));

            var cachedResponse = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (cachedResponse != null)
            {
                try
                {
                    var cached = JsonConvert.DeserializeObject<TResponse>(cachedResponse);

                    _logger?.LogInformation("Cached response found for {CacheKey}", cacheKey);

                    return cached;
                }
                catch (Exception ex)
                {
                    _metrics?.Measure.Meter.Mark(MetricsOptions.MEDIATOR_RESPONSE_CACHE_READ_EXCEPTION, new MetricTags(new string[] { "request", "exception" }, new string[] { request.GetType().Name, ex.GetType().FullName }));
                    _logger?.LogError(ex, "Failed to deserialize cached response for {CacheKey}", cacheKey);
                }
            }

            _metrics?.Measure.Meter.Mark(MetricsOptions.MEDIATOR_RESPONSE_CACHE_READ_MISS, new MetricTags("request", request.GetType().Name));
            _logger?.LogInformation("Fetching {CacheKey} from upstream", cacheKey);

            var result = await next();

            try
            {
                _metrics?.Measure.Meter.Mark(MetricsOptions.MEDIATOR_RESPONSE_CACHE_WRITE, new MetricTags("request", request.GetType().Name));
                _logger?.LogDebug("Caching response for {CacheKey}", cacheKey);
                var responseJson = JsonConvert.SerializeObject(result);
                await _cache.SetStringAsync(cacheKey, responseJson, _options, cancellationToken);
            }
            catch (Exception ex)
            {
                _metrics?.Measure.Meter.Mark(MetricsOptions.MEDIATOR_RESPONSE_CACHE_WRITE_EXCEPTION, new MetricTags(new string[] { "request", "exception" }, new string[] { request.GetType().Name, ex.GetType().FullName }));
                _logger?.LogError(ex, "Failed to cache response for {CacheKey}", cacheKey);
            }

            return result;
        }

        private string ComputeCacheKey(TRequest request)
        {
            var requestJson = JsonConvert.SerializeObject(request);

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var textData = System.Text.Encoding.UTF8.GetBytes(requestJson);
                var hash = md5.ComputeHash(textData);
                return _cacheKeyPrefix + (new Guid(hash)).ToString("N");
            }
        }
    }
}
