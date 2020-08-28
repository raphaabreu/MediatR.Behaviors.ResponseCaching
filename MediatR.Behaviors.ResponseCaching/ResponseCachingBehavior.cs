using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Behaviors.ResponseCaching
{
    public class ResponseCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IMediatorResponseCache<TRequest, TResponse> _responseCache;

        public ResponseCachingBehavior(IMediatorResponseCache<TRequest, TResponse> responseCache)
        {
            _responseCache = responseCache;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var (found, cachedResponse) = await _responseCache.TryGetAsync(request, cancellationToken);
            if (found)
            {
                return cachedResponse;
            }

            var result = await next();

            await _responseCache.SetAsync(request, result, cancellationToken);

            return result;
        }
    }
}
