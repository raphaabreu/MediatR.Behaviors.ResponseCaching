using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Behaviors.ResponseCaching
{
    public interface IMediatorResponseCache<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<(bool Found, TResponse CachedResponse)> TryGetAsync(TRequest request, CancellationToken cancellationToken = default);
        Task RemoveAsync(TRequest request, CancellationToken cancellationToken = default);
        Task SetAsync(TRequest request, TResponse response, CancellationToken cancellationToken = default);
    }
}
